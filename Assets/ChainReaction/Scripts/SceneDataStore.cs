using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Athena.ChainReaction
{
    public class SceneDataStore : MonoBehaviour
    {
        public static SceneDataStore Instance;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public CR_GameData data;
    }
}
