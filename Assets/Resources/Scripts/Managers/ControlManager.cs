using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{

    public enum PlayerContextType
    {
        OutsideGame = -1,
        InNormalWord = 0,
        InDungeon = 1,
        InCombat = 2,
        InMinigame = 3,
        InPauseMenu = 4,
    }

    //this will let us control if we should show or not another UI Panel (for example, we do not want to see the inventory if we're not inGame
    public enum GameContext
    {
        InMainMenus = 0,
        InGame = 1,
        InExclusivePanelMode = 2,
        InMinigame = 3
    }

   


    /// <summary>This class will story info about the general control (this could be: information in run-time
    /// or keep track of information that will esential to know if a player should have its movement available
    /// or general variables to the proyect itself</summary>
    public class ControlManager : Singleton<ControlManager>
    {
        public GameObject Player;
        public GameObject MapHolder;
        public GameObject DungeonHolder;
        public PlayerContextType PlayerContext = PlayerContextType.OutsideGame;
        public GameContext GameContext = GameContext.InMainMenus;

        public string PathToWeaponModelsFolder {
            get
            {
                return "Prefabs/GameElements/3DModels";
            }
        }
        public string PathToDungeonTiles
        {
            get
            {
                return "Prefabs/GameElements/DungeonTiles";
            }
        }
        public string PathToSaveGame
        {
            get
            {
                return Application.persistentDataPath + "/SaveGame";
            }
        }
        public string PathToDefaultSaveValue
        {
            get
            {
                return Application.dataPath + "/StreamingAssets/DefaultValues";
            }
        }

        public string PathToInventorySprites
        {
            get
            {
                return "Sprites/Inventory";
            }
        }

        public string PathToUISprites
        {
            get { return "Sprites/GeneralUI"; }
        }
        
        public string PathToPlayerSprites
        {
            get { return "Sprites/Player"; }
        }

        public string PathToEnemySprites
        {
            get { return "Sprites/Enemy"; }
        }

        public string PathToQuestScriptableObjects
        {
            get { return "ScriptableObjects/Quests/"; }
        }

        public string PathToEnemyScriptableObjects
        {
            get { return "ScriptableObjects/EnemyData/"; }
        }

        public string PathToJournalScriptableObjects
        {
            get { return "ScriptableObjects/Journals/"; }
        }
        

    }


    

}
