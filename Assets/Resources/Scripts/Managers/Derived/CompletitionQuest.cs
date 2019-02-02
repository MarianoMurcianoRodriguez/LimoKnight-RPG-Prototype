using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{


    [CreateAssetMenu(menuName = "Quest/CompletitionQuest")]
    public class CompletitionQuest : Quest
    {
        [SerializeField] private List<Objective> _objectives = new List<Objective>();
        [SerializeField] private int _objectivesCompleted = 0;

        public List<Objective> Objectives
        {
            get { return _objectives; }
            set { if (value != null) _objectives = value; }
        }
        public int ObjectivesCompleted
        {
            get { return _objectivesCompleted; }
            set { _objectivesCompleted = value; }
        }

        //Method called only when the player starts a new game to know what paremetres must be 
        //restarted to its default values
        public override void ResetMissionStatus()
        {
            _objectivesCompleted = 0;
            this.IsActiveNow = false;
            this.IsAvailableNow = false;
            this.CompletedByUser = false;
            if (this.IsAvailableAtStart) this.IsAvailableNow = true;
            foreach (Objective o in _objectives)
                o.IsCompleted = false;
        }

        //QuestManager will recive a call like "Mission-With-Id-X, ObjectiveX" to fullfill one of this
        public override bool UpdateMissionStatus(int identifierObjective)
        {
            bool aux = false;
            if (_objectivesCompleted == _objectives.Count) return aux; //false because no objective was accomplished
            foreach (Objective o in _objectives)
            {
                if (o.IDObjective == identifierObjective)
                {
                    _objectivesCompleted++;
                    o.IsCompleted = true;
                    aux = true;
                }
            }
            if (_objectivesCompleted == _objectives.Count) this.CompletedByUser = true;
            return aux;
        }

        /// <summary>
        /// Method to create copies passed without the reference so we can modified them
        /// in runtime without touching the original scriptableObject
        /// </summary
        public override void AssignValuesFromCode(Quest toCopy)
        {
            if (!(toCopy is CompletitionQuest)) return;
            base.AssignValuesFromCode(toCopy);
            CompletitionQuest toCopyCompletition = toCopy as CompletitionQuest;
            int auxObjectivesCompleted = toCopyCompletition.ObjectivesCompleted;
            List<Objective> auxObjectives = new List<Objective>();
            for (int i = 0; i < toCopyCompletition.Objectives.Count; i++)
                auxObjectives.Add(toCopyCompletition.Objectives[i]);
            Objectives = auxObjectives;
            ObjectivesCompleted = auxObjectivesCompleted;
        }

    }

    [System.Serializable]
    public class Objective
    {
        [SerializeField] private string _nameObjective;
        [SerializeField] private int _indentifierObjective;
        [SerializeField] private bool _isCompleted = false;

        public string NameObjective
        {
            get { return _nameObjective; }
            set { _nameObjective = value; }
        }

        public int IDObjective
        {
            get { return _indentifierObjective; }
            set { _indentifierObjective = value; }
        }

        public bool IsCompleted
        {
            get { return _isCompleted; }
            set { _isCompleted = value; }
        }
    }
}