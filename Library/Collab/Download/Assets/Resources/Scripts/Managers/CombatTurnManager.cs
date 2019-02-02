using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public class CombatTurnManager : Singleton<CombatTurnManager>
    {
        [SerializeField] private GameObject _mainCombatPanel;
        [SerializeField] private CombatUpdater _combatUpdater;

        private List<EnemyCombat> _enemiesDead = new List<EnemyCombat>();
        private Dictionary<int, EnemyCombat> _enemiesInBattle = new Dictionary<int, EnemyCombat>();
        private PlayerCombat _playerCombat;

        private ActualTurn _actualTurn = ActualTurn.None;
        private bool _turnInProgress = false;
        private bool _playerIsDead = false;
        private bool _playerTurnInProgress = false;
        private int _enemiesActionsTurn = 0;
        private int _goldGained = 0;
        private int _expGained = 0;
        private bool _bossBattle = false;
        private EnemyCombat _actualEnemy;

        private int _maxEnemiesInBattle = 4;

        public Dictionary<int, EnemyCombat> EnemiesInBattle
        {
            get { return _enemiesInBattle; }
        }

        public int MaxEnemiesInBattle {
           get { return _maxEnemiesInBattle;}
        }

        public bool PlayerIsDead
        {
            get { return _playerIsDead; }
            set { _playerIsDead = value; }
        }

        public void InitializeBattle(List<EnemyData> enemies, ActualTurn turn)
        {

            ControlManager.Instance.PlayerContext = PlayerContextType.InCombat;
            StatsManager.Instance.RecalculateStatsFromPlayer(); //loads stats + weapon
            _playerCombat = _combatUpdater.PlayerCombat;        
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].TypeEnemy == TypeEnemy.Boss) _bossBattle = true;
                EnemyCombat ec = _combatUpdater.EnemiesSlots[i].GetComponent<EnemyCombat>();
                _enemiesInBattle.Add(i, ec);
                _enemiesInBattle[i].SetEnemyData(enemies[i], StatsManager.Instance.InfoPlayer.ActualLevel);
            }
            UIManager.Instance.ShowUIMainPanel(UIMainPanel.Combat);  //Combat panel -> 1st transit
            _actualTurn = turn;
        }
        

        public void EnemyStartedBattle(List<EnemyData> enemies)
        {
            GameManager.Instance.SwapContextToCombatFromDungeon(enemies, ActualTurn.EnemyTurn);  //GameManager --> transition
        }

        public void PlayerStartedBattle(List<EnemyData> enemies)
        {
            GameManager.Instance.SwapContextToCombatFromDungeon(enemies, ActualTurn.PlayerTurn);
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
                        Debug.Log("TURNO DEL JUGADOR");
                        StartCoroutine(PlayerTurn());
                        break;
                    }
                case ActualTurn.EnemyTurn:
                    {
                        Debug.Log("TURNO DEL ENEMIGO");
                        StartCoroutine(EnemyTurn());
                        break;
                    }
                case ActualTurn.BattleEnded:
                    {
                        StartCoroutine(BattleEnded());
                        break;
                    }
            }
            _turnInProgress = true;
        }
        private void ChangeTurn()
        {
            ActualTurn at = ActualTurn.None;
            switch (_actualTurn)
            {
                case ActualTurn.PlayerTurn:
                    {
                        if (_enemiesInBattle.Count > 0)
                        {
                            at = ActualTurn.EnemyTurn;
                            _combatUpdater.DisableMenuCombatPlayer();
                        }
                        else _actualTurn = ActualTurn.BattleEnded;
                        break;
                    }
                case ActualTurn.EnemyTurn:
                    {
                        if (_playerIsDead) at = ActualTurn.BattleEnded;
                        else at  = ActualTurn.PlayerTurn;
                        break;
                    }
            }
            _actualTurn = at;
            Debug.Log("El turno actual es de: " + _actualTurn);
            _turnInProgress = false;
        }

        public void EnemyActionEnded()
        {
            _enemiesActionsTurn++;
        }
        private IEnumerator EnemyTurn()
        {

            int index = _enemiesInBattle.Count;
            for (int i = 0; i < index; i++)
            {
                Debug.Log("Enemy " + i + " turn.");
                if (_enemiesDead.Contains(_enemiesInBattle[i])) _enemiesActionsTurn++;
                _enemiesInBattle[i].DoDamage();
                yield return new WaitUntil(() => _enemiesActionsTurn >= i);
            }
            _enemiesActionsTurn = 0;
            ChangeTurn();
            yield return null;
        }

        public void ActualEnemyDied(int expGained, int goldGained)
        {
            _enemiesDead.Add(_enemiesInBattle[_enemiesActionsTurn]);
            _enemiesActionsTurn++;
            StatsManager.Instance.AddExperience(expGained);
            StatsManager.Instance.AddGold(goldGained);
        }
        public void PlayerDied()
        {
            _playerIsDead = true;
            StopCoroutine(EnemyTurn());
        }
        public void DamagePlayer(int damage)
        {
            _playerCombat.ReciveAttack(damage);
        }

        private IEnumerator PlayerTurn()
        {
            _playerTurnInProgress = true;
            _combatUpdater.EnableMenuCombatActionPlayer();
            yield return null;
        }

        public void DamageEnemy(int index)
        {
            if (_enemiesDead.Contains(_enemiesInBattle[index])) return;
            _actualEnemy = _enemiesInBattle[index];
            _playerCombat.DoAttack();
        }
        
        public void DealDamageToEnemy(int damage, TypeEffect te, int turns)
        {
            _actualEnemy.RecieveAttack(damage, te, turns);
        }

        public void PlayerAttackEnded()
        {
            _playerTurnInProgress = false;
            ChangeTurn();
        }

        private IEnumerator BattleEnded()
        {
            _expGained = Mathf.RoundToInt(_expGained * _playerCombat.LuckAfterBattle);
            _goldGained = Mathf.RoundToInt(_goldGained * _playerCombat.LuckAfterBattle);
            if (_playerIsDead)
            {
                _expGained = Mathf.RoundToInt(_expGained * 0.5f);
                _goldGained = Mathf.RoundToInt(_goldGained * 0.5f);
            }
            if (_bossBattle == true)
            {
                //Dar objeto final si no termino el juego y sacar del mapa
                //cambiar contexto a juego y mostrando panel recompensas
            }
            _turnInProgress = false;
            _playerIsDead = false;
            _playerTurnInProgress = false;
            _enemiesActionsTurn = 0;
            _combatUpdater.ResetSlots();
            _enemiesInBattle.Clear();
            _enemiesDead.Clear();
            _bossBattle = false;
            //avisar a DungeonManager para que sepa que X enemigos han muerto
            //y si estan todos muertos que instancie al boss
            GameManager.Instance.SwapContextToDungeonFromCombat();
            yield return null;
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

