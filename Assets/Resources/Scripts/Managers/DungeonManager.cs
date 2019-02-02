using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public class DungeonManager : Singleton<DungeonManager>
    {
        private ItemDroper ItemDroper = new ItemDroper();
        private LevelGenerator _levelGenerator;
        private int _actualLevelDungeon = -1;
        private int _levelPlayer = 0;
        private int _maxLevelDungeon = 9;
        private int _numberOfEnemiesKilled = 0;
        private int _numberOfEnemiesToBeat = 0;
        private int[] _rewardsObtained = new int[2];
        private bool _generationHasStartedAndEnded = false;

        public bool GenerationHasStartedAndEnded()
        {
            return _levelGenerator.LevelWasGenerated;
        }

        private void Start()
        {
            _levelGenerator = gameObject.GetComponent<LevelGenerator>();
        }

        public void GenerateLevel()
        {
            _actualLevelDungeon++;
            LoadLevel();
        }

        public void LoadLevel()
        {
           _numberOfEnemiesToBeat = Mathf.RoundToInt(Random.Range(_actualLevelDungeon / 2.5f, (_actualLevelDungeon / 2f) + 2) + _actualLevelDungeon / 1.5f + _actualLevelDungeon/2f + Mathf.Max(2, _actualLevelDungeon/3f));
            _numberOfEnemiesKilled = 0;
            _levelGenerator.GenerateLevel();
           StartCoroutine(GenerateEnemiesCorutine(_numberOfEnemiesToBeat));
        }

        private IEnumerator GenerateEnemiesCorutine(int numberEnemies)
        {
            int levelEnemy = Mathf.RoundToInt(_levelPlayer * 1 + Random.Range(_actualLevelDungeon / 30f, _actualLevelDungeon / 100f));
            yield return new WaitUntil(() => _levelGenerator.LevelWasGenerated);
                _levelGenerator.InstantiateEnemies(levelEnemy, numberEnemies);
            _levelGenerator.SetPositionPlayer();
        }

        private void SpawnBossFloor()
        {
            int levelEnemy = Mathf.RoundToInt(_levelPlayer * 1 + Random.Range(_actualLevelDungeon / 100f, _actualLevelDungeon/30f));
            _levelGenerator.SpawnBoss(levelEnemy);
        }

        //Function called by CombatTurnManager each time a combat ends, this allows us to figurate when 
        //we should spawn the boss, destroy the level because the player died or spawn the prompt towards
        //the next level
        public void CombatEnded(int numberOfEnemiesKilled, bool playerIsDead)
        {
            if (playerIsDead)
            {
                StartCoroutine(EndDungeonWithoutRewards());
            }
            else
            {
                _numberOfEnemiesKilled = numberOfEnemiesKilled + _numberOfEnemiesKilled;

                if (_numberOfEnemiesToBeat == _numberOfEnemiesKilled) SpawnBossFloor();
                else if (_numberOfEnemiesKilled > _numberOfEnemiesToBeat)
                {
                    StartCoroutine(ShowPanelNextDungeon());
                }
            }
        }

        //Hide the KeepFighting, show, the next one...apply exp winned, and call to inventorygiver
        public void LeaveDungeon() {
            StartCoroutine(EndDungeonCoroutine());
        }

        private IEnumerator ShowPanelNextDungeon()
        {
            yield return new WaitForSeconds(1.5f);
            UIManager.Instance.ShowUISecondaryPanel(UISecondaryPanel.KeepFightingDungeon);
        }

        public void KeepFighting()
        {
            if (_actualLevelDungeon==_maxLevelDungeon) EndDungeonCoroutine();
            _actualLevelDungeon++;
            GameManager.Instance.SwapContextToDungeonFromDungeon();

        }

        public void LoadEnemiesOnLevel()
        {
            _numberOfEnemiesToBeat = Mathf.RoundToInt(Random.Range(_actualLevelDungeon / 2.5f, (_actualLevelDungeon / 2f) + 2) + _actualLevelDungeon / 1.5f + _actualLevelDungeon / 2f + Mathf.Max(2, _actualLevelDungeon / 3f));
            _numberOfEnemiesKilled = 0;
            StartCoroutine(GenerateEnemiesCorutine(_numberOfEnemiesToBeat));
        }

        private IEnumerator EndDungeonCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            _levelGenerator.DestroyLevel();
            UIManager.Instance.HideUISecondaryPanel(UISecondaryPanel.KeepFightingDungeon);
            _rewardsObtained = CombatTurnManager.Instance.ObtainExpAndGold();
            //NOW we pop up the rewards panel
            GameManager.Instance.SwapContextToWorldFromCombatOrDungeon();
            yield return new WaitForSeconds(3.5f);
            StatsManager.Instance.AddExperience(_rewardsObtained[0]);
            StatsManager.Instance.AddGold(_rewardsObtained[1]);
            ItemDroper.GiveRewards(StatsManager.Instance.InfoPlayer.ActualLevel, _actualLevelDungeon);
       }

        private IEnumerator EndDungeonWithoutRewards()
        {
            Debug.Log("Player died");
            yield return new WaitForSeconds(0.5f);
            _levelGenerator.DestroyLevel();
            UIManager.Instance.HideUISecondaryPanel(UISecondaryPanel.KeepFightingDungeon);
            _rewardsObtained = CombatTurnManager.Instance.ObtainExpAndGold();
            //NOW we pop up the rewards panel
            GameManager.Instance.SwapContextToWorldFromCombatOrDungeon();
            yield return new WaitForSeconds(3.5f);
            StatsManager.Instance.AddExperience(_rewardsObtained[0]);
            StatsManager.Instance.AddGold(_rewardsObtained[1]);
        }
    }
}

