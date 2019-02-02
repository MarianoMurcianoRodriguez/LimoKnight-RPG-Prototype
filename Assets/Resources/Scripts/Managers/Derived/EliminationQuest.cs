using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{

    [CreateAssetMenu(menuName = "Quest/EliminationQuest")]
    public class EliminationQuest : Quest
    {
        [SerializeField] private List<TypeEnemy> _enemyTypes = new List<TypeEnemy>();
        [SerializeField] private List<int> _enemyQuantity = new List<int>();
        [SerializeField] private List<int> _enemiesDeleted = new List<int>();
        [Range(0,0)] [Tooltip("This atribute shows how many sub-missions have been realized")]
        [SerializeField] private int subChecks = 0;

        public List<TypeEnemy> EnemyTypes
        {
            get { return _enemyTypes; }
            set { if (value != null) _enemyTypes = value; }
        }
        public List<int> EnemyQuantity
        {
            get { return _enemyQuantity; }
            set { if (value != null) _enemyQuantity = value; }
        }
        public List<int> EnemiesDeleted
        {
            get { return _enemiesDeleted; }
            set { _enemiesDeleted = value; }
        }
        


        //Method called only when the player starts a new game to know what paremetres must be 
        //restarted to its default values
        public override void ResetMissionStatus()
        {
            for (int i = 0; i < _enemiesDeleted.Count; i++)
            {
                _enemiesDeleted[i] = 0;
            }
            this.IsActiveNow = false;
            this.IsAvailableNow = false;
            this.CompletedByUser = false;
            if (this.IsAvailableAtStart) this.IsAvailableNow = true;
            this.subChecks = 0;
        }

        //same as CompletitionQuest-UpdateMissionsStatus
        public override bool UpdateMissionStatus(int enemyType)
        {
            int index = 0;
            foreach (TypeEnemy enemyID in _enemyTypes)
            {
                if ((int)enemyID == enemyType) break;
                else index++;
            }
            if (_enemiesDeleted[index] == _enemyQuantity[index]) return false;
            _enemiesDeleted[index]++;
            if (_enemiesDeleted[index] == _enemyQuantity[index])
            {
                subChecks++;
                if (subChecks==_enemyTypes.Count) this.CompletedByUser = true;
                return true;
            }
            else return false;
        }

        public string EliminationQuestDescription(int enemyType)
        {
            int index = 0;
            foreach (TypeEnemy enemyID in _enemyTypes)
            {
                if ((int)enemyID == enemyType) break;
                else index++;
            }
            return "<color=#107710>-Kill " + _enemyQuantity[index].ToString() + " " + _enemyTypes[index].ToString() + ".</color>";
        }

        /// <summary>
        /// Method to create copies passed without the reference so we can modified them
        /// in runtime without touching the original scriptableObject
        /// </summary
        public override void AssignValuesFromCode(Quest toCopy)
        {
            if (!(toCopy is EliminationQuest)) return;
            base.AssignValuesFromCode(toCopy);
            EliminationQuest toCopyElimination = toCopy as EliminationQuest;
            this.EnemyTypes = toCopyElimination.EnemyTypes;
            this.EnemyQuantity = toCopyElimination.EnemyQuantity;
            List<int> aux = new List<int>();
            for (int i = 0; i < toCopyElimination.EnemiesDeleted.Count; i++)
                aux.Add(toCopyElimination.EnemiesDeleted[i]);
            this.EnemiesDeleted = aux;
            subChecks = toCopyElimination.subChecks;
        }
    }
}