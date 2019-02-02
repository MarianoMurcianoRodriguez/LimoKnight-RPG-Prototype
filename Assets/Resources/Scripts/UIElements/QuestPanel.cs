using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LimoKnight
{
    public class QuestPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _slotQuestPrefab;
        [SerializeField] private GameObject _contentQuestParentHolder;
        [SerializeField] private GameObject _missionInfo;
        [SerializeField] private Text _descriptionMission;
        private QuestSlot[] _questSlots;
        private int _lastSelected = -1;
        
        public void SetAllQuest(Quest[] quests) {
            _questSlots = new QuestSlot[quests.Length];
            for (int i=0; i < quests.Length; i++)
            {
                GameObject slot = Instantiate(_slotQuestPrefab);
                slot.transform.SetParent(_contentQuestParentHolder.transform);
                _questSlots[quests[i].ID] = slot.GetComponent<QuestSlot>();
                _questSlots[quests[i].ID].SetQuestValues(quests[i]);
            }
        }

        public void UpdateStatusQuest(Quest quest)
        {
            _questSlots[quest.ID].gameObject.SetActive(true);
            _questSlots[quest.ID].UpdateStatus(quest);
        }

        public void CloseMissionLog()
        {
            UIManager.Instance.HideUIMainPanel(UIMainPanel.MissionLog);
            GameManager.Instance.ShouldPlayerMove(true);
        }
        
        public void ShowMissionDescription(string description, int ID)
        {
            if (!QuestManager.Instance.GetQuestToAccept(ID).CompletedByUser)
            _descriptionMission.text = description;
            _lastSelected = ID;
            _missionInfo.SetActive(true);
        }

        public void CloseDescriptionClicked()
        {
            _missionInfo.SetActive(false);
        }

        public void AcceptMission()
        {
            if (_lastSelected < 0) return;
            Quest q = QuestManager.Instance.GetQuestToAccept(_lastSelected);
            if (q.IsAvailableNow){
                QuestManager.Instance.SetAvailableQuestAsActive(q.ID);
            }
        }

        private void OnDisable()
        {
            _lastSelected = -1;
        }



    }
}

