using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
   

    public enum UIMainPanel
    {
        MainMenu=0,
        Character=1, 
        Inventory=2,
        Pause=3,
        CharacterDialogue=4,
        MissionLog = 5,
        Combat = 6,
        Equippement = 7,
        SlotMachine = 8,
        MenuSize
    }
    public enum UISecondaryPanel
    {
        NewGameOverride = 0,
        UpdateMission = 1,
        ItemDrop = 2,
        KeepFightingDungeon = 3,
        MenuSize
    }


    public class UIManager : Singleton<UIManager>
    {
        /*Each panel MUST be placed in the same order so their childs should be in the same ORDER as the ENUM*/
        [SerializeField] private GameObject _rootUIMainPanel;
        [SerializeField] private GameObject _rootUISecondaryPanel;
        [SerializeField] private Animator _transitionPanelAnimator;
        GameObject[] _UIMainPanels;
        GameObject[] _UISecondaryPanels;
        bool[] _UIMainPanelsState;
        
        public Animator TransitionPanelAnimator
        {
            get { return _transitionPanelAnimator; }
        }
        
        private void Start()
        {
            _UIMainPanels = new GameObject[(int)UIMainPanel.MenuSize];
            _UISecondaryPanels = new GameObject[(int)UISecondaryPanel.MenuSize];
            _UIMainPanelsState = new bool[(int)UIMainPanel.MenuSize];
            for (int i=0; i < _UIMainPanels.Length; i++)
            {
                _UIMainPanels[i] = _rootUIMainPanel.transform.GetChild(i).gameObject;
                _UIMainPanelsState[i] = false;
            }
            _UIMainPanelsState[0] = true;
            for (int i=0; i<_UISecondaryPanels.Length; i++)
            {
                _UISecondaryPanels[i] = _rootUISecondaryPanel.transform.GetChild(i).gameObject;
            }
        }

        /// <summary>
        /// This method will show the MainPanel of the UI, usefull for activate a UIMainPanel for the first
        /// time or to show more than once at a time (character+inventory)
        /// </summary>
        /// <param name="MainPanelShow"></param>
        public void ShowUIMainPanel(UIMainPanel mainPanel)
        {
            _UIMainPanels[(int)mainPanel].SetActive(true);
            _UIMainPanelsState[(int)mainPanel] = true;
        }
        /// <summary>
        ///  For removing a UIMainPanel
        /// </summary>
        /// <param name="MainPanelShow"></param>
        public void HideUIMainPanel(UIMainPanel mainPanel)
        {
            _UIMainPanels[(int)mainPanel].SetActive(false);
            _UIMainPanelsState[(int)mainPanel] = false;
        }
        
        public void ShowUISecondaryPanel(UISecondaryPanel secondaryPanel)
        {
            _UISecondaryPanels[(int)secondaryPanel].SetActive(true);
        }

        public void HideUISecondaryPanel(UISecondaryPanel secondaryPanel)
        {
            _UISecondaryPanels[(int)secondaryPanel].SetActive(false);
        }

        public bool isActiveMainPanel(UIMainPanel panel)
        {
            return _UIMainPanelsState[(int)panel];
        }


        
    }
}
