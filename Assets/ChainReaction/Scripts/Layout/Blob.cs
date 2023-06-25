using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Athena.ChainReaction
{
    public class Blob : MonoBehaviour
    {
        private int value=0;
        public Cell cell;
        [SerializeField] private TMP_Text _rendererText;
        private Player owner;
        [SerializeField] private List<MeshRenderer> _meshRenderers;
        public Vector2 Pos => transform.position;

        public int Value { 
            get => value; 
            set { 
                this.value = value;
                UpdateRenderer();
            } 
        }

        public Player Owner { get => owner; set { owner = value; UpdateRenderer(); } }

        private void UpdateRenderer()
        {
            if (value-1 < 0 || value-1 >= _meshRenderers.Count)
                return;
            //_rendererText.color = owner.PCol;
            //_rendererText.text = value.ToString();
            for (int i = 0; i < _meshRenderers.Count; i++)
            {
                if(i==(value-1))
                    _meshRenderers[i].enabled = true;
                else
                    _meshRenderers[i].enabled = false;
                _meshRenderers[i].material = owner.Mat;
            }
        }

        

    }
}
