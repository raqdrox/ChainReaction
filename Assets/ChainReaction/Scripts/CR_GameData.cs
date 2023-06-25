using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Athena.ChainReaction
{
    public class CR_GameData 
    {
        public int Width;
        public int Height;
        public int PlayerCount;

        public CR_GameData(int width, int height, int playerCount)
        {
            Width = width;
            Height = height;
            PlayerCount = playerCount;
        }
    }
}
