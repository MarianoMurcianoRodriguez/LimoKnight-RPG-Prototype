using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;					//acceso a XMLSerializer que gestiona los XML
using System.Xml;								//atributos basicos de XML
using System.IO;								//Operacion de entrada y salida para fichero

namespace LimoKnight
{
    /// <summary>
    /// This class will hold the information about an armor/accesory, it'll change the player stats and
    /// because more than one can be equipped in more than one slot it'll have to keep more data relevant
    /// </summary>
    [System.Serializable]
    public class InfoArmor
    {
        public string Name;
        public bool IsEquipped;
        public EquippedInSlot EquippedInSlot;
        public int SellValue;
        public string Description;
        public Stats Stats;
        /// <summary>
        /// This should start with /
        /// </summary>
        public string ModelName;

        public void SetValues(string name, bool isEquiped, EquippedInSlot equippedInSlot, int sellValue, string
            description, Stats stats, string modelName)
        {
            this.Name = name; this.IsEquipped = isEquiped; this.EquippedInSlot = equippedInSlot;
            this.SellValue = sellValue; this.Description = description; this.Stats = stats;
            this.ModelName = modelName;
        }

        public void SwapEquipped()
        {
            this.IsEquipped = !this.IsEquipped;
        }
    }

    [System.Serializable]
    public class InfoArmorDatabase
    {
        [XmlArray("InfoArmors")]
        public List<InfoArmor> Database = new List<InfoArmor>();
    }

    public enum EquippedInSlot
    {
        None = -1,
        Helmet = 0,
        Gloves = 1,
        Chestplate = 2,
        Trousers = 3,
        Shoes = 4,
        LeftAccesory = 5,
        RightAccesory = 6,
        WeaponSlot = 7
    }

    [SerializeField]
    public class StatsItems {
        public int Constitution;
        public int Dexterity;
        public int Strength;
        public int Luck;
   
        public void SetValues(int constitution, int dexterity, int strength, int luck)
        {
            this.Constitution = constitution; this.Dexterity = dexterity; this.Strength = strength;
            this.Luck = luck;
        }
    }
}
