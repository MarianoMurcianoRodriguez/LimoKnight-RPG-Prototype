using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LimoKnight
{

    //This class will be sitting everytime in a gameobject (a button) on the combat panel
    //when the battle starts theinitialization ocurrs with (actualstats and infoweapon and sprite) from
    //the one equipped, thie means, anycomponent used here will be preload in this hidden panel
    //
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Image))]
    public class PlayerCombat : MonoBehaviour
    {
        private Animator _animator;
        private Sprite _spriteFull;
        private int _maxHP;
        private int _actualHP =-1;
        private int _damage;
        private int _damageReduction;
        private int _criticalChance;
        private float _luckAfterBattle;
        private WeaponEffect _weaponEffect;
        private float _damageWeapon = -1f;
       
        private bool _isDoingAnimation = false;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }
        public Sprite SpriteFull
        {
            get { return _spriteFull; }
            set
            {
                _spriteFull = value;
                this.gameObject.GetComponent<Image>().sprite = _spriteFull;
                this.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 1f);
            }
        }
        public float LuckAfterBattle
        {
            get { return _luckAfterBattle; }
        }
        public void SetActualStats(int actualHp, int maxHp, int damage, int damageReduction, int criticalChance, float luckAfterBattle)
        {
            _actualHP = actualHp;  _maxHP = maxHp; _damage = damage; _damageReduction = damageReduction; _criticalChance = criticalChance; _luckAfterBattle = luckAfterBattle;
            if (_maxHP <= 0) return;
            StatsManager.Instance.RefreshHP(_actualHP, _maxHP);
        }
        public void SetInfoWeapon(InfoWeapon infoWeapon)
        {
                _damageWeapon = infoWeapon.Damage;
                _weaponEffect = infoWeapon.WeaponEffect;
        }
        
        /// EVENTS CALLED BY HIS ANIMATOR
        public void FinishAnimation()
        {
            _isDoingAnimation = false;
        }
        public void StartAnimation()
        {
            _isDoingAnimation = true;
        }


        public void ReciveAttack(int damage) {
            int damageTaken = damage - Mathf.RoundToInt(damage * (_damageReduction / 100f));
            _actualHP = _actualHP - damageTaken;
            if (_actualHP <= 0)
            {
                _actualHP = 0;
                StartCoroutine(DieCoroutine());
            } else
            {
                StartCoroutine(RecieveAttackCoroutine());
            }
        }

        private IEnumerator DieCoroutine()
        {
            yield return new WaitUntil(() => _isDoingAnimation == false);
            _animator.Play("Die");
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _isDoingAnimation == false);
            StatsManager.Instance.RefreshHP(_actualHP, _maxHP);
            CombatTurnManager.Instance.PlayerDied();
        }
        private IEnumerator RecieveAttackCoroutine()
        {
            yield return new WaitUntil(() => _isDoingAnimation == false);
            _animator.Play("RecieveAttack");
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _isDoingAnimation == false);
            StatsManager.Instance.RefreshHP(_actualHP, _maxHP);
            CombatTurnManager.Instance.EnemyActionEnded();
        }
        
        public void DoAttack()
        {
            StartCoroutine(DoAttackCoroutine());
        }

        private IEnumerator DoAttackCoroutine()
        {
            int damage = _damage + Mathf.RoundToInt(_damageWeapon);
            TypeEffect te = TypeEffect.None;
            int turns = 0;
            if (Random.Range(0, 100) <= _criticalChance)
                damage = damage * 2;
            if (_weaponEffect.TypeEffect != TypeEffect.None)
                if (Random.Range(0, 100) <= _weaponEffect.Probability)
                {
                    te = _weaponEffect.TypeEffect;
                    turns = _weaponEffect.Time;
                }
            yield return new WaitUntil(() => _isDoingAnimation == false);
            _animator.Play("Attack");
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _isDoingAnimation == false);
            CombatTurnManager.Instance.DealDamageToEnemy(damage, te, turns);
        }
        
    }
}


