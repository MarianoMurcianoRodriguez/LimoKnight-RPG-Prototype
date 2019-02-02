using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.Xml.Serialization;					//acceso a XMLSerializer que gestiona los XML
using System.Xml;								//atributos basicos de XML
using System.IO;								//Operacion de entrada y salida para fichero

namespace LimoKnight
{
    /// <summary>
    /// Container class to hold the data from the player, this could mean what kind of archivements are unlocked
    /// what are not, how many gold it has, how many exp....this means: VARIABLE INFORMATION but relevant.
    /// </summary>
    [System.Serializable]
    public class InfoPlayer
    {
        public string SimpleSprite;
        public string FullBodySprite;
        public bool IsGameEnded;
        public int TotalNumberDeaths;
        public int TotalMonstersSlayed;
        public int ActualGold;
        public int TotalGoldObtained;
        public int ActualLevel;
        public int ActualXP;
        //Base stats, they will NEVER change. 
        public Stats Stats;
        public List<float> IncrementByLevel;
        public List<int> Missions;
        public List<int> Journals;

        public void SetValues(string simpleSprite, string fullBodySprite, bool isGameEnded, int totalNumberDeaths, int totalMonstersSlayed, int actualGold,
            int totalGoldObtained, int actualLevel, int actualXP, Stats stats, List<float> incrementByLevel,
            List<int> missions, List<int> journals)
        {
            this.SimpleSprite = simpleSprite; this.FullBodySprite = fullBodySprite; this.IsGameEnded = isGameEnded; this.TotalNumberDeaths = totalNumberDeaths;
            this.TotalMonstersSlayed = totalMonstersSlayed; this.ActualGold = actualGold; this.TotalGoldObtained = totalGoldObtained;
            this.ActualLevel = actualLevel; this.ActualXP = actualXP; this.Stats = stats; this.IncrementByLevel = incrementByLevel;
            this.Missions = missions; this.Journals = journals;
        }
    }
    
}

