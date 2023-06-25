using System.Collections;
using System.Collections.Generic;
using FrostyScripts.Fader;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Athena.ChainReaction
{
    public class CR_Menu : MonoBehaviour
    {
        [SerializeField] private Slider height;
        [SerializeField] private Slider width;
        [SerializeField] private Slider players;
        [SerializeField] private TMP_Text htxt;
        [SerializeField] private TMP_Text wtxt;
        [SerializeField] private TMP_Text ptxt;

        [SerializeField] private Button PlayBtn;
        [SerializeField] private SceneFader fader;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip playBtnAudioClip;
        private void Awake()
        {
            PlayBtn.onClick.AddListener(LoadGame);
        }

        void LoadGame()
        {
            var data = new CR_GameData((int)width.value, (int)height.value, (int)players.value);



            if (SceneDataStore.Instance != null)
                Destroy(SceneDataStore.Instance.gameObject);
            var DataStoreObj = new GameObject("SceneDataStore");
            var ds = DataStoreObj.AddComponent<SceneDataStore>();
            ds.data = data;
            audioSource.PlayOneShot(playBtnAudioClip);
            fader.FadeToScene("CR_Game");
        }
        
        private void Update()
        {
            htxt.text = height.value.ToString();
            wtxt.text = width.value.ToString();
            ptxt.text = players.value.ToString();
        }


    }
}
