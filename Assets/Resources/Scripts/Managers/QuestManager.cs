using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public class QuestManager : Singleton<QuestManager>
    {
        [SerializeField] private QuestPanel _questPanel;
        [SerializeField] private GameObject _questUpdatePanel;
        //And here they will be cloned and modified, if the player saves the game
        //we substitute the previous with this one.
        [SerializeField] private List<Quest> _actualQuestPlayer = new List<Quest>();
        [SerializeField] private List<Quest> _endedActualQuestPlayer = new List<Quest>();
        private Quest[] _allQuests;
        private int _questsCompleted;

        private QuestUpdater _questUpdater;

        public QuestPanel QuestPanel
        {
            get { return _questPanel; }
        }

        public bool IsQuestNotCompleted(int idQuest, int idObjective)
        {
            
            bool cond = false;
            if (_actualQuestPlayer.Count == 0) return false;
            for (int i=0; i<_actualQuestPlayer.Count; i++)
            {
                if (_actualQuestPlayer[i].ID == idQuest)
                {
                    CompletitionQuest cq = _actualQuestPlayer[i] as CompletitionQuest;
                    cq.Objectives[idObjective].IsCompleted = true;
                    cond = true;    
                }
            }
            return cond;
        }

        private void Start()
        {
            Quest[] quests = Resources.LoadAll<Quest>(ControlManager.Instance.PathToQuestScriptableObjects);
            AssignQuestsFromSystemToPlayer(quests);
        }

        private void AssignQuestsFromSystemToPlayer(Quest[] quests)
        {

            _allQuests = new Quest[quests.Length];
            for (int i = 0; i < quests.Length; i++)
                _allQuests[quests[i].ID] = quests[i];
            for (int i = 0; i < _allQuests.Length; i++)
            {
                if (_allQuests[i].IsActiveNow)
                {
                    if (_allQuests[i] is EliminationQuest)
                    {
                        EliminationQuest eq = ScriptableObject.CreateInstance<EliminationQuest>();
                        eq.AssignValuesFromCode(_allQuests[i]);
                        _actualQuestPlayer.Add(eq);
                    }
                    else if (_allQuests[i] is CompletitionQuest)
                    {
                        CompletitionQuest cq = ScriptableObject.CreateInstance<CompletitionQuest>();
                        cq.AssignValuesFromCode(_allQuests[i]);
                        _actualQuestPlayer.Add(cq);
                    }
                }

                if (_allQuests[i].CompletedByUser)
                    _questsCompleted++;

            }
            _questUpdater = _questUpdatePanel.GetComponent<QuestUpdater>();
            _questPanel.SetAllQuest(_allQuests);
        }

        public void ResetMissionsToDefaultState()
        {
            Quest[] quests = Resources.LoadAll<Quest>(ControlManager.Instance.PathToQuestScriptableObjects);
            foreach (Quest q in quests)
            {
                q.ResetMissionStatus();
                _actualQuestPlayer.Clear();
                _endedActualQuestPlayer.Clear();
            }
            AssignQuestsFromSystemToPlayer(quests);

        }

        public void UpdateQuestPlayerData(List<int> missionsFromPlayer)
        {
            foreach (int i in missionsFromPlayer)
            {
                if (_allQuests[i].IsActiveNow)
                    if (_allQuests[i] is EliminationQuest)
                    {
                        EliminationQuest eq = ScriptableObject.CreateInstance<EliminationQuest>();
                        eq.AssignValuesFromCode(_allQuests[i]);
                        _actualQuestPlayer.Add(eq);
                    }
                    else if (_allQuests[i] is CompletitionQuest)
                    {
                        CompletitionQuest cq = ScriptableObject.CreateInstance<CompletitionQuest>();
                        cq.AssignValuesFromCode(_allQuests[i]);
                        _actualQuestPlayer.Add(cq);
                    }
            }
        }

        public void InformEliminationQuest(int typeEnemy)
        {
            if (_actualQuestPlayer.Count>0)
                foreach (Quest q in _actualQuestPlayer)
                {
                    if (q is EliminationQuest)
                    {
                        q.UpdateMissionStatus(typeEnemy);
                        if (q.CompletedByUser)
                        {
                            _questsCompleted++;
                            _questPanel.UpdateStatusQuest(q);
                            StatsManager.Instance.AddExperience(q.Rewards[1]);
                            StatsManager.Instance.AddGold(q.Rewards[0]);
                            DisplayCompletedQuest(q);
                            UpdateActualMissions(q);
                        }
                    }
                }
        }

        public void InformCompletitionQuest(int questID, int subID)
        {
            if (_actualQuestPlayer.Count>0)
                foreach (Quest q in _actualQuestPlayer)
                {
                    if (q.ID == questID)
                    {
                       q.UpdateMissionStatus(subID);
                       if (q.CompletedByUser)
                       {
                            _questsCompleted++;
                            _questPanel.UpdateStatusQuest(q);
                            StatsManager.Instance.AddExperience(q.Rewards[1]);
                            StatsManager.Instance.AddGold(q.Rewards[0]);
                            DisplayCompletedQuest(q);
                            UpdateActualMissions(q);
                       }
                       break;
                    }
                }
        }

        private void UpdateActualMissions(Quest q)
        {
            _endedActualQuestPlayer.Add(q);
            _actualQuestPlayer.Remove(q);
            if (q.NextMissionId >= 0)
            {
                Quest nextQuest = _allQuests[q.NextMissionId];
                Debug.Log("Siguiente: "+nextQuest.name);
                if (nextQuest is CompletitionQuest)
                {
                    CompletitionQuest toAdd = ScriptableObject.CreateInstance<CompletitionQuest>();
                    toAdd.AssignValuesFromCode(nextQuest);
                    toAdd.IsAvailableNow = true;
                    _actualQuestPlayer.Add(toAdd);
                    _questPanel.UpdateStatusQuest(toAdd);
                }
                else if (nextQuest is EliminationQuest)
                {
                    EliminationQuest toAdd = ScriptableObject.CreateInstance<EliminationQuest>();
                    toAdd.AssignValuesFromCode(nextQuest);
                    toAdd.IsAvailableNow = true;
                    _actualQuestPlayer.Add(toAdd);
                    _questPanel.UpdateStatusQuest(toAdd);
                }
            }
        }

        private void SaveMissions()
        {
            for (int i=0; i<_actualQuestPlayer.Count; i++)
                _allQuests[_actualQuestPlayer[i].ID].AssignValuesFromCode(_actualQuestPlayer[i]);
            for (int i = 0; i < _endedActualQuestPlayer.Count; i++)
                _allQuests[_endedActualQuestPlayer[i].ID].AssignValuesFromCode(_endedActualQuestPlayer[i]);
        }

        private void DisplayCompletedQuest(Quest q)
        {
            UIManager.Instance.ShowUISecondaryPanel(UISecondaryPanel.UpdateMission);
           _questUpdater.DisplayMissionUpdate(q.Name, "¡Gold and exp adquired: " 
                + q.Rewards[0] + "G y " + q.Rewards[1]+"XP !");
        }
        
        private void DisplayCompletedObjective(Quest q, int subID)
        {
           if (q is EliminationQuest)
            {
                EliminationQuest eQ = q as EliminationQuest;
                UIManager.Instance.ShowUISecondaryPanel(UISecondaryPanel.UpdateMission);
                _questUpdater.DisplayMissionUpdate(eQ.Name, eQ.EliminationQuestDescription(subID));
            }
           else if (q is CompletitionQuest)
            {
                CompletitionQuest cQ = q as CompletitionQuest;
                UIManager.Instance.ShowUISecondaryPanel(UISecondaryPanel.UpdateMission);
                _questUpdater.DisplayMissionUpdate(cQ.Name, cQ.Objectives[subID].NameObjective);
            }
        }

        private void ResetMissionsStatus()
        {
            for (int i=0; i<_allQuests.Length; i++)
            {
                _allQuests[i].ResetMissionStatus();
            }
        }

        public Quest GetQuestToAccept(int ID)
        {
            foreach (Quest q in _allQuests)
            {
                if (q.ID == ID) {
                    return q;
                } 
            }
            return null;
            
        }

        public void SetAvailableQuestAsActive(int ID)
        {
            foreach (Quest q in _allQuests)
            {
                if (q.ID == ID)
                {
                    if (_allQuests[ID] is EliminationQuest)
                    {
                        EliminationQuest eq = ScriptableObject.CreateInstance<EliminationQuest>();
                        eq.AssignValuesFromCode(_allQuests[ID]);
                        _actualQuestPlayer.Add(eq);
                        eq.IsActiveNow = true;
                        eq.IsAvailableNow = false;
                        _questPanel.UpdateStatusQuest(eq);
                    }
                    else if (_allQuests[ID] is CompletitionQuest)
                    {
                        CompletitionQuest cq = ScriptableObject.CreateInstance<CompletitionQuest>();
                        cq.AssignValuesFromCode(_allQuests[ID]);
                        _actualQuestPlayer.Add(cq);
                        cq.IsActiveNow = true;
                        cq.IsAvailableNow = false;
                        _questPanel.UpdateStatusQuest(cq);
                    }
                }
            }
        }
        

    }
}