using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Athena.ChainReaction
{[RequireComponent(typeof(Image),typeof(Animator))]
    public class SceneFader : MonoBehaviour
    {

        private Image fadeImage;
        private Animator fadeImageAnimator;

        void Awake()
        {
            fadeImage = GetComponent<Image>();
            fadeImageAnimator = GetComponent<Animator>();
        }
        public void FadeToScene(string scene)
        {
            StartCoroutine(LoadScene(scene));
        }
        IEnumerator LoadScene(string scene)
        {
            fadeImageAnimator.SetTrigger("Fade");
            yield return new WaitUntil(() => fadeImage.color.a == 1f);
            SceneManager.LoadScene(scene);
        }
    }
}
