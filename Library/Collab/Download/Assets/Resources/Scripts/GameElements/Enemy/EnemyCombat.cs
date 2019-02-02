using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LimoKnight
{

    //This class will be sitting everytime in a gameobject (a button) on the combat panel
    //when the battle starts theinitialization ocurrs, this means, anycomponent used here will be preload in this hidden panel
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Button))]
    public class EnemyCombat : MonoBehaviour 
    {
        private string _name;
        private TypeEnemy _typeEnemy;
        private int _lifePoints;
        private int _actualLifePoints;
        private int _damage;
        private int _damageReduction;
        private int _criticChance;
        private int _experienceGiven;
        private int _goldGiven;

        private Sprite _sprite;
        private Animator _animator;
        private bool _isDoingAnimation = false;
        private TypeEffect _typeEffect = TypeEffect.None;
        private int _affectedFor = 0;


        public int ActualLifePoints
        {
            get { return _actualLifePoints; }
        }

        public string Name
        {
            get { return _name; }
        }

        public TypeEffect TypeEffect
        {
            get { return _typeEffect; }
        }

        public Sprite Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }
        public void StartAnimation()
        {
            _isDoingAnimation = true;
        }
        public void FinishAnimation()
        {
            _isDoingAnimation = false;
        }
        public void SetEnemyData(EnemyData enemyData, int level)
        {
            if (enemyData == null) return;
            _name = enemyData.Name;
            _typeEnemy = enemyData.TypeEnemy;
            _lifePoints = enemyData.Stats.Constitution + Mathf.RoundToInt(enemyData.Stats.Constitution * enemyData.IncreaseByLevel[0] * level);
            _actualLifePoints = _lifePoints;
            _damage = enemyData.Stats.Strength + Mathf.RoundToInt(enemyData.Stats.Strength * enemyData.IncreaseByLevel[1]);
            _damageReduction = Mathf.RoundToInt(Mathf.Clamp(enemyData.Stats.Defense + Mathf.RoundToInt(enemyData.Stats.Defense * enemyData.IncreaseByLevel[2]), 0f, 45f));
            _criticChance = Mathf.RoundToInt(Mathf.Clamp(enemyData.Stats.Luck + Mathf.RoundToInt(enemyData.Stats.Luck * enemyData.IncreaseByLevel[3]), 0f, 50f));
            _animator = gameObject.GetComponent<Animator>();
            _animator.runtimeAnimatorController = enemyData.AnimatorController;
            _sprite = Resources.Load(ControlManager.Instance.PathToEnemySprites + enemyData.SpriteName, typeof(Sprite)) as Sprite;
            gameObject.GetComponent<Image>().sprite = _sprite;
            gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 1f);
        }
   

        public void DoDamage()
        {
            Debug.Log("Enemy is going to attack:");
            if (_affectedFor > 0 || _typeEffect == TypeEffect.None)
            {
                NegativeEffectProcess();
            }
            else
            {
                Debug.Log("Is not going to have any effect so he attacks");
                StartCoroutine(DoDamageCoroutine());
            }
        }

        private void NegativeEffectProcess()
        {
            _affectedFor--;
            switch (_typeEffect)
            {
                //Stun corta el turno pero no daña
                case TypeEffect.Stun:
                    {
                        CombatTurnManager.Instance.EnemyActionEnded();
                        break;
                    }
                //Hielo hace el 20% del daño de la vida actual y corta el turno
                case TypeEffect.Freeze:
                    {
                        TakeDamage(Mathf.RoundToInt(_lifePoints * 0.2f));
                        break;
                    }
                //Hace daño (el 15%) pero no corta el turno
                case TypeEffect.Fire:
                    {
                        TakeDamageAndAttack(Mathf.RoundToInt(_lifePoints * 0.15f));
                        break;
                    }
            }
        }
        public void TakeDamage(int damage)
        {
            _actualLifePoints -= Mathf.RoundToInt(damage * (_damageReduction / 100f));
            if (_lifePoints <= 0)
            {
                StartCoroutine(DieCoroutine());
            }
            else
            {
                StartCoroutine(TakeDamageOnlyCoroutine());
            }
        }

        private IEnumerator DieCoroutine()
        {

            yield return new WaitUntil(() => _isDoingAnimation == false);
            Debug.LogError("ENEMY ANIMATOR > DIE MUST TRIGGER - START&END ANIMATION");
            _animator.Play("Die");
            yield return new WaitUntil(() => _isDoingAnimation == false);
            QuestManager.Instance.InformEliminationQuest((int)_typeEnemy);
            CombatTurnManager.Instance.ActualEnemyDied(_experienceGiven, _goldGiven);
        }
        private IEnumerator TakeDamageOnlyCoroutine()
        {

            yield return new WaitUntil(() => _isDoingAnimation == false);
            Debug.LogError("ENEMY ANIMATOR > TAKE DAMAGE MUST TRIGGER - START&END ANIMATION");
            _animator.Play("TakeDamage");
            yield return new WaitUntil(() => _isDoingAnimation == false);
            CombatTurnManager.Instance.EnemyActionEnded();
        }
        private void TakeDamageAndAttack(int damage)
        {
            _actualLifePoints -= Mathf.RoundToInt(damage * (_damageReduction / 100f));
            if (_lifePoints <= 0)
            {
                StartCoroutine(DieCoroutine());
            }
            else
            {
                StartCoroutine(TakeDamageAndAttackCoroutine());
            }
        }
        private IEnumerator TakeDamageAndAttackCoroutine()
        {
            yield return new WaitUntil(() => _isDoingAnimation == false);
            Debug.LogError("ENEMY ANIMATOR > TAKE DAMAGE MUST TRIGGER - START&END ANIMATION");
            _animator.Play("TakeDamage");
            yield return new WaitUntil(() => _isDoingAnimation == false);
            StartCoroutine(DoDamageCoroutine());
        }
        private IEnumerator DoDamageCoroutine()
        {
            int damage = _damage;
            bool isCritic = (Random.Range(0, 100) < _criticChance);
            if (isCritic)
                damage = damage + Mathf.RoundToInt(damage * Random.Range(0f, 1f));
            yield return new WaitUntil(() => _isDoingAnimation == false);
            Debug.LogError("ENEMY ANIMATOR > ATTACK MUST TRIGGER - START&END ANIMATION");
            _animator.Play("Attack");
            yield return new WaitUntil(() => _isDoingAnimation == false);
            CombatTurnManager.Instance.DamagePlayer(damage);
        }

        public void RecieveAttack(int damage, TypeEffect typeEffect, int turns)
        {
            _actualLifePoints = _actualLifePoints - Mathf.RoundToInt((damage / (float)_damageReduction));
            if (_actualLifePoints <= 0)
            {
                _actualLifePoints = 0;
                StartCoroutine(DieCoroutine());
            }
            else
            {
                _typeEffect = typeEffect;
                _affectedFor = turns;
                StartCoroutine(RecieveAttackCoroutine());
            }
        }

        private IEnumerator RecieveAttackCoroutine()
        {
            yield return new WaitUntil(() => _isDoingAnimation == false);
            _animator.Play("TakeDamage");
            yield return new WaitUntil(() => _isDoingAnimation == false);
            CombatTurnManager.Instance.PlayerAttackEnded();
        }
        

        public string LifeString()
        {
            return "<color=aa1010>"+_actualLifePoints.ToString() + "</color>/" + _lifePoints.ToString() + "hp";
        }
    }
    



}

