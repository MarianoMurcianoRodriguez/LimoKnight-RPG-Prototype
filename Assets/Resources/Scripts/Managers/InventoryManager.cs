using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LimoKnight
{

    public enum ContextItems
    {
        None = -1,
        RemoveItems = 0,
        SellItems = 1, 
        TakeItems = 2, 
        EquippeItems = 3,
    }

    /// <summary>
    /// This class will hold all the Inventory of the player, including which ones are actually equipped
    /// </summary>
    public class InventoryManager : Singleton<InventoryManager>
    {

        [SerializeField] private GameObject InventoryPanelUI;
        [SerializeField] private Equippement Equippement;
        private int _maxItemsInPossesion = 40;
        private int _maxArmorsInPossesion = 30;
        private int _maxWeaponInPossesion = 15;
        private int _actualItems = 0;
        //helmet+gloves+chestplate+trousers+shoes+2 accesories
        private int _maxArmorEquiped = 7;
        private Dictionary<int, InfoArmor> _armorEquipped = new Dictionary<int, InfoArmor>();
        private Dictionary<int, InfoArmor> _armorsInInventory = new Dictionary<int, InfoArmor>();
        private Dictionary<int, InfoWeapon> _weaponsInInventory = new Dictionary<int, InfoWeapon>();
        private Dictionary<int, InfoItem> _itemsInInventory = new Dictionary<int, InfoItem>();

        //Auxiliar List to keep track of what items have been deleted and what indexs for the dictionarys are free
        private List<int> _freeIntArmors = new List<int>();
        private List<int> _freeIntWeapons = new List<int>();
        private List<int> _freeIntItems = new List<int>();

        private InventoryPanel _inventoryPanel;

        private InfoWeapon _weaponEquipped;
        private Weapon _weaponPlayer;
        private Stats _statsEquipped;
        //Variables for What should do double click on a slot
        private ContextItems _contextItems;
        private List<InventorySlotInfo> _itemsInContext = new List<InventorySlotInfo>();

        private string _itemToTakeName;
        private int _itemToTakeSlotIndex;
        private InventorySlotInfo _slotItemSelected;

        public string ItemToTakeName
        {
            get { return _itemToTakeName; }
        }

        public int NumberOfItemsInContext
        {
            get { return _itemsInContext.Count; }
        }

        public ContextItems ContextItem
        {
            get { return _contextItems; }
            set
            {
                if (_contextItems != value) _contextItems = value;
                _itemsInContext.Clear();
            }
        }

        //used to apply extra effects on the player besides the weapon damage
        //wich will NOT be applied.
        public Stats StatsEquipped
        {
            get { return _statsEquipped; }
            set
            {
                if (value != _statsEquipped && value != null)
                    _statsEquipped = value;
                else if (value == null) _statsEquipped = new Stats();
                StatsManager.Instance.RecalculateStatsInventory(_statsEquipped);
            }
        }

        public int AvailableItemSpace
        {
            get { return (_maxItemsInPossesion - _itemsInInventory.Count); }
        }
        public int AvailableArmorSpace
        {
            get { return (_maxArmorsInPossesion - _armorsInInventory.Count); }
        }
        public int AvailableWeaponSpace
        {
            get { return (_maxWeaponInPossesion - _itemsInInventory.Count); }
        }


        /********************************METHODS OF CLASS****************************************/

        public InfoItemsDatabase GetAllItemsInInventory()
        {
            InfoItem[] items = new InfoItem[_itemsInInventory.Count];
            _itemsInInventory.Values.CopyTo(items, 0);
            InfoItemsDatabase iid = new InfoItemsDatabase();
            foreach (InfoItem ii in items)
                iid.DataBase.Add(ii);
            return iid;
        }

        public InfoArmorDatabase GetAllArmorsInInventory()
        {
            InfoArmor[] armors = new InfoArmor[_armorsInInventory.Count];
            _armorsInInventory.Values.CopyTo(armors, 0);
            InfoArmorDatabase iad = new InfoArmorDatabase();
            foreach (InfoArmor ia in armors)
                iad.Database.Add(ia);
            return iad;
        }

        public InfoWeaponsDatabase GetAllWeaponsInInventory()
        {
            InfoWeapon[] weapons = new InfoWeapon[_weaponsInInventory.Count];
            _weaponsInInventory.Values.CopyTo(weapons, 0);
            InfoWeaponsDatabase iwd = new InfoWeaponsDatabase();
            foreach (InfoWeapon iw in weapons)
                iwd.DataBase.Add(iw);
            return iwd;
        }

        private void Start()
        {
            _weaponPlayer = ControlManager.Instance.Player.GetComponent<Weapon>();
            _inventoryPanel = InventoryPanelUI.GetComponent<InventoryPanel>();
            _inventoryPanel.Initialize();
        }

        public void SetEquippedWeapon(InfoWeapon iw)
        {
            if (iw == _weaponEquipped) return;
            _weaponEquipped = iw;
            _weaponPlayer.RefreshWeapon(_weaponEquipped);
        }
        public InfoWeapon GetEquippedWeapon() { return _weaponEquipped; }

        public void SetInventoryWeapons(List<InfoWeapon> infoWeapon)
        {
            for (int i = 0; i < infoWeapon.Count; i++)
            {
                _weaponsInInventory.Add(i, infoWeapon[i]);
                if (infoWeapon[i].IsEquipped == true)
                    SetEquippedWeapon(infoWeapon[i]);
                _inventoryPanel.AddToInventory(InventorySlotType.Weapon, infoWeapon[i].ModelName, i);
            }
            Stats statsAhora = this.StatsEquipped;
            _actualItems += infoWeapon.Count;

        }

        public void SetInventoryArmor(List<InfoArmor> infoArmor)
        {
            Stats statsEquippement = new Stats();
            for (int i = 0; i < infoArmor.Count; i++)
            {
                _armorsInInventory.Add(i, infoArmor[i]);
                if (infoArmor[i].IsEquipped)
                {
                    statsEquippement.Luck += infoArmor[i].Stats.Luck;
                    statsEquippement.Constitution += infoArmor[i].Stats.Constitution;
                    statsEquippement.Defense += infoArmor[i].Stats.Defense;
                    statsEquippement.Strength += infoArmor[i].Stats.Strength;
                    _armorEquipped.Add((int)infoArmor[i].EquippedInSlot, infoArmor[i]);
                }
                _inventoryPanel.AddToInventory(InventorySlotType.Armor, infoArmor[i].EquippedInSlot, infoArmor[i].ModelName, i);
            }
            this.StatsEquipped = statsEquippement;
            _actualItems += infoArmor.Count;
        }

        public void SetInventoryItems(List<InfoItem> infoItem)
        {
            for (int i = 0; i < infoItem.Count; i++)
            {
                _itemsInInventory.Add(i, infoItem[i]);
                if (infoItem[i].TypeItem == TypeItem.Important) _inventoryPanel.AddToInventory(InventorySlotType.Important, infoItem[i].ModelName, i);
                else _inventoryPanel.AddToInventory(InventorySlotType.Item, infoItem[i].ModelName, i);
            }
            _actualItems += infoItem.Count;
        }

        //to add something we must find the index availables for such item (if there are only
        //30 items and we removed 3, we cannot go to the 31 after deleting because we are
        //limiting that dictionary to a fixed size
        public bool AddToInventory(InfoItem i)
        {
            if (_itemsInInventory.Count >= _maxItemsInPossesion) return false;
            int index = 0;
            if (_freeIntItems.Count > 0)
            {
                index = _freeIntItems[0];
                _freeIntItems.RemoveAt(0);
            }
            else
                index = _itemsInInventory.Count;
            if (i.TypeItem == TypeItem.Important) _inventoryPanel.AddToInventory(InventorySlotType.Important, i.ModelName, index);
            else _inventoryPanel.AddToInventory(InventorySlotType.Item, i.ModelName, index);
            _itemsInInventory.Add(index, i);
            return true;
        }

        public bool AddToInventory(InfoWeapon w)
        {
            if (_weaponsInInventory.Count >= _maxWeaponInPossesion) return false;
            int index = 0;
            if (_freeIntWeapons.Count > 0)
            {
                index = _freeIntWeapons[0];
                _freeIntWeapons.RemoveAt(0);
            }
            else
                index = _weaponsInInventory.Count;
            _inventoryPanel.AddToInventory(InventorySlotType.Weapon, EquippedInSlot.WeaponSlot, w.ModelName, index);
            _weaponsInInventory.Add(index, w);
            return true;
        }

        public bool AddToInventory(InfoArmor a)
        {
            if (_armorsInInventory.Count >= _maxArmorsInPossesion) return false;
            int index = 0;
            if (_freeIntArmors.Count > 0)
            {
                index = _freeIntArmors[0];
                _freeIntArmors.RemoveAt(0);
            }
            else
                index = _armorsInInventory.Count;
            _inventoryPanel.AddToInventory(InventorySlotType.Armor, a.ModelName, index);
            _armorsInInventory.Add(index, a);
            return true;
        }

        public int MaxItems
        {
            get { return _maxItemsInPossesion + _maxWeaponInPossesion + _maxArmorsInPossesion; }
        }

        public void ProcessSelectedItems(InventorySlotInfo isi)
        {
            if (!_itemsInContext.Contains(isi))
            {
                _itemsInContext.Add(isi);
                return;
            }
            switch (_contextItems)
            {
                case ContextItems.RemoveItems:
                    {
                        RemoveItem(isi);
                        break;
                    }
                case ContextItems.TakeItems:
                    {
                        TakeImportantItem(isi);
                        break;
                    }
                case ContextItems.SellItems:
                    {
                        if (isi.InventorySlotType == InventorySlotType.Important || isi.EquippedInSlot != EquippedInSlot.None || _weaponsInInventory[isi.Index].IsEquipped == true)
                        {
                            _itemsInContext.Remove(isi);
                        }
                        break;
                    }
                case ContextItems.EquippeItems:
                    {
                        if (isi.InventorySlotType == InventorySlotType.Armor || isi.InventorySlotType == InventorySlotType.Weapon)
                            TryToEquipItem(isi);
                        break;
                    }
                default: break;
            }
        }

        public void RemoveItem(InventorySlotInfo isi)
        {
            if (isi.InventorySlotType == InventorySlotType.Important) return;
            else if (isi.InventorySlotType == InventorySlotType.Armor && isi.EquippedInSlot != EquippedInSlot.None) return;
            else if (isi.InventorySlotType == InventorySlotType.Weapon && isi.Index == 0) return;
            else
            {
                switch (isi.InventorySlotType)
                {
                    case InventorySlotType.Armor:
                        {
                            _armorsInInventory.Remove(isi.Index);
                            _freeIntArmors.Add(isi.Index);
                            _itemsInContext.Remove(isi);
                            _inventoryPanel.RemoveFromInventory(isi.IndexSlot);
                            break;
                        }
                    case InventorySlotType.Weapon:
                        {
                            _weaponsInInventory.Remove(isi.Index);
                            _freeIntWeapons.Add(isi.Index);
                            _itemsInContext.Remove(isi);
                            _inventoryPanel.RemoveFromInventory(isi.IndexSlot);
                            break;
                        }
                    case InventorySlotType.Item:
                        {
                            _itemsInInventory.Remove(isi.Index);
                            _freeIntItems.Add(isi.Index);
                            _itemsInContext.Remove(isi);
                            _inventoryPanel.RemoveFromInventory(isi.IndexSlot);
                            break;
                        }
                }
                _actualItems--;
            }
        }


        public void SellItemsSelected()
        {
            StartCoroutine(SellItemsCorutine());
        }

        private IEnumerator SellItemsCorutine()
        {
            int gold = 0;
            foreach (InventorySlotInfo isi in _itemsInContext)
            {
                switch (isi.InventorySlotType)
                {
                    case InventorySlotType.Armor:
                        {
                            gold = _armorsInInventory[isi.Index].SellValue;
                            break;
                        }
                    case InventorySlotType.Weapon:
                        {
                            gold = _weaponsInInventory[isi.Index].SellValue;
                            break;
                        }
                    case InventorySlotType.Item:
                        {
                            gold = _itemsInInventory[isi.Index].SellValue;
                            break;
                        }
                }
                StatsManager.Instance.AddGold(gold);
                yield return new WaitForSeconds(0.15f);
            }
            while (_itemsInContext.Count > 0)
            {
                RemoveItem(_itemsInContext[0]);
            }
            yield return null;
        }

        //An NPC only can take important items
        public void TakeImportantItem(InventorySlotInfo isi)
        {
            if (isi.InventorySlotType == InventorySlotType.Important)
            {
                _itemToTakeName = _itemsInInventory[isi.Index].NameItem;
                _itemToTakeSlotIndex = isi.Index;
                _slotItemSelected = isi;
            }
        }

        public bool RemoveImportantItem(string name)
        {
            if (_itemToTakeName == null) return false;
            if (name == _itemsInInventory[_itemToTakeSlotIndex].NameItem)
            {
                _itemsInInventory.Remove(_itemToTakeSlotIndex);
                _freeIntItems.Add(_itemToTakeSlotIndex);
                _itemsInContext.Remove(_slotItemSelected);
                _inventoryPanel.RemoveFromInventory(_slotItemSelected.IndexSlot);
                return true;
            }
            return false;
        }

        public bool PlayerHas(string name)
        {
            bool aux = false;
            for (int i = 0; i < _itemsInInventory.Count; i++)
            {
                if (_itemsInInventory[i].NameItem == name)
                {
                    aux = true;
                    break;
                }
            }
            return aux;
        }

        private void TryToEquipItem(InventorySlotInfo isi)
        {
            Debug.Log("El objeto tiene indice: " + isi.Index);
            Debug.Log("De tipo: " + isi.InventorySlotType.ToString());
            if (isi.InventorySlotType == InventorySlotType.Armor && _armorsInInventory[isi.Index].IsEquipped == false)
            {
                Debug.Log("Es un armadura con nombre: "+_armorsInInventory[isi.Index].Name);
                Equippement.UpdateArmorEquipped(_armorsInInventory[isi.Index]);
                ReplaceEquippedArmor(_armorsInInventory[isi.Index], isi.Index);
            }
            else if (isi.InventorySlotType == InventorySlotType.Weapon && _weaponsInInventory[isi.Index].IsEquipped == false)
            {
                Debug.Log("Es un arma con nombre: " + _weaponsInInventory[isi.Index].Name);
                Equippement.UpdateWeaponEquipped(_weaponsInInventory[isi.Index]);
                ReplaceEquippedWeapon(_weaponsInInventory[isi.Index], isi.Index);
            }
        }

        private void ReplaceEquippedArmor(InfoArmor ia, int index)
        {
            Stats actualStats = this.StatsEquipped;
            int indexPrevious = -1;
            for (int i = 0; i < _armorsInInventory.Count; i++)
            {
                if (_armorsInInventory[i].IsEquipped && _armorsInInventory[i].EquippedInSlot == ia.EquippedInSlot)
                {
                    indexPrevious = i;
                    break;
                }
            }
            _armorEquipped[(int)ia.EquippedInSlot] = ia;
            _armorsInInventory[index].SwapEquipped();
            _armorsInInventory[indexPrevious].SwapEquipped();
            actualStats.DecreaseStats(_armorsInInventory[indexPrevious].Stats);
            actualStats.IncreaseStats(ia.Stats);
            this.StatsEquipped = actualStats;
        }

        private void ReplaceEquippedWeapon(InfoWeapon iw, int index)
        {
            Stats actualStats = this.StatsEquipped;
            for (int i = 0; i < _weaponsInInventory.Count; i++)
            {
                if (_weaponsInInventory[i].IsEquipped == true)
                {
                    _weaponsInInventory[i].IsEquipped = false;
                    _weaponsInInventory[index].IsEquipped = true;
                    SetEquippedWeapon(iw);
                }
                Equippement.UpdateWeaponEquipped(iw);
            }
        }
    }
}

