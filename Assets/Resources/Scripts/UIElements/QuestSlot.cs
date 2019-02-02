using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LimoKnight
{
     
    public class QuestSlot : MonoBehaviour
    {
        [SerializeField] private Text _textName;
        [SerializeField] private Text _textRewards;
        [SerializeField] private Text _textStatus;
        private string _questDescription;
        private int ID; //same as the mission, same as his index on QuestPanel.allquests
        private QuestPanel questPanel;

        public void Start()
        {
            questPanel = QuestManager.Instance.QuestPanel;
        }

        public string QuestDescription
        {
            get { return _questDescription; }
            set { _questDescription = value; }
        }

        public void SetQuestValues(Quest quest)
        {
            ID = quest.ID;
            _textName.text = quest.Name;
            _questDescription = quest.Description;
            _textRewards.text = quest.Rewards[0].ToString()+"G | "+quest.Rewards[1].ToString()+"EXP";
            _textStatus.text = ObtainStatus(quest);
            if (_textStatus.text == "") this.gameObject.SetActive(false);
            else this.gameObject.SetActive(true);
        }

        private string ObtainStatus(Quest quest)
        {
            if (quest.CompletedByUser) return "<b><color=#006400>C O M P L E T E D</color></b>";
            else if (quest.IsActiveNow) return "<b><color=#FF8C00>A C T I V E</color></b>";
            else if (quest.IsAvailableNow) return "<b><color=#008B8B>A V A I L A B L E</color></b>";
            else return "";
        }

        public void UpdateStatus(Quest quest)
        {
            _textStatus.text = ObtainStatus(quest);
            if (_textStatus.text == "") this.gameObject.SetActive(false);
            else this.gameObject.SetActive(true);
        }

        public void InformQuestPanelShowQuestDescription()
        {
            questPanel.ShowMissionDescription(_questDescription, ID);
        }


    }
}
