using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public class InventoryPanel : MonoBehaviour
    {
        /// <summary> Holder of all inventoryslots</summary>
        [SerializeField] private GameObject _contentSlots;
        /// <summary> Contains InventorySlotInfo, initially empty and it gets filled</summary>
        private List<InventorySlotInfo> _infoItemsInInventory = new List<InventorySlotInfo>();
        /// <summary> List of All Slots in the Inventory, so it contains initally all slots this will reflect the actual order of the slots in the inventory</summary>
        private List<GameObject> _slotsInventory = new List<GameObject>();
        /// <summary> For each slot in the inventory, his inventorySlotInfo (to _infoItemsInInventory)  this will reflect the actual order of the slots in the inventory</summary>
        private List<InventorySlotInfo> _slotsInventoryInfo = new List<InventorySlotInfo>();
        
        
        private int _itemsInInventory = 0;
        private InventorySlotType _inventorySlotShow;

        public InventorySlotType InventorySlotShow
        {
            get
            {
                return _inventorySlotShow;
            }
            set
            {
                if (value != InventorySlotShow)
                {
                    _inventorySlotShow = value;
                    RefreshUISlots();
                }
            }
        }


        /// <summary>
        /// This method will load the inventory panel, this means:
        /// -Create many slots as the maximum and adding them to a list, and do the same with 
        ///     each inventoryslotinfo of each slot
        /// </summary>
        public void Initialize()
        {
            _inventorySlotShow = InventorySlotType.All;
            int numSlots = InventoryManager.Instance.MaxItems;
            for (int i=1; i<=numSlots; i++)
            {
                GameObject go = _contentSlots.transform.GetChild(i-1).gameObject;
                go.name = "Slot-" + i.ToString();
                go.SetActive(false);
                go.transform.SetParent(_contentSlots.transform);
                _slotsInventory.Add(go);
                _slotsInventoryInfo.Add(go.GetComponentInChildren<InventorySlotInfo>());
            }
        }

        /// <summary>
        /// This method will always add a slot. InventoryManager will look up for this. And
        /// if this item is the same type that the one's we're showing up we should refresh the slots
        /// </summary>
        public void AddToInventory(InventorySlotType slotType, string imagePath, int index) {
            
            InventorySlotInfo newSlot = _slotsInventoryInfo[_itemsInInventory];
            _itemsInInventory++;
            newSlot.SetValue(imagePath, slotType, index, _itemsInInventory);
            _infoItemsInInventory.Add(newSlot);
            if (newSlot.InventorySlotType==_inventorySlotShow||_inventorySlotShow==InventorySlotType.All)
                UpdateUISlots();

        }

        public void AddToInventory(InventorySlotType slotType, EquippedInSlot inSlot, string imagePath, int index)
        {
            InventorySlotInfo newSlot = _slotsInventoryInfo[_itemsInInventory];
            _itemsInInventory++;
            newSlot.SetValue(imagePath, slotType, inSlot, index, _itemsInInventory);
            _infoItemsInInventory.Add(newSlot);
            if (newSlot.InventorySlotType == _inventorySlotShow || _inventorySlotShow == InventorySlotType.All)
                UpdateUISlots();
        }

        /// <summary>
        /// To remove a item we will pass the index of that slots, and with that we can obtain
        /// the inventoryslotinfo and the inventoryslot asociated with that, but we will have
        /// to find the inventoryslotinfo in _infoItemsInventory
        /// </summary>
        public void RemoveFromInventory(int index)
        {
            //We shall find those InventorySlotInfo with bad indexerSlot (greater than index) and reducing by 1
            //this is because the slots are not conscious of in what order they're stored
            for (int i = index; i < _itemsInInventory; i++)
            {
                if (_slotsInventoryInfo[i].IndexSlot > index)
                {
                    _slotsInventoryInfo[i].IndexSlot--;
                }
                if (_infoItemsInInventory[i].IndexSlot > index)
                    _infoItemsInInventory[i].IndexSlot--;
            }
            _itemsInInventory--;
            //Removing the item from the items that we have in inventory
            InventorySlotInfo isiToRemove = _slotsInventoryInfo[index];
            _infoItemsInInventory.Remove(isiToRemove);
            //Updating the list of slots and moving the removed to the last
            GameObject slotToLast = _slotsInventory[index];
            _slotsInventory[index].SetActive(false);
            slotToLast.transform.SetAsLastSibling();
            _slotsInventory.Add(slotToLast);
            _slotsInventory.RemoveAt(index);
            //Same with the slotinfo, we take the one which was going to be last, reseting the value, adding it and removing the old one
            InventorySlotInfo slotInfoToLast = slotToLast.GetComponentInChildren<InventorySlotInfo>();
            slotInfoToLast.ResetValues();
            _slotsInventoryInfo.Add(slotInfoToLast);
            _slotsInventoryInfo.RemoveAt(index);
            //We need to update the IndexSlot of those which were greater than index
            
        }

        /// <summary>
        /// Refresh mean load everything because we want to swap context or load the whole inventory
        /// because more than one item would change 
        /// </summary>
        private void RefreshUISlots()
        {
            //disable all items
            int allItems = _infoItemsInInventory.Count;
            for (int i = allItems; i <= _slotsInventory.Count; i++)
            {
                _slotsInventory[i].SetActive(false);
            }

            if (_inventorySlotShow == InventorySlotType.All)
            {
              
                for (int i=0; i<_infoItemsInInventory.Count; i++)
                {
                    _slotsInventory[i].SetActive(true);
                    _slotsInventoryInfo[i].SetValue(_infoItemsInInventory[i].ImagePath, _infoItemsInInventory[i].InventorySlotType, _infoItemsInInventory[i].EquippedInSlot, _infoItemsInInventory[i].Index, i);
                }
            } else
            {
                int aux = -1;
                //enable the right slot-type slots and fillem with the right content.
                for (int i = 0; i < _infoItemsInInventory.Count; i++)
                {
                    if (_infoItemsInInventory[i].InventorySlotType == _inventorySlotShow) aux++;
                    _slotsInventory[aux].SetActive(true);
                    _slotsInventoryInfo[aux].SetValue(_infoItemsInInventory[i].ImagePath, _infoItemsInInventory[i].InventorySlotType, _infoItemsInInventory[i].EquippedInSlot, _infoItemsInInventory[i].Index,i);
                }
            }
        }

        /// <summary>
        /// This Method will update an item which must be seen in the inventory right know. 
        /// </summary>
        private void UpdateUISlots()
        {
            int index = -1;
            //we will search for how many slots with the right type there are
            foreach (InventorySlotInfo isi in _infoItemsInInventory)
            {
                if (isi.InventorySlotType == _inventorySlotShow || _inventorySlotShow == InventorySlotType.All) index++;
            }
            //this means all slots but this last one are inactive and this one needs the data
            _slotsInventoryInfo[index].SetValue(_infoItemsInInventory[index].ImagePath, _infoItemsInInventory[index].InventorySlotType, _infoItemsInInventory[index].EquippedInSlot, _infoItemsInInventory[index].Index, index);
            _slotsInventory[index].SetActive(true);
        }

       
       /* private void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (_itemsInInventory < InventoryManager.Instance.MaxItems())
                {
                    /*_slotsInventory[_itemsInInventory].SetActive(true);
                    _slotsInventoryInfo[_itemsInInventory].SetValue("/Inventory1", InventorySlotType.All, _itemsInInventory);
                    Debug.Log(_slotsInventory[_itemsInInventory].name + " - " + _slotsInventoryInfo[_itemsInInventory].ImagePath
                        + " - " + _slotsInventoryInfo[_itemsInInventory].InventorySlotType + " - " + _slotsInventoryInfo[_itemsInInventory].Index);
                    AddToInventory(_slotsInventoryInfo[_itemsInInventory]);
                    AddToInventory(InventorySlotType.All,"/Inventory1", _itemsInInventory);

                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (_itemsInInventory < InventoryManager.Instance.MaxItems())
                {
                    /*_slotsInventory[_itemsInInventory].SetActive(true);
                   _slotsInventoryInfo[_itemsInInventory].SetValue("/Inventory1", InventorySlotType.All, _itemsInInventory);
                   Debug.Log(_slotsInventory[_itemsInInventory].name + " - " + _slotsInventoryInfo[_itemsInInventory].ImagePath
                       + " - " + _slotsInventoryInfo[_itemsInInventory].InventorySlotType + " - " + _slotsInventoryInfo[_itemsInInventory].Index);
                   AddToInventory(_slotsInventoryInfo[_itemsInInventory]);
                    AddToInventory(InventorySlotType.All, "/Inventory1", _itemsInInventory);

                }
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                if (_itemsInInventory> 0)
                {
                    int random = UnityEngine.Random.Range(0, _itemsInInventory);
                    Debug.Log("REMOVE ITEM: " + _slotsInventory[random].name);
                    RemoveFromInventory(random);
                }
            }
        }*/
    }
}

