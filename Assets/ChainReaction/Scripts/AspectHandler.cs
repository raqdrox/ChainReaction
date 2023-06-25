using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Athena.ChainReaction
{
    public class AspectHandler : MonoBehaviour
    {
        private Camera cam;
        [SerializeField]private float sizeInMeters=1f;
        private float uiHeight=0f;
        private float uiWidth=0f;
        private float orthoSize=0f;

        [SerializeField] private List<RectTransform> uiWidthComponents;
        [SerializeField] private List<RectTransform> uiHeightComponents;

        private void Awake()
        {
            cam= GetComponent<Camera>();
            FixUi();
        }

        
        void FixUi()
        {
            uiWidth = 0f;
            uiHeight = 0f;
            if (uiHeightComponents.Count!=0)
            {
                foreach (var heightComponent in uiHeightComponents)
                {
                    uiHeight += heightComponent.rect.height;
                }
            }

            if (uiWidthComponents.Count != 0)
            {
                foreach (var widthComponent in uiWidthComponents)
                {
                    uiWidth += widthComponent.rect.width;
                }
            }

            orthoSize = sizeInMeters * ((Screen.height + uiHeight) / (Screen.width + uiWidth)) ;
            cam.orthographicSize = orthoSize;
        }
    }
}