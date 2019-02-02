using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml.Serialization;					//acceso a XMLSerializer que gestiona los XML
using System.Xml;								//atributos basicos de XML
using System.IO;								//Operacion de entrada y salida para fichero

namespace LimoKnight
{
    /// <summary>
    /// Manager XMl. Esta clase contiene las operaciones para guardar y escribir sobre diferentes
    /// ficheros XML, asi como para extraer datos.
    /// </summary>
    public class XMLManager : Singleton<XMLManager>
    {   
        private InfoWeaponsXMLManager _infoWeaponsXMLManager = new InfoWeaponsXMLManager();
        private InfoItemsXMLManager _infoItemsXMLManager = new InfoItemsXMLManager();
        private InfoArmorXMLManager _infoArmorXMLManager = new InfoArmorXMLManager();
        private InfoPlayerXMLManager _infoPlayerXMLManager = new InfoPlayerXMLManager();

        /********************************METHODS OF CLASS****************************************/
        public void SaveWeaponsDatabaseIntoXML(InfoWeaponsDatabase weaponsToSave)
        {
            _infoWeaponsXMLManager.CreateInfoWeaponsDatabaseXML(weaponsToSave, ControlManager.Instance.PathToSaveGame);
        }
        public InfoWeaponsDatabase LoadWeaponsDatabaseFromXML()
        {
            return _infoWeaponsXMLManager.LoadInfoWeaponsDatabaseXML(ControlManager.Instance.PathToSaveGame);
        }
        public void SaveItemsDatabaseIntoXML(InfoItemsDatabase itemsToSave)
        {
            _infoItemsXMLManager.CreateInfoItemsDatabaseXML(itemsToSave, ControlManager.Instance.PathToSaveGame);
        }
        public InfoItemsDatabase LoadItemsDatabaseFromXML()
        {
            return _infoItemsXMLManager.LoadInfoItemsDatabaseXML(ControlManager.Instance.PathToSaveGame);
        }
        public void SaveArmorDatabaseIntoXML(InfoArmorDatabase armorToSave)
        {
            _infoArmorXMLManager.CreateInfoArmorDatabaseXML(armorToSave, ControlManager.Instance.PathToSaveGame);
        }
        public InfoArmorDatabase LoadArmorDatabaseFromXMl()
        {
            return _infoArmorXMLManager.LoadInfoArmorDatabaseXML(ControlManager.Instance.PathToSaveGame);
        }
        public void SaveInfoPlayerIntoXML(InfoPlayer infoPlayer)
        {
            _infoPlayerXMLManager.CreateInfoPlayerXML(infoPlayer, ControlManager.Instance.PathToSaveGame);
        }
        public InfoPlayer LoadInfoPlayerFromXML()
        {
            return _infoPlayerXMLManager.LoadInfoPlayerXML(ControlManager.Instance.PathToSaveGame);
        }

        public string SaveFolderPath() { return ControlManager.Instance.PathToSaveGame; }

        public void BuildSaveFolder()
        {
            if (Directory.Exists(ControlManager.Instance.PathToSaveGame)) return;
            else Directory.CreateDirectory(ControlManager.Instance.PathToSaveGame);
        }

        public void ResetAllGameData()
        {
            _infoItemsXMLManager.DeleteExistingInfoItemsDatabaseXML(SaveFolderPath());
            _infoWeaponsXMLManager.DeleteExistingInfoWeaponsDatabaseXML(SaveFolderPath());
            _infoArmorXMLManager.DeleteInfoArmorDatabaseXML(SaveFolderPath());
            _infoPlayerXMLManager.DeleteExistingInfoPlayerXML(SaveFolderPath());
        }

        public void SaveAllData()
        {
            string savePath = ControlManager.Instance.PathToSaveGame;
            _infoArmorXMLManager.CreateInfoArmorDatabaseXML(InventoryManager.Instance.GetAllArmorsInInventory(), savePath);
            _infoWeaponsXMLManager.CreateInfoWeaponsDatabaseXML(InventoryManager.Instance.GetAllWeaponsInInventory(), savePath);
            _infoItemsXMLManager.CreateInfoItemsDatabaseXML(InventoryManager.Instance.GetAllItemsInInventory(), savePath);
            _infoPlayerXMLManager.CreateInfoPlayerXML(StatsManager.Instance.InfoPlayer, savePath);
        }

        /// <summary>
        /// Method to know if a game save existed prior to the game, this can include a simple game
        /// </summary>
        /// <returns>true or false</returns>
        public bool GameSaveExisted()
        {
            if (Directory.Exists(ControlManager.Instance.PathToSaveGame))
            {
                if (Directory.GetFiles(ControlManager.Instance.PathToSaveGame).Length > 0)  return true;
                return false;
            }
            return false;
        }

        /// <summary>
        /// Metod to create the savefolder in the, this will only be called if the directory doesnt exist.
        /// </summary>
        public void CreateSaveFolder()
        {
            if (Directory.Exists(ControlManager.Instance.PathToSaveGame)) return;
            Directory.CreateDirectory(ControlManager.Instance.PathToSaveGame);
        }

        /// <summary>
        /// Method to copy the default saves to the persistent folder
        /// </summary>
        public void SaveDefaultValues()
        {
            string[] files = Directory.GetFiles(ControlManager.Instance.PathToDefaultSaveValue);
          
            for (int i = 0; i<files.Length; i++)
            {
                File.Copy(files[i], SaveFolderPath() + "/" + Path.GetFileName(files[i]));
            }
        }

        public void LoadDefaultData()
        {
            string pathDSF = ControlManager.Instance.PathToDefaultSaveValue;
            InfoWeaponsDatabase iwd = _infoWeaponsXMLManager.LoadInfoWeaponsDatabaseXML(pathDSF);
            InventoryManager.Instance.SetInventoryWeapons(iwd.DataBase);
            InfoArmorDatabase iad = _infoArmorXMLManager.LoadInfoArmorDatabaseXML(pathDSF);
            InventoryManager.Instance.SetInventoryArmor(iad.Database);
            InfoPlayer ip = _infoPlayerXMLManager.LoadInfoPlayerXML(pathDSF);
            StatsManager.Instance.InfoPlayer = ip;

        }

        public void LoadAllData()
        {
            string pathSG = ControlManager.Instance.PathToSaveGame;
            InfoWeaponsDatabase iwd = _infoWeaponsXMLManager.LoadInfoWeaponsDatabaseXML(pathSG);
            InventoryManager.Instance.SetInventoryWeapons(iwd.DataBase);
            InfoArmorDatabase iad = _infoArmorXMLManager.LoadInfoArmorDatabaseXML(pathSG);
            InventoryManager.Instance.SetInventoryArmor(iad.Database);
            InfoItemsDatabase iim = _infoItemsXMLManager.LoadInfoItemsDatabaseXML(pathSG);
            InventoryManager.Instance.SetInventoryItems(iim.DataBase);
            InfoPlayer ip = _infoPlayerXMLManager.LoadInfoPlayerXML(pathSG);
            StatsManager.Instance.InfoPlayer = ip;
        }


        private void Start()
        {/*
            InfoPlayer ip = new InfoPlayer();
            Stats s = new Stats();
            s.SetValues(10, 5, 3, 0);
            List<float> increaseByLevel = new List<float>();
            increaseByLevel.Add(1.2f); increaseByLevel.Add(1.5f); increaseByLevel.Add(0.7f); increaseByLevel.Add(0.3f);
            ip.SetValues("/DialogueSprite/Limo", "/FullSprites/LimoSprite", false, 0, 0, 0, 0, 0, 0, s, increaseByLevel, new List<int>(), new List<int>());
            _infoPlayerXMLManager.CreateInfoPlayerXML(ip, ControlManager.Instance.PathToDefaultSaveValue);*/
        }
    }

    public class InfoWeaponsXMLManager
    {
        public string nameXMLFile = "/DatabaseWeaponData.xml";
        /// <summary>
        /// Function to generate an XML (usefull to override an existing one).
        /// </summary>
        public void CreateInfoWeaponsDatabaseXML(InfoWeaponsDatabase database, string destinyFolder)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(InfoWeaponsDatabase));
            FileStream stream = new FileStream(destinyFolder + nameXMLFile, FileMode.Create);
            serializer.Serialize(stream, database);  //we  write on that file
            stream.Close();
        }

        /// <summary>
        /// Function to extract the database from the xml in the specified directory
        /// </summary>
        /// <returns>The Database contained in the xml</returns>
        public InfoWeaponsDatabase LoadInfoWeaponsDatabaseXML(string sourceFolder)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(InfoWeaponsDatabase));
            FileStream stream = new FileStream(sourceFolder + nameXMLFile, FileMode.Open);
            InfoWeaponsDatabase iwdb = serializer.Deserialize(stream) as InfoWeaponsDatabase;
            stream.Close();
            return iwdb;
        }

        public void DeleteExistingInfoWeaponsDatabaseXML(string sourceFolder)
        {
            File.Delete(sourceFolder + nameXMLFile);
        }

    }
    public class InfoItemsXMLManager
    {
        public string nameXMLFile = "/DatabaseItemData.xml";
   
        public void CreateInfoItemsDatabaseXML(InfoItemsDatabase database, string destinyFolder)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(InfoItemsDatabase));
            FileStream stream = new FileStream(destinyFolder + nameXMLFile, FileMode.Create);
            serializer.Serialize(stream, database);  //we  write on that file
            stream.Close();
        }

        public InfoItemsDatabase LoadInfoItemsDatabaseXML(string sourceFolder)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(InfoItemsDatabase));
            FileStream stream = new FileStream(sourceFolder + nameXMLFile, FileMode.Open);
            InfoItemsDatabase iidb = serializer.Deserialize(stream) as InfoItemsDatabase;
            stream.Close();
            return iidb;
        }

        public void DeleteExistingInfoItemsDatabaseXML(string sourceFolder)
        {
            File.Delete(sourceFolder + nameXMLFile);
        }
    }
    public class InfoArmorXMLManager
    {
        public string nameXMLFile = "/DatabaseArmorData.xml";

        public void CreateInfoArmorDatabaseXML(InfoArmorDatabase database, string destinyFolder)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(InfoArmorDatabase));
            FileStream stream = new FileStream(destinyFolder + nameXMLFile, FileMode.Create);
            serializer.Serialize(stream, database);
            stream.Close();
        }

        public InfoArmorDatabase LoadInfoArmorDatabaseXML(string sourceFolder)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(InfoArmorDatabase));
            FileStream stream = new FileStream(sourceFolder + nameXMLFile, FileMode.Open);
            InfoArmorDatabase iad = serializer.Deserialize(stream) as InfoArmorDatabase;
            stream.Close();
            return iad;
        }

        public void DeleteInfoArmorDatabaseXML(string sourceFolder)
        {
            File.Delete(sourceFolder + nameXMLFile);
        }
    }
    public class InfoPlayerXMLManager
    {
        public string nameXMLFile = "/InfoPlayerData.xml";
        /// <summary>
        /// Function to generate an XML (usefull to override an existing one).
        /// </summary>
        public void CreateInfoPlayerXML(InfoPlayer ip, string destinyFolder)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(InfoPlayer));
            FileStream stream = new FileStream(destinyFolder + nameXMLFile, FileMode.Create);
            serializer.Serialize(stream, ip);  //we  write on that file
            stream.Close();
        }

        /// <summary>
        /// Function to extract the database from the xml in the specified directory
        /// </summary>
        /// <returns>The infoplayer (not the database because there's only one) contained in the xml</returns>
        public InfoPlayer LoadInfoPlayerXML(string sourceFolder)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(InfoPlayer));
            FileStream stream = new FileStream(sourceFolder + nameXMLFile, FileMode.Open);
            InfoPlayer ip = serializer.Deserialize(stream) as InfoPlayer;
            stream.Close();
            return ip;
        }

        public void DeleteExistingInfoPlayerXML(string sourceFolder)
        {
            File.Delete(sourceFolder + nameXMLFile);
        }
    }
}
    


