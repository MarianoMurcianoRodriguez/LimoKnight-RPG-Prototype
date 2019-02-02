using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight {
    //This class, the same as the inventory slot should be able to pop up a panel saying its atributes,
    //its description...but considering the time, at least we will place the skeleton for that either way
    public class EquippementData : MonoBehaviour {
        private string NameItem = null;
        private string DescriptionItem = null;
        private int DamageWeapon = -1;
        private Stats StatsItem = null;

        public void SetValuesSimpleItem(string nameItem, string descriptionItem)
        {
            ResetValues();
            this.NameItem = nameItem;
            this.DescriptionItem = descriptionItem;
        }

        public void SetValuesArmor(string nameItem, string descriptionItem, Stats stats)
        {
            ResetValues();
            this.NameItem = nameItem;
            this.DescriptionItem = descriptionItem;
            this.StatsItem = stats;
        }

        public void SetValuesWeapon(string nameItem, string descriptionItem, int damageWeapon)
        {
            ResetValues();
            this.NameItem = nameItem;
            this.DescriptionItem = descriptionItem;
            this.DamageWeapon = damageWeapon;
        }

        private void ResetValues()
        {
            this.NameItem = null;
            this.DescriptionItem = null;
            this.DamageWeapon = -1;
            this.StatsItem = null;
        }
    }
}
