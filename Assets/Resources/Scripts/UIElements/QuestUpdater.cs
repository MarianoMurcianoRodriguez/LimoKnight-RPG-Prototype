using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LimoKnight
{
    [RequireComponent(typeof(Animator))]
    public class QuestUpdater : MonoBehaviour
    {
        [SerializeField] private Text _nameMission;
        [SerializeField] private Text _objective;
        private bool _endedAnimation = true;
        private Animator _animator;

        public bool EndedAnimation
        {
            get { return _endedAnimation; }
            set { _endedAnimation = value; }
        }

        private void Awake()
        {
            _animator = this.gameObject.GetComponent<Animator>();
        }

        public void DisplayMissionUpdate(string nameMission, string objectiveText)
        {
            _nameMission.text = nameMission;
            _objective.text = objectiveText;
            _animator.Play("ShowMission");
        }

        public void StartAnimation()
        {
            _endedAnimation = false;
        }

        public void FinishAnimation()
        {
            _endedAnimation = true;
            UIManager.Instance.HideUISecondaryPanel(UISecondaryPanel.UpdateMission);
        }
    }

}
