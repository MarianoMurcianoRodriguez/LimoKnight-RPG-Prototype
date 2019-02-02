using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    /// <summary>This class is used to be called by any object in the game because they probably dont know how to
    /// do their purpose, the GameManage will contact other Managers if needed to fill this blank space:
    /// For example, the UIButton to start a new game, calls 'StartNewGame()' and if this is true, at somepoint
    /// this manager will call de UIManager </summary>
    public class GameManager : Singleton<GameManager> {

        [SerializeField] private TransitionPanel _transitionPanel;
        private Vector3 _lastPositionPlayerWorld;
        private Vector3 _lastPositionPlayerDungeon;

        public TransitionPanel TransitionPanel
        {
            get { return _transitionPanel; }
        }

        public void StartNewGame()
        {
            if (XMLManager.Instance.GameSaveExisted())
            {
                UIManager.Instance.ShowUISecondaryPanel(UISecondaryPanel.NewGameOverride);
            }
            else
            {
                XMLManager.Instance.CreateSaveFolder();
                XMLManager.Instance.LoadDefaultData();
                StartCoroutine(StartMainGameCoroutine());
            }
        }

        public void HideOverridePanel()
        {
            UIManager.Instance.HideUISecondaryPanel(UISecondaryPanel.NewGameOverride);
        }

        public void ResetDataAndStart()
        {
            XMLManager.Instance.ResetAllGameData();
            QuestManager.Instance.ResetMissionsToDefaultState();
            XMLManager.Instance.LoadDefaultData();
            StartCoroutine(StartMainGameCoroutine());
        }

        private IEnumerator StartMainGameCoroutine()
        {
            UIManager.Instance.TransitionPanelAnimator.Play("AnimateTransition");
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _transitionPanel.ShouldLoadNextPanel == true);
            UIManager.Instance.HideUIMainPanel(UIMainPanel.MainMenu);
            UIManager.Instance.HideUISecondaryPanel(UISecondaryPanel.NewGameOverride);
            ControlManager.Instance.MapHolder.SetActive(true);
            _lastPositionPlayerWorld = ControlManager.Instance.Player.transform.position;
            ControlManager.Instance.Player.SetActive(true);
            ControlManager.Instance.GameContext = GameContext.InGame;
            ControlManager.Instance.PlayerContext = PlayerContextType.InNormalWord;
            UIManager.Instance.ShowUIMainPanel(UIMainPanel.Character);
        }

        public void LoadGame()
        {
            XMLManager.Instance.LoadAllData();
            StartCoroutine(StartMainGameCoroutine());
        }

        public void SwapContextToCombatFromDungeon()
        {
            UIManager.Instance.TransitionPanelAnimator.Play("AnimateTransition");
            StartCoroutine(SwapContextToCombatFromDungeonCoroutine());
        }
        private IEnumerator SwapContextToCombatFromDungeonCoroutine()
        {
            yield return new WaitUntil(() => _transitionPanel.ShouldLoadNextPanel == true);
            ControlManager.Instance.Player.SetActive(false);
            ControlManager.Instance.DungeonHolder.SetActive(false);
            _lastPositionPlayerDungeon = ControlManager.Instance.Player.transform.position;
            CombatTurnManager.Instance.InitializeBattle();
        }

        public void SwapContextToDungeonFromWorld()
        {
            UIManager.Instance.TransitionPanelAnimator.Play("AnimateTransition");
            StartCoroutine(SwapContextToDungeonFromWorldCoroutine());

        }
        private IEnumerator SwapContextToDungeonFromWorldCoroutine()
        {

            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _transitionPanel.ShouldLoadNextPanel == true);
            ControlManager.Instance.MapHolder.SetActive(false);
            ControlManager.Instance.Player.SetActive(false);
            ControlManager.Instance.DungeonHolder.SetActive(true);
            ControlManager.Instance.PlayerContext = PlayerContextType.InDungeon;
            this._lastPositionPlayerWorld = ControlManager.Instance.Player.transform.position;
            DungeonManager.Instance.GenerateLevel();
            yield return new WaitUntil(() => DungeonManager.Instance.GenerationHasStartedAndEnded() == true);
            ControlManager.Instance.Player.GetComponent<PlayerMovement>().MovementSpeed = 5f;
            ControlManager.Instance.Player.SetActive(true);
        }

        public void SwapContextToDungeonFromCombat()
        {
            UIManager.Instance.TransitionPanelAnimator.Play("AnimateTransition");
            StartCoroutine(SwapContextToDungeonFromCombatCoroutine());
            
        }
        private IEnumerator SwapContextToDungeonFromCombatCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _transitionPanel.ShouldLoadNextPanel == true);
            UIManager.Instance.HideUIMainPanel(UIMainPanel.Combat);
            ControlManager.Instance.DungeonHolder.SetActive(true);
            ControlManager.Instance.PlayerContext = PlayerContextType.InDungeon;
            ControlManager.Instance.Player.transform.position = _lastPositionPlayerDungeon;
            ControlManager.Instance.Player.SetActive(true);
            ControlManager.Instance.Player.GetComponent<PlayerMovement>().enabled = true;
            ControlManager.Instance.Player.GetComponent<PlayerMovement>().MovementSpeed = 5f;
        }

        public void SwapContextToWorldFromCombatOrDungeon()
        {
            UIManager.Instance.TransitionPanelAnimator.Play("AnimateTransition");
            StartCoroutine(SwapContextToWorldFromCombatOrDungeonCoroutine());
        }

        private IEnumerator SwapContextToWorldFromCombatOrDungeonCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _transitionPanel.ShouldLoadNextPanel == true);
            UIManager.Instance.HideUIMainPanel(UIMainPanel.Combat);
            ControlManager.Instance.DungeonHolder.SetActive(false);
            ControlManager.Instance.MapHolder.SetActive(true);
            ControlManager.Instance.PlayerContext = PlayerContextType.InNormalWord;
            ControlManager.Instance.Player.transform.position = _lastPositionPlayerWorld;
            ControlManager.Instance.Player.SetActive(true);
            ControlManager.Instance.Player.GetComponent<PlayerMovement>().enabled = true;
            ControlManager.Instance.Player.GetComponent<PlayerMovement>().MovementSpeed = 10f;
        }

        public void SwapContextToDungeonFromDungeon()
        {
            UIManager.Instance.TransitionPanelAnimator.Play("AnimateTransition");
            StartCoroutine(SwapContextToDungeonFromDungeonCoroutine());
        }

        private IEnumerator SwapContextToDungeonFromDungeonCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _transitionPanel.ShouldLoadNextPanel == true);
            UIManager.Instance.HideUISecondaryPanel(UISecondaryPanel.KeepFightingDungeon);
            ControlManager.Instance.Player.SetActive(false);
            ControlManager.Instance.PlayerContext = PlayerContextType.InDungeon;
            DungeonManager.Instance.LoadEnemiesOnLevel();
            yield return new WaitForSeconds(2f);
            ControlManager.Instance.Player.GetComponent<PlayerMovement>().MovementSpeed = 5f;
            ControlManager.Instance.Player.SetActive(true);
        }

        public void SwapContextToMinigameFromWorld()
        {
            UIManager.Instance.TransitionPanelAnimator.Play("AnimateTransition");
            StartCoroutine(SwapContextToMinigameFromWorldCoroutine());
        }
        
        private IEnumerator SwapContextToMinigameFromWorldCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _transitionPanel.ShouldLoadNextPanel == true);
            ControlManager.Instance.PlayerContext = PlayerContextType.InMinigame;
            UIManager.Instance.ShowUIMainPanel(UIMainPanel.SlotMachine);
            ControlManager.Instance.MapHolder.SetActive(false);
            this._lastPositionPlayerWorld = ControlManager.Instance.Player.transform.position;
            ControlManager.Instance.Player.SetActive(false);
        }

        public void SwapContextToWorldFromMinigame()
        {
            UIManager.Instance.TransitionPanelAnimator.Play("AnimateTransition");
            StartCoroutine(SwapContextToWorldFromMinigameCoroutine());
        }

        private IEnumerator SwapContextToWorldFromMinigameCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => _transitionPanel.ShouldLoadNextPanel == true);
            UIManager.Instance.HideUIMainPanel(UIMainPanel.SlotMachine);
            ControlManager.Instance.PlayerContext = PlayerContextType.InMinigame;
            ControlManager.Instance.MapHolder.SetActive(true);
            ControlManager.Instance.Player.transform.position = _lastPositionPlayerWorld;
            ControlManager.Instance.Player.SetActive(true);
        }

        public void SetPositionPlayerInDungeon(Vector3 position)
        {
            ControlManager.Instance.Player.transform.position = position;
            _lastPositionPlayerDungeon = position;
        }

        public void QuitGameWithoutSave()
        {
            Application.Quit();
        }
        

        public void SaveGame()
        {
            XMLManager.Instance.SaveAllData();
        }

        public void ShouldPlayerMove(bool shouldMove)
        {
           ControlManager.Instance.Player.GetComponent<PlayerMovement>().enabled = shouldMove;
        }

    }
}
