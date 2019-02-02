using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public class CombatTurnManager : Singleton<CombatTurnManager>
    {
        [SerializeField] private GameObject _mainCombatPanel;
        [SerializeField] private CombatUpdater _combatUpdater;

        private List<bool> _enemiesAlive = new List<bool>();
        private List<EnemyCombat> _enemiesInCombat = new List<EnemyCombat>();
        private PlayerCombat _playerCombat;

        private ActualTurn _actualTurn = ActualTurn.None;

        private List<EnemyData> _enemiesAddToBattle = new List<EnemyData>();

        private bool _turnInProgress = false;
        private bool _playerIsDead = false;
        private bool _playerTurnInProgress = false;
        private int _enemyDoingAction = -1;
        private int _enemyAttacked = -1;
        private int _goldGained = 0;
        private int _expGained = 0;
        private bool _bossBattle = false;
        private EnemyCombat _actualEnemy;

        private int _maxEnemiesInBattle = 4;
        private ActualTurn _preloadTurn = ActualTurn.None;
        private bool _startedBattle = false;   //to know what turn should be applied 
        private bool _canEnemiesBeAdded = false; //to add Enemies uppon contact
        
        public ActualTurn PreloadTurn
        {
            set {if (_preloadTurn == ActualTurn.None && value != ActualTurn.None)
                { 
                    _startedBattle = true;
                    _preloadTurn = value;
                    StartCoroutine(LoadEnemies());
                } }
        }
        public bool StartedBattle
        {
            get { return _startedBattle; }
        }
        public bool CanEnemiesBeAdded
        {
            get { return _canEnemiesBeAdded; }
        }
        public bool PlayerIsDead
        {
            get { return _playerIsDead; }
            set { _playerIsDead = value; }
        }
        public List<EnemyCombat> EnemiesInCombat
        {
            get { return _enemiesInCombat; }
        }
        public List<bool> EnemiesAlive
        {
            get { return _enemiesAlive; }
        }
        
        //This corrutine will be called the first time an an enemy is hitted by a weapon
        //after all enemies hitted will be added because they wont call this but another
        //method which will try to add the enemy
        private IEnumerator LoadEnemies()
        {
            GameManager.Instance.SwapContextToCombatFromDungeon();
            _canEnemiesBeAdded = true;
            float timePassed = 0f;
            float timeToPass = 0.35f;
            while (timePassed < timeToPass)
            {
                timePassed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            _canEnemiesBeAdded = false;
        }

        public bool AddEnemy(EnemyData enemy)
        {
            if (_enemiesAddToBattle.Count < _maxEnemiesInBattle)
            {
                _enemiesAddToBattle.Add(enemy);
                return true;
            }
            return false;
        }

        public void InitializeBattle()
        {
            ControlManager.Instance.PlayerContext = PlayerContextType.InCombat;
            StatsManager.Instance.RecalculateStatsFromPlayer(); //loads stats + weapon
            _playerCombat = _combatUpdater.PlayerCombat;
            _enemyAttacked = -1;
            for (int i = 0; i < _enemiesAddToBattle.Count; i++)
            {
                if ((int)_enemiesAddToBattle[i].TypeEnemy>=100) _bossBattle = true;
                EnemyCombat ec = _combatUpdater.EnemiesSlots[i].GetComponent<EnemyCombat>();
                _enemiesInCombat.Add(ec);
                _enemiesAlive.Add(true);
                _enemiesInCombat[i].SetEnemyData(_enemiesAddToBattle[i], StatsManager.Instance.InfoPlayer.ActualLevel);
            }
            UIManager.Instance.ShowUIMainPanel(UIMainPanel.Combat);  //Combat panel -> 1st transit
            _actualTurn = _preloadTurn;
        }
        
        
        //Everyframe if there's a turn, we will look up for who should do X thing enemies or the player send actions and this class process them sendind 'damage'
        //or anything those selected by them, and mostly important, swap turns when they all said they're done its the turn of the player and viceversa.
        //So we must now: who starts the battle, then who follows up, then how many eneemies are, how many alive are, keeping track of rewards (before passing them to the 
        //stats manager etc)
        private void Update()
        {
            if (_turnInProgress == true || _actualTurn == ActualTurn.None) return;
            switch (_actualTurn)
            {
                case ActualTurn.PlayerTurn:
                    {
                        DoPlayerTurn();
                        break;
                    }
                case ActualTurn.EnemyTurn:
                    {
                        _enemyDoingAction = 0;
                        DoEnemyTurn(_enemyDoingAction);
                        break;
                    }
                case ActualTurn.BattleEnded:
                    {
                        BattleEnded();
                        break;
                    }
            }
            if (_actualTurn != ActualTurn.None)
                _turnInProgress = true;
        }
        private void ChangeTurn()
        {
            ActualTurn at = ActualTurn.None;
            switch (_actualTurn)
            {
                case ActualTurn.PlayerTurn:
                    {
                        if (EnemiesStillAlive())
                        {
                            at = ActualTurn.EnemyTurn;
                            _combatUpdater.DisableMenuCombat();
                        }
                        else
                            at = ActualTurn.BattleEnded;
                        break;
                    }
                case ActualTurn.EnemyTurn:
                    {
                        if (_playerIsDead) at = ActualTurn.BattleEnded;
                        else
                            at  = ActualTurn.PlayerTurn;
                        break;
                    }
            }
            _actualTurn = at;
            _turnInProgress = false;
        }

        /// <summary>
        /// When its the turn to all the enemies, we will go from the first one till
        /// the last one, they can either do an action or pass because they're dead
        /// </summary>
        private void DoEnemyTurn(int enemyIndex)
        {
            if (!_enemiesAlive[enemyIndex]) EnemyActionEnded();
            else
            {
                _enemiesInCombat[enemyIndex].DoDamage();    //we call the enemy to deal damage
            }
        }

        private bool EnemiesStillAlive()
        {
            foreach (bool b in _enemiesAlive)
            {
                if (b) return true;
            }
            return false;
        }

        // In case that the enemy is ended it actions this will be called and either 
        // Give the turn to other enemy or end this turn. This action will be called
        //when the player recives the atack from the enemy (animation enemy > anim player > this)
        public void EnemyActionEnded()
        {
            _enemyDoingAction++;
            if (_enemyDoingAction < _enemiesInCombat.Count)
            {
                DoEnemyTurn(_enemyDoingAction);
            }
            else
            {
                ChangeTurn();
            }
        }


        //This method will be called by an enemy in case that a player kills him
        //This could happen either because he kills them in his turn (in which case this
        //is the last function called by the player) this is not hard to distinguish because
        //we know in each moment
        public void ActualEnemyDied(int expGained, int goldGained, EnemyCombat ec)
        {
            if (_actualTurn == ActualTurn.EnemyTurn)
            {
                _enemiesAlive[_enemiesInCombat.IndexOf(ec)] = false;
                _expGained += expGained;
                _goldGained += goldGained;
                EnemyActionEnded();
            }
            else if (_actualTurn == ActualTurn.PlayerTurn)
            {

                _enemiesAlive[_enemyAttacked] = false;
                _expGained += expGained;
                _goldGained += goldGained;
                PlayerAttackEnded();
            }
        }

        //Because now each enemy will be playing his turn, instead of going in a coroutine
        //This means the player dies and we 'forget' about the rest of them 
        public void PlayerDied()
        {
            _playerIsDead = true;
            ChangeTurn();
        }

        //Because the enemy doesnt have a conexion with the player, he passes it to the 
        //combat manager after he does the attack animation
        public void DamagePlayer(int damage)
        {
            _playerCombat.ReciveAttack(damage);
        }

        private void DoPlayerTurn()
        {
            _playerTurnInProgress = true;
            _combatUpdater.EnableMenuCombatActionPlayer();
        }

        //Same as DamagePlayer, because the player doesnt have a reference about the enemy
        //he must delegate on this previous to do any damage.
        public void DamageEnemy(int index)
        {
            _enemyAttacked = index;
            if (_enemiesAlive[index]==false) return;
            _actualEnemy = _enemiesInCombat[index];
            _playerCombat.DoAttack();
        }
        
        //After making his anmation, and his calculation he returns his damage, type and effect
        //and we send them to the enemy selected
        public void DealDamageToEnemy(int damage, TypeEffect te, int turns)
        {
            _actualEnemy.RecieveAttack(damage, te, turns);
        }

        //If the enemy survives the damage this will be the last thing called after
        //his animation, we will wait at least 1 second so its not so fast. If the enemy
        //dies, however, it will be called instead the ActualEnemyDied
        public void PlayerAttackEnded()
        {
            _playerTurnInProgress = false;
            Invoke("ChangeTurn", 1f);
        }

        private void BattleEnded()
        {
            if (_playerIsDead)
            {
                _expGained = Mathf.RoundToInt(_expGained * 0.5f);
                _goldGained = Mathf.RoundToInt(_goldGained * 0.5f);
            }
            if (_bossBattle == true)
            {
                if (!(InventoryManager.Instance.PlayerHas("The Golden Key") 
                    || StatsManager.Instance.InfoPlayer.IsGameEnded == true)){
                    InfoItem key = new InfoItem();
                    key.SetValues("The Golden Key", TypeItem.Important, "A forgotten key, cursed, everyone who touched it before is now dead", ConsumeEffect.None, 0, 1, "/Items/Important/GoldenKey");
                    if (InventoryManager.Instance.AvailableItemSpace>0)
                        InventoryManager.Instance.AddToInventory(key);
                }
                _expGained = Mathf.RoundToInt(_expGained * _playerCombat.LuckAfterBattle);
                _goldGained = Mathf.RoundToInt(_goldGained * _playerCombat.LuckAfterBattle);
            }
            DungeonManager.Instance.CombatEnded(_enemiesInCombat.Count, _playerIsDead);
            _actualTurn = ActualTurn.None;
            _turnInProgress = false;
            _playerIsDead = false;
            _playerTurnInProgress = false;
            _combatUpdater.ResetSlots();
            _enemiesAlive.Clear();
            _enemiesInCombat.Clear();
            _enemiesAddToBattle.Clear();
            _bossBattle = false;
            _preloadTurn = ActualTurn.None;
            _startedBattle = false;    
            _canEnemiesBeAdded = false;
            _enemyAttacked = -1;
            _enemyDoingAction = -1;
             GameManager.Instance.SwapContextToDungeonFromCombat();
        }

        public int[] ObtainExpAndGold()
        {
            int[] rewards = new int[2];
            rewards[0] = _expGained;
            rewards[1] = _goldGained;
            _expGained = 0;
            _goldGained = 0;
            return rewards;
        }
    }


    public enum ActualTurn
    {
        None,
        PlayerTurn,
        EnemyTurn,
        BattleEnded,
    }


}

