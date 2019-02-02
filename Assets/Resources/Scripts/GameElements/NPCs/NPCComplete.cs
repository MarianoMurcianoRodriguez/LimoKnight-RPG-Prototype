using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    /// <summary>
    /// Same as the NPCTalkAndMenu but he request a item in which case when completed it's done. 
    /// </summary>
    public class NPCComplete : NPCTalkAndMenu
    {
        [Tooltip("What item is he waitting for?")]
        [SerializeField]
        private string _nameItemAsked;
        private bool _isWaittingForItem = true;
        private bool _itemWasGiven = false;


        protected override void Update()
        {
            base.Update();
            if (_isWaittingForItem && this._inside)
            {
                if (UIManager.Instance.isActiveMainPanel(UIMainPanel.Inventory))
                {
                    if (!_itemWasGiven)
                    {
                        string nameItem = InventoryManager.Instance.ItemToTakeName;
                        if (nameItem != null && nameItem == _nameItemAsked)
                        {
                           bool taked = InventoryManager.Instance.RemoveImportantItem(nameItem);
                            _itemWasGiven = taked;
                            if (_itemWasGiven)
                            {
                                _isWaittingForItem = false;
                                base.CompleteQuest();
                                UIManager.Instance.HideUIMainPanel(UIMainPanel.Inventory);
                                GameManager.Instance.ShouldPlayerMove(true);
                                InventoryManager.Instance.ContextItem = ContextItems.None;
                            }
                        }
                    }
                }
            }
        }
    }
}
