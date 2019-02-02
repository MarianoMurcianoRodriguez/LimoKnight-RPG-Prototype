using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.Xml.Serialization;					//acceso a XMLSerializer que gestiona los XML
using System.Xml;								//atributos basicos de XML
using System.IO;								//Operacion de entrada y salida para fichero

namespace LimoKnight
{
    /// <summary>
    /// Container class which works the same as WeaponData but this would be handy in a server-database, this class
    /// will only handle how a Weapon is represented in the XML File then this would be used to give the info to the 
    /// Weapon.cs class
    /// </summary>
   [System.Serializable]
    public class InfoWeapon
    {
        public string Name;
        public float Damage;
        public bool IsEquipped;
        //Will not be used in this type but it can affect the image drop
        public WeaponType WeaponType;
        public WeaponEffect WeaponEffect;
        public WeaponRarity WeaponRarity;
        public float AttackSpeed;
        public int SellValue;
        /// <summary>
        /// This name MUST add a forward-slash / to the modelName
        /// If its '/Weapons/InitialSword' it means the sprite is in that spot 
        /// and that in the '3DModels>Weapon' it's the same too, this is because the item
        /// should link to a prefab and they're 2 things linked to eachother
        /// </summary>
        public string ModelName;

        public void SetValues(string name, float damage, bool isEquipped,  WeaponType weaponType, WeaponEffect weaponEffect,
            WeaponRarity weaponRarity, float attackSpeed, int sellValue, string modelName)
            {Name = name; Damage = damage; IsEquipped = isEquipped; WeaponType = weaponType; WeaponEffect = weaponEffect;
                WeaponRarity = weaponRarity; AttackSpeed = attackSpeed; SellValue = sellValue;
                ModelName = modelName;}
    }

    [System.Serializable]
    public class InfoWeaponsDatabase
    {
        [XmlArray("InfoWeapons")]
        public List<InfoWeapon> DataBase = new List<InfoWeapon>();
    }

    /// <summary>
    /// Any weapon could do: nothing (just hit for it's damage), stun, fire or freeze
    /// </summary>
    public enum TypeEffect
    {
        None = 0,
        Stun = 1,
        Fire = 2,
        Freeze = 3,
    }

    /// <summary>
    /// Each weapon could have an effect and it requires in the worst case: type, probability, time
    /// and the enemy will handle exactly how those works on them
    /// </summary>
    [System.Serializable]
    public class WeaponEffect
    {
        public TypeEffect TypeEffect;
        public float Probability;
        public int Time;

        public void SetValues(TypeEffect typeEffect, float probability, int time)
        {
            this.TypeEffect = typeEffect; this.Probability = probability; this.Time = time;
        }
    }

    public enum WeaponType
    {
        None = -1,
        SimpleSword = 0,
        Dagger = 1,
        Mace = 2,
        Axe = 3,
        LongSword = 4
    }

    public enum WeaponRarity
    {
        None = -1,
        Normal = 0,
        Uncommon = 1,
        Rare = 2,
        Extraordinary = 3
    }
}
