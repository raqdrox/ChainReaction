using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Athena.ChainReaction
{
    public class CR_Rotate : MonoBehaviour
    {
        void Start()
        {
            var rot1 = transform.DORotate(new Vector3(360f, 360f, 0f), 5f, RotateMode.FastBeyond360)
                .SetRelative()
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
            
        }

        
    }
}
