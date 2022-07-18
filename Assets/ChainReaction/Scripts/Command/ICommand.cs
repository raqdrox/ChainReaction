using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Athena.ChainReaction
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}
