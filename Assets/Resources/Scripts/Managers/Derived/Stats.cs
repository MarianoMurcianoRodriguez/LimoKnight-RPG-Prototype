using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    [System.Serializable]
    public class Stats
    {
        public int Constitution;
        public int Strength;
        public int Defense;
        public int Luck;

        public void SetValues(int constitution, int strength, int defense,  int luck)
        {
            this.Constitution = constitution; this.Strength = strength; this.Defense = defense;  this.Luck = luck;
        }

        public void DecreaseStats(Stats stats)
        {
            this.Constitution -= stats.Constitution; this.Strength -= stats.Strength; this.Defense -= stats.Defense;
            this.Luck -= stats.Luck;
        }

        public void IncreaseStats(Stats stats)
        {
            this.Constitution += stats.Constitution; this.Strength += stats.Strength; this.Defense += stats.Defense;
            this.Luck += stats.Luck;
        }
    }
}

