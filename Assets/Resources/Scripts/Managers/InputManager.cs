using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public class KeyBindings
    {
        public KeyCode Inventory = KeyCode.I;
        public KeyCode Equipement = KeyCode.E;
        public KeyCode Journal = KeyCode.J;
        public KeyCode LogMission = KeyCode.L;
        public KeyCode Action = KeyCode.Space;
        public KeyCode PauseMenu = KeyCode.Escape;
    }

    public class InputManager : Singleton<InputManager>
    {
        private KeyBindings _keyBindings = new KeyBindings();
        public KeyBindings KeyBindings { get { return _keyBindings; } }
        private PlayerContextType _previousPlayerContextType;

        public PlayerContextType PreviousPlayerContextType
        {
            get { return _previousPlayerContextType; }
        }

        private void Update()
        {
          if (ControlManager.Instance.GameContext == GameContext.InGame || ControlManager.Instance.PlayerContext == PlayerContextType.InMinigame)
            {
                //the most important is pause menu, others could wait
                if (Input.GetKeyDown(_keyBindings.PauseMenu))
                {
                    if (UIManager.Instance.isActiveMainPanel(UIMainPanel.Pause) == true)
                    {
                        UIManager.Instance.HideUIMainPanel(UIMainPanel.Pause);
                        ControlManager.Instance.PlayerContext = _previousPlayerContextType;
                    }
                    else
                    {
                        UIManager.Instance.ShowUIMainPanel(UIMainPanel.Pause);
                        _previousPlayerContextType = ControlManager.Instance.PlayerContext;
                        ControlManager.Instance.PlayerContext = PlayerContextType.InPauseMenu;
                    }
                    }
                else if (!DialogueManager.Instance.IsDisplayingDialogue && !(ControlManager.Instance.PlayerContext == PlayerContextType.InPauseMenu))
                {
                    if (Input.GetKeyDown(_keyBindings.Inventory))
                    {
                        //SHOW INVENTORY
                        if (UIManager.Instance.isActiveMainPanel(UIMainPanel.Inventory) == true)
                        {
                            GameManager.Instance.ShouldPlayerMove(true);
                            UIManager.Instance.HideUIMainPanel(UIMainPanel.Inventory);
                            if (UIManager.Instance.isActiveMainPanel(UIMainPanel.Equippement))
                                UIManager.Instance.HideUIMainPanel(UIMainPanel.Equippement);
                            InventoryManager.Instance.ContextItem = ContextItems.None;
                        }
                        else
                        {
                            GameManager.Instance.ShouldPlayerMove(false);
                            InventoryManager.Instance.ContextItem = ContextItems.RemoveItems;
                            UIManager.Instance.ShowUIMainPanel(UIMainPanel.Inventory);
                        }
                    }
                    if (Input.GetKeyDown(_keyBindings.Equipement))
                    {
                        if (UIManager.Instance.isActiveMainPanel(UIMainPanel.Equippement) == true)
                        {
                            GameManager.Instance.ShouldPlayerMove(true);
                            UIManager.Instance.HideUIMainPanel(UIMainPanel.Inventory);
                            UIManager.Instance.HideUIMainPanel(UIMainPanel.Equippement);
                            InventoryManager.Instance.ContextItem = ContextItems.None;
                        }
                        else
                        {
                            GameManager.Instance.ShouldPlayerMove(false);
                            InventoryManager.Instance.ContextItem = ContextItems.EquippeItems;
                            if (!UIManager.Instance.isActiveMainPanel(UIMainPanel.Inventory))
                                UIManager.Instance.ShowUIMainPanel(UIMainPanel.Inventory);
                            UIManager.Instance.ShowUIMainPanel(UIMainPanel.Equippement);
                        }

                    }
                    if (Input.GetKeyDown(_keyBindings.LogMission)) {
                        if (UIManager.Instance.isActiveMainPanel(UIMainPanel.MissionLog))
                        {
                            GameManager.Instance.ShouldPlayerMove(true);
                            UIManager.Instance.HideUIMainPanel(UIMainPanel.MissionLog);
                        }
                        else
                        {
                            GameManager.Instance.ShouldPlayerMove(false);
                            UIManager.Instance.ShowUIMainPanel(UIMainPanel.MissionLog);
                        }
                    }
                }
          }
        }

    }
}

