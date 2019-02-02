using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LimoKnight
{

    public class CharacterDialogueFade : MonoBehaviour
    {

        private CanvasGroup _canvasGroup;
        private float _speedTransition = 0.14f;
        [SerializeField] private float _delay = 1.8f;
        [SerializeField] private float _positionOffset = 200f;
        public bool IsVisible = false;
        public bool IsNotVisible = true;


        private void OnEnable()
        {
            if (_canvasGroup == null) _canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
            StartCoroutine("FadeIn");
        }

        public void HideDialogue()
        {
            StartCoroutine("FadeOut");
        }

        private IEnumerator FadeIn()
        {
            IsNotVisible = false;
            float count = 0f;
            while (count < 1f)
            {
                yield return new WaitForSeconds(Time.deltaTime * _delay);
                count = _delay * Time.deltaTime + count;
                if (count > 1f) count = 1f;
                _canvasGroup.alpha = count;
                if (_canvasGroup.alpha == 1f) IsVisible = true;
            }
            yield return null;
        }

        private IEnumerator FadeOut()
        {
            IsVisible = false;
            float count = 1f;
            while (count > 0f)
            {
                yield return new WaitForSeconds(Time.deltaTime * _delay);
                count = count - _delay * Time.deltaTime;
                if (count < 0f) count = 0f;
                _canvasGroup.alpha = count;
                if (_canvasGroup.alpha == 0f) IsNotVisible = true;
            }
            yield return null;
        }
    }

}