using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LimoKnight
{
    public class LevelsPlayer {
        public int ExpForLevel(int i)
        {
            return (Mathf.RoundToInt(i * (10 + Mathf.Log(i * i) + 25*i)));
        }
    };
    //While this class controls the stats of the player it'll contain all
    //infoplayer andanyone who wants to talk with that class shall call from here
    //and calculate the level actual, and the increments on the stats
    public class StatsManager : Singleton<StatsManager>
    {
        //Objects to update UI values
        [SerializeField] private Text CurrentLevelText;
        [SerializeField] private Text CurrentConstitutionText;
        [SerializeField] private Text CurrentStrengthText;
        [SerializeField] private Text CurrentDefenseText;
        [SerializeField] private Text CurrentLuckText;
        [SerializeField] private Image CurrentLevelExperienceValueImage;
        [SerializeField] private Image CurrentHPValueImage;
        [SerializeField] private Text CurrentHPAndMaxHPText;
        [SerializeField] private Text CurrentGoldText;
        [SerializeField] private Image CharacterSimpleSprite;
        [Tooltip("PlayerCombat of the combat panel!")]
        [SerializeField] private PlayerCombat _playerCombat;

        private int _actualValueHP = 0;
        
        
        private InfoPlayer _infoPlayer;
        //Stats obtained either for loading first the data (through _infoPlayer) or after leveling up
        private Stats _statsPlayer = new Stats();
        private Stats _statsInventory = new Stats();
        private Stats _actualStats = new Stats();

        private LevelsPlayer _levels = new LevelsPlayer();

        public InfoPlayer InfoPlayer
        {
            get { return _infoPlayer; }
            set { _infoPlayer = value;
                UpdateAllFromPlayer();
            }
        }
        
        public Stats ActualStats
        {
            get { return _actualStats; }
            set { _actualStats = value; }
        }

        private void Start()
        {
            CurrentLevelExperienceValueImage.fillAmount = 0f;
            CurrentHPValueImage.fillAmount = 1f;
        }


        //Constant values probably in a real game this should change drastically on testing: 0.6f, 0f, 50f
        public void RecalculateStatsFromPlayer()
        {
            int maxHP = _actualStats.Constitution;
            int damage = _actualStats.Strength;
            int damageReduction = Mathf.RoundToInt(Mathf.Clamp(_actualStats.Defense * 0.15f, 0f, 50f));
            int criticalChance = Mathf.RoundToInt(Mathf.Clamp(_actualStats.Luck * 0.25f, 0f, 90f));
            float luckAfterBattle = 1 + _actualStats.Luck * 0.005f;
            if (_actualValueHP == 0) _actualValueHP = maxHP;
            _playerCombat.SetActualStats(_actualValueHP, maxHP, damage, damageReduction, criticalChance, luckAfterBattle);
            if (ControlManager.Instance.PlayerContext == PlayerContextType.InCombat)
            {
                _playerCombat.SpriteFull = Resources.Load(ControlManager.Instance.PathToPlayerSprites +
                                         _infoPlayer.FullBodySprite, typeof(Sprite)) as Sprite;
                _playerCombat.SetInfoWeapon(InventoryManager.Instance.GetEquippedWeapon());
            } 
           
        }

        public void RecalculateStatsInventory(Stats statsFromInventory)
        {
            _actualStats.Constitution = _statsPlayer.Constitution + statsFromInventory.Constitution;
            _actualStats.Strength = _statsPlayer.Strength + statsFromInventory.Strength;
            _actualStats.Defense = _statsPlayer.Defense + statsFromInventory.Defense;
            _actualStats.Luck = _statsPlayer.Luck + statsFromInventory.Luck;
            _statsInventory = statsFromInventory;
            RecalculateStatsFromPlayer();
            RefreshStatsUI();
        }

        public void AddExperience(int experiencePoints)
        {
            int experience = _infoPlayer.ActualXP + experiencePoints;
            int nextLevel = _infoPlayer.ActualLevel + 1;
            int necessaryExp = _levels.ExpForLevel(nextLevel);
            if (experience < necessaryExp)
                _infoPlayer.ActualXP = experience;

            else
            {
                do
                {
                    _infoPlayer.ActualXP = experience - necessaryExp;       //la que tengamos: total - necesaria
                    _infoPlayer.ActualLevel = nextLevel;
                    nextLevel = _infoPlayer.ActualLevel + 1;
                    necessaryExp = _levels.ExpForLevel(nextLevel);
                }
                while (!(experience < necessaryExp));
                CurrentLevelText.text = "<b>" + nextLevel.ToString() + "</b>";
                UpdateStatsPlayer();
            } 
            /*else
            {
                _infoPlayer.ActualXP = experience - necessaryExp;
                _infoPlayer.ActualLevel = nextLevel;
                CurrentLevelText.text = "<b>"+ nextLevel.ToString() + "</b>";
                UpdateStatsPlayer();
            }*/
            float percentOfLevel = (_infoPlayer.ActualXP/(float)_levels.ExpForLevel(_infoPlayer.ActualLevel+1));
            RefreshExpUI(percentOfLevel);
           
        }

        public void AddGold(int goldCoins)
        {
            _infoPlayer.ActualGold += goldCoins;
            if (goldCoins > 0)
                _infoPlayer.TotalGoldObtained += goldCoins;
           RefreshGoldUI(_infoPlayer.ActualGold);
        }

        private void UpdateAllFromPlayer()
        {
            UpdateStatsPlayer();
            RefreshExpUI(_infoPlayer.ActualXP/ (float) _levels.ExpForLevel(_infoPlayer.ActualLevel+1));
            RefreshGoldUI(_infoPlayer.ActualGold);
            CurrentLevelText.text = "<b>" + _infoPlayer.ActualLevel + "</b>";
        }
        private void UpdateStatsPlayer()
       {
            RefreshSimpleSpritePlayer();
            Stats newStats = _infoPlayer.Stats;
            //Because stats are always the same (base) we gotta calculate
            newStats.Constitution = Mathf.FloorToInt(_infoPlayer.IncrementByLevel[0] * _infoPlayer.ActualLevel + _infoPlayer.Stats.Constitution);
            newStats.Strength = Mathf.FloorToInt(_infoPlayer.IncrementByLevel[1] * _infoPlayer.ActualLevel + _infoPlayer.Stats.Strength);
            newStats.Defense = Mathf.FloorToInt(_infoPlayer.IncrementByLevel[2] * _infoPlayer.ActualLevel + _infoPlayer.Stats.Defense);
            newStats.Luck = Mathf.FloorToInt(_infoPlayer.IncrementByLevel[3] * _infoPlayer.ActualLevel + _infoPlayer.Stats.Luck);
            _statsPlayer = newStats;
            //Now this changes procs the actual to change
            _actualStats.Constitution = _statsPlayer.Constitution + _statsInventory.Constitution;
            _actualStats.Strength = _statsPlayer.Strength + _statsInventory.Strength;
            _actualStats.Defense = _statsPlayer.Defense + _statsInventory.Defense;
            _actualStats.Luck = _statsPlayer.Luck + _statsInventory.Luck;
            RecalculateStatsFromPlayer();
            RefreshStatsUI();
        }
         
        private void RefreshSimpleSpritePlayer()
        {
            CharacterSimpleSprite.sprite = Resources.Load(ControlManager.Instance.PathToPlayerSprites + _infoPlayer.SimpleSprite, typeof(Sprite)) as Sprite;
        }

        private void RefreshStatsUI()
        {
            CurrentConstitutionText.text = "<b>"+ _actualStats.Constitution.ToString() + "</b>";
            CurrentStrengthText.text = "<b>" + _actualStats.Strength.ToString() + "</b>";
            CurrentDefenseText.text = "<b>" + _actualStats.Defense.ToString() + "</b>";
            CurrentLuckText.text = "<b>" + _actualStats.Luck.ToString() + "</b>";
        }

        private void RefreshExpUI(float percent)
        {
            StartCoroutine(IncreaseFill(CurrentLevelExperienceValueImage, 0.7f, percent, 0f));
            CurrentLevelText.text = "<b>" + _infoPlayer.ActualLevel.ToString() +"</b>";
        }

        private void RefreshGoldUI(int quantity)
        {
            CurrentGoldText.text = "<b>" + quantity.ToString() + "</b>";
        }

        public void RefreshHP(int actualValue, int maxValue)
        {
            _actualValueHP = actualValue;
            CurrentHPAndMaxHPText.text = "<b>" + actualValue.ToString() + "/" + maxValue.ToString() + "</b>";
            
            StartCoroutine(IncreaseFill(CurrentHPValueImage, 0.7f, ((actualValue / (float)maxValue)), 0f));
        }

        private IEnumerator IncreaseFill(Image imageToFill, float timeToDoAnimation, float fillToReach, float initialWaitTime)
        {
            float fillToApply = fillToReach - imageToFill.fillAmount;
            if (fillToApply == 0f) yield return null;
            yield return new WaitForSeconds(initialWaitTime);
            float timeSpent = 0;
            float fillOriginal = imageToFill.fillAmount;
            
            while (timeSpent < timeToDoAnimation)
            {
                imageToFill.fillAmount = fillOriginal + (timeSpent / timeToDoAnimation * fillToApply);
                if (imageToFill.fillAmount > 1)
                {
                    imageToFill.fillAmount = 1;
                    timeSpent = timeToDoAnimation;
                }
                else if (imageToFill.fillAmount < 0)
                {
                    imageToFill.fillAmount = 0;
                    timeSpent = timeToDoAnimation;
                }
                timeSpent = timeSpent + Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        public void SaveInfoPlayerStats()
        {
            XMLManager.Instance.SaveInfoPlayerIntoXML(_infoPlayer);
        }

        public void AddJournalsToInfoPlayer(List<int> indexJournals){
            _infoPlayer.Journals = indexJournals;
        }

        public void RestoreHP()
        {
            RefreshHP(_actualStats.Constitution, _actualStats.Constitution);
        }
        
    }
}
