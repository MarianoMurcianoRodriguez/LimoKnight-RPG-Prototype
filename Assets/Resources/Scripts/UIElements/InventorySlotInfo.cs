using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LimoKnight
{

    public class InventorySlotInfo : MonoBehaviour
    {

        private string _imagePath; //this image will be loaded from Item/Armor/WeaponInfo and will start with /
        private InventorySlotType _inventorySlotType = InventorySlotType.None;
        private EquippedInSlot _equippedInSlot = EquippedInSlot.None;
        private int _indexSlot = -1; 
        private int _index = -1;      //used to know in the InventoryManager what item is of those posible in the lists
        private Image _itemImage;


        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                if (value != _imagePath) _imagePath = value;
            }
        }

        public InventorySlotType InventorySlotType{
            get
            {
                return _inventorySlotType;
            }
            set
            {
                if (value != _inventorySlotType) _inventorySlotType = value;
            }
        }

        public EquippedInSlot EquippedInSlot
        {
            get { return _equippedInSlot; }
            set { if (value != _equippedInSlot) _equippedInSlot = value; }
        }

        public int Index
        {
            get { return _index; }
            set { if (value != _index) _index = value; }
        }

        public int IndexSlot
        {
            get { return _indexSlot; }
            set { if (value != _indexSlot) _indexSlot = value; }
        }



        public void SetValue(string image, InventorySlotType inventorySlotType, int index, int indexSlot)
        {
            if (_itemImage == null) _itemImage = this.gameObject.GetComponent<Image>();
            this._imagePath = image;
            this._inventorySlotType = inventorySlotType;
            this._index = index;
            this._indexSlot = indexSlot;
            this._equippedInSlot = EquippedInSlot.None;
            UpdateSprite();
        }

        public void SetValue(string image, InventorySlotType inventorySlotType, EquippedInSlot inSlot, int index, int indexSlot)
        {
            if (_itemImage == null) _itemImage = this.gameObject.GetComponent<Image>();
            this._imagePath = image;
            this._inventorySlotType = inventorySlotType;
            this._index = index;
            this._indexSlot = indexSlot;
            this._equippedInSlot = inSlot;
            UpdateSprite();
        }

        public void ResetValues()
        {
            this._imagePath = "";
            this._index = -1;
            this._indexSlot = -1;
            this._inventorySlotType = InventorySlotType.None;
            this._equippedInSlot = EquippedInSlot.None;
        }

        private void UpdateSprite()
        {
            Sprite sprite = Resources.Load(ControlManager.Instance.PathToInventorySprites + _imagePath, typeof(Sprite)) as Sprite;
            _itemImage.sprite = sprite;
        }

        /// <summary>
        /// Depending on the context, this SlotInfo will be processed by anyform, f.e: if player is 'inEquipement' he'll equip the item
        /// so this is the first step
        /// </summary>
        public void SendToInventoryManager()
        {
            InventoryManager.Instance.ProcessSelectedItems(this);
        }
        
    }

    public enum InventorySlotType
    {
        None = -1,
        Item,
        Armor,
        Weapon,
        Important,
        All
    }
}
