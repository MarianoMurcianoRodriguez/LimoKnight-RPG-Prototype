using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LimoKnight
{
    public enum ActionPlayer
    {
        None,
        Attack,
        Investigate
    }

    public class CombatUpdater: MonoBehaviour
    {
        [Tooltip("This Gameobjects need an empty animator plus an botton, plus enemyCombat")]
        [SerializeField] private GameObject[] _enemiesSlots;
        [Tooltip("Same, but with playerCombat")]
        [SerializeField] private GameObject _playerSlot;
        [SerializeField] private Text _nameEnemyInvestigate;
        [SerializeField] private Text _actualLifeEnemyInvestigate;
        [SerializeField] private Text _StatusEnemyInvestigate;
        [SerializeField]private Animator _animatorCombatUpdater;

        private bool _isDoingAnimation = false;
        private ActionPlayer _actionPlayer;
        private bool _investigationPanelIsShown = false;
        private bool _attackWasDone = false;

        public void FinishAnimation()
        {
            _isDoingAnimation = false;
        }
        public void StartAnimation()
        {
            _isDoingAnimation = true;
        }
        public GameObject[] EnemiesSlots
        {
            get { return _enemiesSlots; }
            set { _enemiesSlots = value; }
        }
        public PlayerCombat PlayerCombat
        {
            get { return _playerSlot.GetComponent<PlayerCombat>(); }
        }
        public void ResetSlots()
        {
            for (int i=0; i<4;i++)
            {
                _enemiesSlots[i].GetComponent<Image>().sprite = null;
                _enemiesSlots[i].GetComponent<Animator>().runtimeAnimatorController = null;
                _enemiesSlots[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
            }
           
        }

        public void SetActionPlayerToAttack()
        {
            if (_attackWasDone) return;
            _actionPlayer = ActionPlayer.Attack;
        }
        public void SetActionPlayerToInvestigate()
        {
            if (_attackWasDone) return;
            _actionPlayer = ActionPlayer.Investigate;
        }

        public void EnableMenuCombatActionPlayer()
        {
            _animatorCombatUpdater.Play("ShowPanelCombatPlayer");
        }
        
        public void SelectEnemy(int index)
        {
            if (_actionPlayer == ActionPlayer.None) return;
            else if (_actionPlayer == ActionPlayer.Investigate)
                EnableMenuInvestigationPlayer(index);
            else if (_actionPlayer == ActionPlayer.Attack && CombatTurnManager.Instance.
                EnemiesAlive[index]==true)
            {
                _attackWasDone = true;
                DisableMenuInvestigationPlayerAndCombat(index);
            }

            _actionPlayer = ActionPlayer.None;
        }
        
        private void EnableMenuInvestigationPlayer(int index)
        {
            if (_investigationPanelIsShown) DisableMenuInvestigationPlayer();
            EnemyCombat ed = CombatTurnManager.Instance.EnemiesInCombat[index];
            _actualLifeEnemyInvestigate.text = ed.LifeString();
            _nameEnemyInvestigate.text = "<b>"+ed.Name+"</b>";
            switch (ed.TypeEffect)
            {
                case TypeEffect.Stun:
                    {
                        _StatusEnemyInvestigate.text = "STUNNED!";
                        break;
                    }
                case TypeEffect.Freeze:
                    {
                        _StatusEnemyInvestigate.text = "FROZEN!";
                        break;
                    }
                case TypeEffect.Fire:
                    {
                        _StatusEnemyInvestigate.text = "BURNT!";
                        break;
                    }
                case TypeEffect.None:
                    {
                        _StatusEnemyInvestigate.text = "None";
                        break;
                    }
            }
            StartCoroutine(EnableInvestigationPanel());
        }

        private IEnumerator EnableInvestigationPanel()
        {

            yield return new WaitUntil(() => _isDoingAnimation == false);
            _investigationPanelIsShown = true;
            _animatorCombatUpdater.Play("ShowPanelInvestigationPlayer");
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _isDoingAnimation == false);
        }

        public void DisableMenuInvestigationPlayer()
        {
            StartCoroutine(DisableMenuInvestigationPlayerCoroutine());
        }

        private IEnumerator DisableMenuInvestigationPlayerCoroutine()
        {
            yield return new WaitUntil(() => _isDoingAnimation == false);
            _investigationPanelIsShown = false;
            _animatorCombatUpdater.Play("HidePanelInvestigationPlayer");
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _isDoingAnimation == false);
        }

        public void DisableMenuInvestigationPlayerAndCombat(int index)
        {
            StartCoroutine(DisableMenuInvestigationaPlayerAndCombatCoroutine(index));
        }

        private IEnumerator DisableMenuInvestigationaPlayerAndCombatCoroutine(int index)
        {
            if (_investigationPanelIsShown)
            {
                yield return new WaitUntil(() => _isDoingAnimation == false);
                _investigationPanelIsShown = false;
                _animatorCombatUpdater.Play("HidePanelInvestigationPlayer");
                yield return new WaitForEndOfFrame();
                yield return new WaitUntil(() => _isDoingAnimation == false);
            }
            yield return new WaitUntil(() => _isDoingAnimation == false);
            _animatorCombatUpdater.Play("HidePanelCombatPlayer");
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _isDoingAnimation == false);
            CombatTurnManager.Instance.DamageEnemy(index);
            _attackWasDone = false;
        }

        public void DisableMenuCombat()
        {
            StartCoroutine(DisableMenuCombatCoroutine());
        }

        private IEnumerator DisableMenuCombatCoroutine()
        {
            yield return new WaitUntil(() => _isDoingAnimation == false);
            _animatorCombatUpdater.Play("HidePanelCombatPlayer");
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _isDoingAnimation == false);
            _attackWasDone = false;
            _actionPlayer = ActionPlayer.None;
        }

    }
}