using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LimoKnight
{

    /// <summary>
    /// This NPC will show a variable of dialogues prior to open the inventory in any specific way.
    /// </summary>
    public class NPCTalkAndMenu : NPCTalk
    {
        [Tooltip("In what mode will the inventory be opened?")]
        [SerializeField]
        private ContextItems _inventoryInMode;
        [SerializeField] private GameObject _sellButton;
        private int _lastItemsSelected = 0;
        private Text text;

        private bool _isWaittingToDialogue = false;

        protected override void Update()
        {
            base.Update();
            if (!_isWaittingToDialogue)
                StartCoroutine(DetectEndDialogue());
        }

        private IEnumerator DetectEndDialogue()
        {
            _isWaittingToDialogue = true;
            yield return new WaitUntil(() => this._inside && DialogueManager.Instance.IsDisplayingDialogue == true);
            yield return new WaitUntil(() => DialogueManager.Instance.IsDisplayingDialogue == false);
            GameManager.Instance.ShouldPlayerMove(false);
            UIManager.Instance.ShowUIMainPanel(UIMainPanel.Inventory);
            InventoryManager.Instance.ContextItem = _inventoryInMode;
            if (_inventoryInMode == ContextItems.SellItems)
            {
                _sellButton.SetActive(true);
                StartCoroutine(ProcessItemSelectedCorutine());
            }
            yield return new WaitUntil(() => InventoryManager.Instance.ContextItem == ContextItems.None);
            _lastItemsSelected = 0;
            if (_inventoryInMode == ContextItems.SellItems)
                _sellButton.SetActive(false);
            _isWaittingToDialogue = false;
        }

        protected override void CompleteQuest()
        {
            base.CompleteQuest();
        }

        private IEnumerator ProcessItemSelectedCorutine()
        {
            do
            {
                if (text == null) text = _sellButton.transform.GetChild(0).GetComponent<Text>();
                text.text = _lastItemsSelected + " items ready to be sold.\n Press me to sell!";
                yield return new WaitUntil(() => InventoryManager.Instance.NumberOfItemsInContext != _lastItemsSelected && InventoryManager.Instance.ContextItem == ContextItems.SellItems);
                _lastItemsSelected = InventoryManager.Instance.NumberOfItemsInContext;
            } while (InventoryManager.Instance.ContextItem != ContextItems.None);
            
        }

        public void SellItems()
        {
            if (InventoryManager.Instance.ContextItem == ContextItems.SellItems)
                InventoryManager.Instance.SellItemsSelected();
        }
    }
}