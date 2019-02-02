using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{

    public enum TypeDrop
    {
        Gold,
        Item,
        Weapon,
        Armor,
        TotalTypesDrop = 4
    }

    public class ItemDroper
    {
        private int _floorReached;
        private int _levelPlayer;
        
        private float _normalChance = 50f;      //1-(20+levelplayer) points to give in 2 stats 
        private float _uncommonChance = 30f;          //10-(30+levelplayer) points to give in 2 stats
        private float _rareChance = 15f;              //30-(50+levelplayer) points to give in 3 stats
        private float _extraordinayChance = 5f;       //50-(100+level players) points to give in 4 stats
        //In case of a weapon, the same but for the only stat posible (Damage)

        private float _chanceofWeapon = 40f;
        private float _chanceofArmor = 40f;
        private float _chanceofItem = 20f;

        private float _chanceofGettingGold = 20f; //only used on non-rare or superior items 

        private WeaponRarity actualRarityDrop = WeaponRarity.None; //Used to determinate statspoints and value ((levelplayer+stats) * rarity)
        private int statsPointDrop = -1; //Same for the stats point

        private void EstablishPercentsForItems(int floorReached, int levelPlayer)
        {
            _levelPlayer = levelPlayer;
            _floorReached = floorReached;
        }

        //Method to know how many items should be given (3 + max of 4 items) dependent of the floor
        private int GetQuantityItems()
        {
            return (3 + Mathf.RoundToInt((_floorReached+1) / 3f));
        }

        //Calculating the cost of the drop between the rarity, the level of the player and the statspoints
        private int CostDrop()
        {
            float multiplier = 0f;
            if (actualRarityDrop == WeaponRarity.None || actualRarityDrop == WeaponRarity.Normal)
                multiplier = 1f;
            else if (actualRarityDrop == WeaponRarity.Uncommon)
                multiplier = 1.2f;
            else if (actualRarityDrop == WeaponRarity.Rare)
                multiplier = 1.7f;
            else if (actualRarityDrop == WeaponRarity.Extraordinary)
                multiplier = 2f;
            return Mathf.RoundToInt((_levelPlayer + statsPointDrop) * multiplier);
        }

        //This method will we give us 1 of 4 things posible
        private TypeDrop TypeItemToGive()
        {
            TypeDrop actualDrop;
            int itemIndex = Random.Range(0, 100);
            if (itemIndex <= _chanceofWeapon)           //0-40
                actualDrop = TypeDrop.Weapon;
            else if (itemIndex <= _chanceofArmor + _chanceofWeapon) //40-40+40
                actualDrop = TypeDrop.Armor;
            else {
                itemIndex = Mathf.RoundToInt(Random.Range(0, 100));
                if (itemIndex <= _chanceofGettingGold)
                    actualDrop = TypeDrop.Gold;
                else actualDrop = TypeDrop.Item;
            }
            return actualDrop;
        }

        private void SetQualityItem()
        {
            int number = Random.Range(0, 100);
            if (number <= _normalChance)
                actualRarityDrop = WeaponRarity.Normal;
            else if (number <= _normalChance + _uncommonChance)
                actualRarityDrop = WeaponRarity.Uncommon;
            else if (number <= _normalChance + _uncommonChance + _rareChance)
                actualRarityDrop = WeaponRarity.Rare;
            else if (number <= _normalChance + _uncommonChance + _rareChance + _extraordinayChance)
                actualRarityDrop = WeaponRarity.Extraordinary;
        }

        private int RarityStats()
        {
            if (actualRarityDrop == WeaponRarity.Normal)
                statsPointDrop = Random.Range(1, (20 + _levelPlayer));
            else if (actualRarityDrop == WeaponRarity.Uncommon)
                statsPointDrop = Random.Range(10, (30 + _levelPlayer));
            else if (actualRarityDrop == WeaponRarity.Rare)
                statsPointDrop = Random.Range(30, (50 + _levelPlayer));
            else if (actualRarityDrop == WeaponRarity.Extraordinary)
                statsPointDrop = Random.Range(50, (100 + _levelPlayer));
            return statsPointDrop;
        }

        private WeaponEffect WeaponEffect()
        {
            WeaponEffect we = new WeaponEffect();
            int chanceStatus = Random.Range(0, 100);
            int typeEffect = Random.Range(0, 4);
            int durationStatus = Mathf.RoundToInt(Random.Range(0, 100) * 3);
            if (actualRarityDrop == WeaponRarity.Uncommon && chanceStatus >= 70 && typeEffect != 0)
                we.SetValues((TypeEffect)typeEffect, Random.Range(0f, 100f), durationStatus);
            else if (actualRarityDrop == WeaponRarity.Rare && chanceStatus >= 30)
                we.SetValues((TypeEffect)Random.Range(1, 4), chanceStatus, durationStatus);
            else if (actualRarityDrop == WeaponRarity.Extraordinary)
                we.SetValues((TypeEffect)Random.Range(1, 4), chanceStatus, durationStatus);
            else if (actualRarityDrop == WeaponRarity.Normal)
                we.SetValues(TypeEffect.None, 0f, 0);
            return we;
        }

        //Be careful, the model name in the weapon should also have a prefab called the same way
        //without /Weapon/ (of course) in the gamemodels folder so /Sprites/Inventory/Weapons/0 --> /GameModel/Weapons/0
        /// <summary>
        /// THIS METHOD SHOULD BE CHANGED BECAUSE WE PLACE A MAGICAL NUMBER (10) TO THE NUMBERS OF ITEM THAT THERE'ARE W
        /// </summary>
        /// <returns></returns>
        private string IconNameWeapon()
        {
            return Random.Range(0, 3).ToString();
        }

        private EquippedInSlot SlotArmor()
        {
            return (EquippedInSlot)Random.Range(0, 7);
        }

        private string IconNameArmor(EquippedInSlot slot)
        {
            return Random.Range(0, 3).ToString();
        }

        private Stats ArmorStats()
        {
            Stats stats = new Stats();
            stats.SetValues(0, 0, 0, 0);
            int pointstoGive;
            if (actualRarityDrop == WeaponRarity.Rare)
            {
                pointstoGive = Mathf.RoundToInt(RarityStats() / 3f);
                stats.SetValues(pointstoGive, pointstoGive, pointstoGive, 0);
            }
            else if (actualRarityDrop == WeaponRarity.Extraordinary)
            {
                pointstoGive = Mathf.RoundToInt(RarityStats() / 4f);
                stats.SetValues(pointstoGive, pointstoGive, pointstoGive, pointstoGive);
            }
            else
            {
                pointstoGive = Mathf.RoundToInt(RarityStats() / 2f);
                stats.SetValues(pointstoGive, pointstoGive, 0, 0);
            }
            return stats;
        }

        public string IconNameItem() {
           return Random.Range(0, 3).ToString();
        }

        private void PopulateDrop(TypeDrop actualDrop)
        {
            switch (actualDrop)
            {
                case TypeDrop.Gold:
                    {
                        StatsManager.Instance.AddGold(CostDrop());
                        break;
                    }
                case TypeDrop.Weapon:
                    {
                        InfoWeapon iw = new InfoWeapon();
                        iw.SetValues("Weapon", (float)RarityStats(), false, WeaponType.SimpleSword, WeaponEffect(), actualRarityDrop, 1f, CostDrop(), "/Weapons/"+IconNameWeapon());
                        if (InventoryManager.Instance.AvailableWeaponSpace > 0)
                            InventoryManager.Instance.AddToInventory(iw);
                        else
                            StatsManager.Instance.AddGold(iw.SellValue);
                        
                        break;
                    }
                case TypeDrop.Armor:
                    {
                        InfoArmor ia = new InfoArmor();
                        EquippedInSlot slot = SlotArmor();
                        Stats stats = ArmorStats();
                        ia.SetValues("Armadura", false, slot, CostDrop(), "Some description randomly to be implemented", stats, "/Armors/"+slot.ToString()+"/"+IconNameArmor(slot));
                        if (InventoryManager.Instance.AvailableArmorSpace > 0)
                            InventoryManager.Instance.AddToInventory(ia);
                        else
                            StatsManager.Instance.AddGold(ia.SellValue);
                        break;
                    }
                case TypeDrop.Item:
                    {
                        InfoItem ii = new InfoItem();
                        ii.SetValues("Item", TypeItem.Other, "Some description randomly generated", ConsumeEffect.None, CostDrop(), 1, "/Items/"+IconNameItem());
                        if (InventoryManager.Instance.AvailableItemSpace > 0)
                            InventoryManager.Instance.AddToInventory(ii);
                        else
                            StatsManager.Instance.AddGold(ii.SellValue);
                        break;
                    }
            }
        }

        public void GiveRewards(int levelPlayer, int levelDungeon)
        {
            EstablishPercentsForItems(levelDungeon, levelPlayer);
            for (int i=0; i<GetQuantityItems(); i++)
            {
                SetQualityItem();
                TypeDrop drop_i = TypeItemToGive();
                PopulateDrop(drop_i);
            }
        }

        //Used on the slotmachine this is why the drop is predefined.
        public void DropItemDirectly(int levelPlayer, TypeDrop drop)
        {
            SetQualityItem();
            _levelPlayer = levelPlayer;
            PopulateDrop(drop);
        }
    }
}

 