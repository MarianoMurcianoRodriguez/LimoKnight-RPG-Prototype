using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LimoKnight
{
    public class OptionMenuHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _mainOptionPanel;
        [SerializeField] private GameObject _optionPanel;
        [SerializeField] private GameObject _keybindingPanel;
        [SerializeField] private ConfigurationValues _configurationValues;
        [SerializeField] private GameObject _optionButton;

        private void OnEnable()
        {
            _mainOptionPanel.SetActive(true);
        }

        private void OnDisable()
        {
            _mainOptionPanel.SetActive(false);
            _optionPanel.SetActive(false);
            _keybindingPanel.SetActive(false);
        }

        public void GoToMainOptionPanel()
        {
           _optionPanel.SetActive(false);
            _keybindingPanel.SetActive(false);
            _mainOptionPanel.SetActive(true);
        }

        public void GoToOptionPanel()
        {
            _mainOptionPanel.SetActive(false);
            _keybindingPanel.SetActive(false);
            ChangeImageMemerButton();
            _optionPanel.SetActive(true);
        }

        public void GoToKeybindingPanel()
        {
            _mainOptionPanel.SetActive(false);
            _optionPanel.SetActive(false);
            _keybindingPanel.SetActive(true);
        }

        public void QuitMenuPause()
        {
            ControlManager.Instance.PlayerContext = InputManager.Instance.PreviousPlayerContextType;
            UIManager.Instance.HideUIMainPanel(UIMainPanel.Pause);
        }

        public void QuitWithoutSave()
        {
            GameManager.Instance.QuitGameWithoutSave();
        }

        public void SwapValuesButton()
        {
            _configurationValues.AreYouAMember = !_configurationValues.AreYouAMember;
            ChangeImageMemerButton();
           
        }

        private void ChangeImageMemerButton()
        {
            if (_configurationValues.AreYouAMember == true)
                _optionButton.GetComponent<Image>().sprite = Resources.Load(ControlManager.Instance.PathToUISprites + "/butonSelected", typeof(Sprite)) as Sprite;
            else
                _optionButton.GetComponent<Image>().sprite = Resources.Load(ControlManager.Instance.PathToUISprites + "/butonNoSelected", typeof(Sprite)) as Sprite;
        }




    }
}

