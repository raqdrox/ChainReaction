using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Athena.ChainReaction
{
    public class PlaceBlobCommand : ICommand
    {
        public Player _player;
        public Cell _cell;
        public Cell[,] _savedState;
        public CRGrid _grid;
        public int _blobVal=0;


        public bool _saveCell;
        public bool _firstTurn;
        public PlaceBlobCommand(Player player, CRGrid grid, Cell cell,bool saveCell=false)
        {
            _player = player;
            _cell = cell;
            _grid = grid;
            _savedState = _grid._cells;
            _saveCell = saveCell;
            _firstTurn = player.FirstTurn;
        }

        
        public void Execute()
        {
            var blob=CR_GameManager.Instance.CreateBlobInCell(_cell);
            if(_saveCell)
                CR_GameManager.Instance.PreviousCell = _cell;
            blob.Owner = _player;
            
            blob.Value++;
            _blobVal = blob.Value;
            _player.FirstTurn = false;
        }

        public void Undo()
        {
            
            _grid._cells = _savedState;

            if (_saveCell)
                CR_GameManager.Instance.PreviousCell = _cell;

            _player.FirstTurn = _firstTurn;

            if(_cell.blob!=null)
                _cell.blob.Value--;
        }
        
    }
}
