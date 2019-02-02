using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml.Serialization;					//acceso a XMLSerializer que gestiona los XML
using System.Xml;								//atributos basicos de XML
using System.IO;								//Operacion de entrada y salida para fichero

namespace LimoKnight
{
    public enum TypeItem
    {
        None = -1, 
        Consumable = 0,
        Important = 1,
        Other = 2
    }
    public enum ConsumeEffect
    {
        None = -1, 
        Heal = 0,
        Energy = 1
    }
    
    /// <summary>
    /// Clase para la representación de todos cualquier objeto del juego
    /// </summary>
    [System.Serializable]
    public class InfoItem
    {
        public string NameItem;
        public TypeItem TypeItem;
        public string ItemDescription;
        public ConsumeEffect ConsumeEffect;
        public int Quantity;
        public int SellValue;
        public string ModelName;

        /// <summary>
        /// Remember that 'Modelname' should be /TYPEITEM/SUBTYPE(glove, important, accesory)/SPRITENAME
        /// </summary>
        public void SetValues(string nameItem, TypeItem typeItem, string itemDescription, ConsumeEffect consumeEffect, int sellValue, int quantity, string modelName)
        {
            NameItem = nameItem; TypeItem = typeItem; ItemDescription = itemDescription;
            ConsumeEffect = consumeEffect; Quantity = quantity; SellValue = sellValue;
            ModelName = modelName;
        }
    }
    /// <summary>
	/// Info Items database. Esta clase es la que se mete en el XML y contiene la anterior.
	/// </summary>
	[System.Serializable]
    public class InfoItemsDatabase
    {
        [XmlArray("InfoItems")]
        public List<InfoItem> DataBase = new List<InfoItem>();
    }
}
    


