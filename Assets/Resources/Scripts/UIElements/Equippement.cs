using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LimoKnight {
    public class Equippement : MonoBehaviour {

        [SerializeField] Image _weaponImage;
        [SerializeField] Image _helmetImage;
        [SerializeField] Image _chestplateImage;
        [SerializeField] Image _glovesImage;
        [SerializeField] Image _trousersImage;
        [SerializeField] Image _shoesImage;
        [SerializeField] Image _leftAccesoryImage;
        [SerializeField] Image _rightAccesoryImage;

        [SerializeField] EquippementData _weaponData;
        [SerializeField] EquippementData _helmetData;
        [SerializeField] EquippementData _chestplateData;
        [SerializeField] EquippementData _glovesData;
        [SerializeField] EquippementData _trousersData;
        [SerializeField] EquippementData _shoesData;
        [SerializeField] EquippementData _leftAccesoryData;
        [SerializeField] EquippementData _rightAccesoryData;

        private bool wasInitialized = false;

        private void OnEnable()
        {
            if (!wasInitialized)
            {
                InfoArmorDatabase iad = InventoryManager.Instance.GetAllArmorsInInventory();
                InfoWeaponsDatabase iwd = InventoryManager.Instance.GetAllWeaponsInInventory();
                foreach (InfoArmor ia in iad.Database)
                {
                    if (ia.IsEquipped)
                        UpdateArmorEquipped(ia);
                }
                foreach (InfoWeapon iw in iwd.DataBase)
                {
                    if (iw.IsEquipped)
                        UpdateWeaponEquipped(iw);
                }
                wasInitialized = true;
            }
        }

        public void UpdateArmorEquipped(InfoArmor ia)
        {
            Sprite sprite = Resources.Load(ControlManager.Instance.PathToInventorySprites + ia.ModelName, typeof(Sprite)) as Sprite;
            switch (ia.EquippedInSlot)
            {
                case (EquippedInSlot.Chestplate):
                    {
                        _chestplateData.SetValuesArmor(ia.Name, ia.Description, ia.Stats);
                        _chestplateImage.sprite = sprite;
                        break;
                    }
                case (EquippedInSlot.Gloves):
                    {
                        _glovesData.SetValuesArmor(ia.Name, ia.Description, ia.Stats);
                        _glovesImage.sprite = sprite;
                        break;
                    }
                case (EquippedInSlot.Helmet):
                    {
                        _helmetData.SetValuesArmor(ia.Name, ia.Description, ia.Stats);
                        _helmetImage.sprite = sprite;
                        break;
                    }
                case (EquippedInSlot.LeftAccesory):
                    {
                        _leftAccesoryData.SetValuesArmor(ia.Name, ia.Description, ia.Stats);
                        _leftAccesoryImage.sprite = sprite;
                        break;
                    }
                case (EquippedInSlot.RightAccesory):
                    { 
                        _rightAccesoryData.SetValuesArmor(ia.Name, ia.Description, ia.Stats);
                        _rightAccesoryImage.sprite = sprite;
                        break;
                    }
                case (EquippedInSlot.Shoes):
                    {
                        _shoesData.SetValuesArmor(ia.Name, ia.Description, ia.Stats);
                        _shoesImage.sprite = sprite;
                        break;
                    }
                case (EquippedInSlot.Trousers):
                    {
                        _trousersData.SetValuesArmor(ia.Name, ia.Description, ia.Stats);
                        _trousersImage.sprite = sprite;
                        break;
                    }
            }
        }

        public void UpdateWeaponEquipped(InfoWeapon iw)
        {
            _weaponImage.sprite = Resources.Load(ControlManager.Instance.PathToInventorySprites + iw.ModelName, typeof(Sprite)) as Sprite;
            _weaponData.SetValuesWeapon(iw.Name, "Rarity Value: " + iw.WeaponRarity.ToString(), Mathf.RoundToInt(iw.Damage));
        }
    }
}
