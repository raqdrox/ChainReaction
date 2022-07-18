using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Athena.ChainReaction
{
    [Serializable]
    public class Player
    {
        public int Id;
        public bool FirstTurn=true;
        public Material Mat;
        public Color PCol;
        public string PName;
    }

}
