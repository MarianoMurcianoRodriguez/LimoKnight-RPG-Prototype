using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    
    public class EnviromentTrigger : MonoBehaviour
    { 
        [SerializeField] private float _sizeActionSphere = 1f;
        [SerializeField] private GameObject _optionPanel;
        private bool _inside = false;
        private bool _wasTriggeredNow = false;

        private void Start()
        {
            StartCoroutine(DetectCollisionCorutine());
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gameObject.transform.position, _sizeActionSphere);
        }
        
        private IEnumerator DetectCollisionCorutine()
        {
            Collider[] colls;
            do
            {
                _inside = false;
                colls = Physics.OverlapSphere(gameObject.transform.position, _sizeActionSphere);
                for (int i = 0; i < colls.Length; i++)
                {
                    if (colls[i].gameObject.tag == "Player")
                    {
                        _optionPanel.SetActive(true);
                        _inside = true;
                    }
                }
                if (_inside == false && _optionPanel.activeInHierarchy == true)
                {
                    _optionPanel.SetActive(false);
                }
                yield return new WaitForEndOfFrame();
            } while (true);
        }

        public virtual void DoTrigger()
        {
            StatsManager.Instance.RestoreHP();
            GameManager.Instance.SwapContextToDungeonFromWorld();
        }

        public void ClosePanel()
        {
            _optionPanel.SetActive(false);
        }

        public void OnEnable()
        {
            StartCoroutine(DetectCollisionCorutine());   
        }

    }
}

