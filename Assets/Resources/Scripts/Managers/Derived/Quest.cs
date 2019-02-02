using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public enum TypeQuest
    {
        Elimination,
        Completition, 
    }

    public enum TypeReward
    {
        Gold, 
        Experience,
        Both,
    }

    
    public enum TypeEnemy
    {
        Headhog,
        Owl,
        Skeleton,
        EnemySize,              //used to do random(0, this) to pick a random enemy
        SkeletonKing = 100,     //boss size is the oposite we do (100, bossSize)
        BossSize
    }

  

    public abstract class Quest : ScriptableObject
    {
        [SerializeField] private int _identifier = -1;
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [Tooltip("Is the mission in current realization?")]
        [SerializeField] private bool _isActiveNow = false;
        [Tooltip("Did this quest get avaiable but not accepted for now by the player?")]
        [SerializeField] private bool _isAvailableNow = false;
        [Tooltip("Is the mission available at the start (did not require any event or" +
            "any mission completition to be added as active at the start?")]
        [SerializeField] private bool _isAvailableAtStart = false;
        [Tooltip("If the mission is ended, isActive will be false as such is not active anymore")]
        [SerializeField] private bool _completedByUser;
        [SerializeField] private TypeReward _typeReward;
        [SerializeField] private int[] _rewards = new int[2] { 0, 0 };
        [SerializeField] private int _nextMissionId = -1;

        public int ID
        {
            get { return _identifier; }
            set { if (_identifier != value) _identifier= value; }
        }
        public string Name
        {
            get { return _name; }
            set { if (_name != value) _name = value; }
        }
        public string Description
        {
            get { return _description; }
            set { if (_description != value) _description = value; }
        }
        public bool IsActiveNow
        {
            get { return _isActiveNow; }
            set { _isActiveNow = value;
                if (_isActiveNow) _completedByUser = false;
            }
        }
        public bool IsAvailableNow
        {
            get { return _isAvailableNow; }
            set { _isAvailableNow = value; }
        }
        public bool IsAvailableAtStart
        {
            get { return _isAvailableAtStart; }
            set { _isAvailableAtStart = value;
                if (_isAvailableAtStart) _isActiveNow = true; }
        }
        public bool CompletedByUser
        {
            get { return _completedByUser; }
            set { _completedByUser = value;
                if (_completedByUser) _isActiveNow = false;
            }
        }
        public TypeReward TypeReward
        {
            get { return _typeReward; }
            set { _typeReward = value; }
        }
        public int[] Rewards
        {
            get { return _rewards; }
            set {  _rewards = value; }
        }
        public int NextMissionId
        {
            get { return _nextMissionId; }
            set { _nextMissionId = value; }
        }

        public abstract bool UpdateMissionStatus(int parametre);
        public abstract void ResetMissionStatus();
        public virtual void AssignValuesFromCode(Quest toCopy)
        {
            ID = toCopy.ID; Name = toCopy.Name; Description = toCopy.Description;
            TypeReward = toCopy.TypeReward; Rewards = toCopy.Rewards; NextMissionId = toCopy.NextMissionId;
            //things that change should not be passed as reference but as value
            bool aux1 = toCopy.CompletedByUser; bool aux2 = toCopy.IsAvailableAtStart;
            bool aux3 = toCopy.IsActiveNow; bool aux4 = toCopy.IsAvailableNow;
            IsActiveNow = aux3; IsAvailableNow = aux4; IsAvailableAtStart = aux2;
            CompletedByUser = aux1;
        }
    }

}
