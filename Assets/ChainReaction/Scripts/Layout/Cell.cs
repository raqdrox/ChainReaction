using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Athena.ChainReaction
{
    public class Cell : MonoBehaviour
    {
        public Tuple<int, int> idx;

        public Blob blob;
        public Vector2 Pos => transform.position;


    }
}
