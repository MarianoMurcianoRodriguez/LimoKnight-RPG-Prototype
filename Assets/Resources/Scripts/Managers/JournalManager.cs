using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LimoKnight
{
    public class Journal : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private string _from;
        [SerializeField] private string _to;
        [SerializeField] private int _id = -1;
        [SerializeField] private int _nextJournalID = -1;
        [SerializeField] private bool _inPosession = false;
        [SerializeField] private GameObject _gameobjectTrigger;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; } 
            set { _description = value; }
        }

        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        public string To
        {
            get { return _to; }
            set { _to = value; }
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public int NextJournalID
        {
            get { return _nextJournalID; }
            set { _nextJournalID = value; }
        }

        public bool InPossesion
        {
            get { return _inPosession; }
            set { _inPosession = value; }
        }

        public GameObject GameObjectTrigger
        {
            get { return _gameobjectTrigger; }
            set { _gameobjectTrigger = value; }
        }
    }
    
    public class JournalManager : Singleton<JournalManager>
    {
        private Journal[] _allJournals;
        private List<int> _journalsUnlocked = new List<int>();

        private void Start()
        {
            Journal[] journals = Resources.LoadAll<Journal>(ControlManager.Instance.PathToJournalScriptableObjects);
            _allJournals = new Journal[journals.Length];
            for (int i = 0; i < journals.Length; i++)
            {
                if (journals[i].InPossesion) _journalsUnlocked.Add(journals[i].ID);
                _allJournals[journals[i].ID] = journals[i];
            }
        }

        public void UnlockJournalID(int ID)
        {
            _journalsUnlocked.Add(ID);
        }

        public void SaveUnlockedJournals()
        {
            StatsManager.Instance.AddJournalsToInfoPlayer(_journalsUnlocked);
            foreach (int i in _journalsUnlocked)
            {
                _allJournals[i].InPossesion = true;
            }
        }
        
        public void ResetUnlockedJournals()
        {
            foreach (Journal j in _allJournals)
            {
                j.InPossesion = false;
            }
        }
    }
}
    


