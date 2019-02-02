using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LimoKnight
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] protected bool _shouldMove = false;
        [SerializeField] protected float _sizeActionSphere = 1f;
        [SerializeField] protected bool _procsQuestCompletition = true;
        [SerializeField] private int _completitionQuestID;
        [SerializeField] private int _completitionSubObjectiveID;
        [SerializeField] private bool _shouldCompleteOnFinding = false;
        private bool _isNotCompleted;
        private bool _wasDetected = false;
        protected bool _inside = false;

        protected IEnumerator Walk()
        {
            yield return null;
        }

         void Start()
        {
            _isNotCompleted = QuestManager.Instance.IsQuestNotCompleted(_completitionQuestID, _completitionSubObjectiveID);
            _wasDetected = _isNotCompleted;
            if (_shouldMove) StartCoroutine(Walk());
            StartCoroutine(PlayerCloseTo());
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gameObject.transform.position, _sizeActionSphere);
        }
        private IEnumerator PlayerCloseTo()
        {
            GameObject panel = this.transform.GetChild(0).gameObject;
            Collider[] colls;
            do
            {
                _inside = false;
                if (_wasDetected == false)      //if the mission was not detected once we must keep trying each frame
                {
                    _isNotCompleted = QuestManager.Instance.IsQuestNotCompleted(_completitionQuestID, _completitionSubObjectiveID);
                    _wasDetected = _isNotCompleted;
                }
                colls = Physics.OverlapSphere(gameObject.transform.position, _sizeActionSphere);
                for (int i = 0; i < colls.Length; i++)
                {
                    if (colls[i].gameObject.tag == "Player")
                    {
                        if (_isNotCompleted == true && _shouldCompleteOnFinding && _procsQuestCompletition)
                        {
                            QuestManager.Instance.InformCompletitionQuest(_completitionQuestID, _completitionSubObjectiveID);
                            _isNotCompleted = false;
                        }
                        panel.SetActive(true);
                        _inside = true;
                    }
                }
                if ((panel.activeInHierarchy) && (_inside == false))
                    panel.SetActive(false);
                
                yield return new WaitForEndOfFrame();
            } while (true);
        }

        protected virtual void CompleteQuest()
        {
            if (_shouldCompleteOnFinding == false && _procsQuestCompletition) {
                QuestManager.Instance.InformCompletitionQuest(_completitionQuestID, _completitionSubObjectiveID);

            }
        }
        

    }
}
