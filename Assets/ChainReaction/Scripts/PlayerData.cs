using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Athena.ChainReaction
{
    [CreateAssetMenu(menuName = "Athena/ChainReaction/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public List<Player> Data;
    }
}
