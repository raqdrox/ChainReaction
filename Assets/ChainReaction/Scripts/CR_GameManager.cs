using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrostyScripts.Fader;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Athena.ChainReaction
{
    public class CR_GameManager : MonoBehaviour
    {
        public static CR_GameManager Instance;

        [SerializeField] private Camera _camera;

        private GameState _state;
        private CommandManager _commandManager;

        [SerializeField] private SceneFader fader;

        //GenerateLevel
        [SerializeField] private GameObject _boardParent;
        [SerializeField] private int _width = 4;
        [SerializeField] private int _height = 4;
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private GameObject _boardPrefab;
        [SerializeField] private GameObject _blobPrefab;
        [SerializeField] private float sizeInMeters;
        [SerializeField] private float uiheight;
        private CRGrid _grid;
        //TurnSelect
        [SerializeField] private int _playerCount = 2;
        [SerializeField] PlayerData _playerData;
        [SerializeField] List<Player>  _playerList;
        List<Player> _players;
        int _currPlayerIdx = 0;
        private SpriteRenderer _bgSprite;
        //PlayerInput
        [SerializeField] private float _cellInputSize;
        public Cell PreviousCell;

        //Explosion
        private List<Cell> _expList;
        
        float _travelTime = 0.5f;
        [SerializeField]private GameObject _blobMovePrefab;
        

        //Win
        private Player _winner;
        [SerializeField] private TMP_Text Player_Text;
        //Debug
        [SerializeField] private TMP_Text State_Text;

        //Audio
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip popAudioClip;
        [SerializeField] private AudioClip playBtnAudioClip;

        private void Awake()
        {
            Instance = this;
            _expList = new List<Cell>();
            _commandManager = new CommandManager();
            
        }

        private void SetupData(CR_GameData data)
        {
            _width = data.Width;
            _height = data.Height;
            _playerCount = data.PlayerCount;
        }

        private void Start()
        {
            CR_GameData data;
            if (SceneDataStore.Instance != null)
            {
                data = SceneDataStore.Instance.data;
            }
            else
            {
                data = new CR_GameData(7, 13, 2);
            }
            SetupData(data);
            _playerList = Instantiate(_playerData).Data;
            ChangeState(GameState.GenerateLevel);
        }



        private void ChangeState(GameState newState)
        {

            
            _state = newState;
            switch (newState)
            {
                case GameState.GenerateLevel:
                    SetupPlayers();
                    GenerateGrid();
                    ChangeState(GameState.TurnSelect);
                    break;
                case GameState.WaitingInput:
                    break;
                case GameState.Win:
                    ShowWinScreen();
                    break;
                case GameState.Explosion:
                    ExplosionStateHandler();
                    break;
                case GameState.TurnSelect:
                    SelectTurn(true);
                    ChangeState(GameState.WaitingInput);
                    break;
                case GameState.UndoTurn:
                    SelectTurn(false);
                    ChangeState(GameState.WaitingInput);
                    break;
                default:
                    break;
            }

        }

        private void ShowWinScreen()
        {
            //WinScreen.SetActive(true);
            Player_Text.SetText(_winner.PName+" Wins!");
            Player_Text.color=_winner.PCol;
        }

        //GenerateLevel
        private void SetupPlayers()
        {
            _players = _playerList.GetRange(0, _playerCount);
            int pid = 0;
            foreach (var plr in _players)
            {
                plr.Id = pid;
                pid++;
            }
        }

        private void GenerateGrid()
        {
            _grid = new CRGrid(_width, _height);

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    var cell = Instantiate(_cellPrefab, new Vector2(x, y), Quaternion.identity,_boardParent.transform).GetComponent<Cell>();
                    cell.transform.position += new Vector3(0.5f,0.5f,0f);
                    _grid._cells[x, y] = cell;
                    cell.idx = new Tuple<int, int>(x, y);
                }
            }
            var center = new Vector2((_width / 2f) , (_height / 2f) );

            var board = Instantiate(_boardPrefab, center, Quaternion.identity, _boardParent.transform).GetComponent<SpriteRenderer>();
            board.size = new Vector2(_width-0.1f, _height -0.3f);
            _bgSprite = board;
            _camera.transform.position = new Vector3(center.x, center.y+0.4f, -10f);
            
            
            
        }


        //TurnSelect & UndoTurn

        void SelectTurn(bool next = true)
        {
            if (!_players[_currPlayerIdx].FirstTurn || !next)
            {
                var plrGrid = PlayersOnGrid();
                var removePlayers = new List<Player>();
                foreach (var plr in _players)
                {
                    if (!plrGrid.Contains(plr))
                        removePlayers.Add(plr);
                }
                var currPlayer = _players[_currPlayerIdx];
                foreach (var plr in removePlayers)
                {
                    _players.Remove(plr);
                }
                _currPlayerIdx = _players.IndexOf(currPlayer);
                if (next)
                {
                    _currPlayerIdx++;
                    _currPlayerIdx %= _players.Count;
                }
                else
                {
                    _currPlayerIdx--;
                    if (_currPlayerIdx < 0)
                        _currPlayerIdx = _players.Count - 1;
                }
            }
            _bgSprite.color = _players[_currPlayerIdx].Mat.color;
            Player_Text.SetText(_players[_currPlayerIdx].PName+"'s Turn");
            Player_Text.color = _players[_currPlayerIdx].PCol;


        }

        //InputState

        private void Update()
        {
            if (_state == GameState.WaitingInput)
            {
                //Input Check
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    HandleInput(worldPosition);
                }
            }
            State_Text.text = _state.ToString();
        }

        void HandleInput(Vector3 pos)
        {
            //Get Cell
            var cell = GetCellContainingPoint(pos);
            if (cell == null)
                return;
            if (cell.blob != null && cell.blob.Owner != _players[_currPlayerIdx])
                return;
            //Command PlaceBlob
            var placeCmd = new PlaceBlobCommand(_players[_currPlayerIdx], _grid, cell,true);
            _commandManager.RunCommandAndSave(placeCmd);
            ExpLogic(cell);
        }

        private void ExpLogic(Cell cell)
        {
            //ExpLogic
            if (CheckForExplodable(cell))
                _expList.Add(cell);

            //GoToExp if _expList has content
            if (_expList.Count != 0)
                ChangeState(GameState.Explosion);
            else
                ChangeState(GameState.TurnSelect);
        }

        Cell GetCellContainingPoint(Vector3 point)
        {//BAD CODE PLEASE OPTIMIZE
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (Vector2.Distance(_grid._cells[x, y].Pos, point) <= _cellInputSize)
                        return _grid._cells[x, y];
                }
            }
            return null;
        }

        //ExplosionState

        bool CheckForExplodable(Cell cell)
        {
            var adjacent = _grid.GetAdjacentCells(cell);
            if (cell.blob == null)
                return false;
            return cell.blob.Value >= adjacent.Count;
        }

        void ExplosionStateHandler()
        {
            StartCoroutine(ExplosionHandler());
        }
        IEnumerator ExplosionHandler_Old()
        {
            var wait = false;
            while (_expList.Count!=0)
            {
                //wait = true;

                var newList = new List<Cell>();
                foreach (var cell in _expList)
                {
                    var adjCells = _grid.GetAdjacentCells(cell);
                    RemoveBlobInCell(cell);


                    //DOTween
                    foreach (var item in adjCells)
                    {
                        //Spawn orb pf
                        //create lerp sequence

                    }
                    //execute sequence
                    

                    foreach (var item in adjCells)
                    {
                        var placeCmd = new PlaceBlobCommand(_players[_currPlayerIdx], _grid, item);
                        _commandManager.RunCommand(placeCmd);
                        if (CheckForExplodable(item) && !newList.Contains(item) && !_expList.Contains(item))
                            newList.Add(item);
                    }

                    
                }
                var winner = CheckWinCondition();
                if (winner != null)
                {
                    _winner = winner;
                    ChangeState(GameState.Win);
                }
                yield return new WaitUntil(() => !wait);
                _expList = newList;
                
            }
            ChangeState(GameState.TurnSelect);
        }
        IEnumerator ExplosionHandler()
        {
            var wait = false;
            while (_expList.Count != 0)
            {

                wait = true;
                var sequence = DOTween.Sequence();
                var movingBlobs = new List<GameObject>();
                var newList = new List<Cell>();
                var adjCellList = new List<Cell>();
                foreach (var cell in _expList)
                {
                    var adjCells = _grid.GetAdjacentCells(cell);
                    RemoveBlobInCell(cell);

                    foreach (var item in adjCells)
                    {
                        GameObject moveBlob = Instantiate(_blobMovePrefab, cell.transform);
                        moveBlob.GetComponent<MeshRenderer>().material = _players[_currPlayerIdx].Mat;
                        movingBlobs.Add(moveBlob);
                        sequence.Insert(0, moveBlob.transform.DOMove(item.Pos, _travelTime));

                        var placeCmd = new PlaceBlobCommand(_players[_currPlayerIdx], _grid, item);
                        _commandManager.AddCommandToQueue(placeCmd);
                        adjCellList.Add(item);
                    }


                }
                sequence.OnComplete(() => {
                    foreach (var item in movingBlobs)
                    {
                        Destroy(item);
                    }
                    _commandManager.RunQueue();
                    foreach (var item in adjCellList)
                    {
                        if (CheckForExplodable(item) && !newList.Contains(item) && !_expList.Contains(item))
                        {
                            newList.Add(item);
                            
                        }
                    }
                    var winner = CheckWinCondition();

                    if (winner != null)
                    {
                        _winner = winner;
                        ChangeState(GameState.Win);
                    }
                    wait = false;
                    
                });
                sequence.Play();
                audioSource.PlayOneShot(popAudioClip);
                yield return new WaitUntil(() => !wait);
                if(_state == GameState.Win)
                {
                    yield break;
                }
                _expList = newList;

            }
            ChangeState(GameState.TurnSelect);
        }

        //WinCheck
        Player CheckWinCondition()
        {
            var plrs = PlayersOnGrid();
            if(plrs.Count==1)
            {
                Debug.Log("Winner: "+ plrs[0].Id);
                return plrs[0];
            }
            return null;

        }
        List<Player> PlayersOnGrid()
        {
            //BAD CODE PLEASE OPTIMISE
            List<Player> plrs = new List<Player>();
            if (_players.FindAll(p => p.FirstTurn == true).Count != 0)
                return _players;
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if(_grid._cells[x,y].blob!=null)
                    {
                        if (!plrs.Contains(_grid._cells[x, y].blob.Owner))
                            plrs.Add(_grid._cells[x, y].blob.Owner);
                    }
                }
            }

            return plrs;
        }
        public Blob CreateBlobInCell(Cell cell,int val=0)
        {
            if (cell.blob == null)
            {
                var blob = Instantiate(_blobPrefab, cell.transform).GetComponent<Blob>();
                blob.cell = cell;
                blob.Value = val;
                cell.blob = blob;

            }
            return cell.blob;
        }
        public void RemoveBlobInCell(Cell cell)
        {
            Destroy(cell.blob.gameObject);
            cell.blob = null;
        }

        public void UndoMove()
        {//BREAKS IF EXPLODED BEFORE
            if (_winner != null)
                return;
            Debug.Log("Undo");
            if (_commandManager.CanUndo)
            {
                _commandManager.UndoCommand();
                StopAllCoroutines();
                ChangeState(GameState.UndoTurn);
            }
        }
        public void RedoMove()
        {//BROKEN
            Debug.Log("Redo");
            if (_commandManager.CanRedo)
            {
                _commandManager.RedoCommand();
                ExpLogic(PreviousCell);
            }
        }

        public void RestartGame()
        {
            audioSource.PlayOneShot(playBtnAudioClip);
            fader.FadeToScene("CR_Game");
        }

        public void GoToMenu()
        {
            audioSource.PlayOneShot(playBtnAudioClip);
            fader.FadeToScene("CR_Menu");
        }
    }
    public enum GameState
    {
        GenerateLevel,
        WaitingInput,
        Explosion,
        TurnSelect,
        UndoTurn,
        Win
    }
}
