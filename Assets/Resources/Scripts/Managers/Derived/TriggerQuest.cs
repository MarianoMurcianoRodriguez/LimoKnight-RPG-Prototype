using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    //This function will be called and added to an empty object (the trigger itself)
    //and will be  waitting till the player triggers it, it will then be passed to the quest manager
    //and then destroyed
    public class TriggerQuest : MonoBehaviour
    {
        private int _questID;
        private int _objectiveID;
        private SphereCollider _collider;

        public int QuestID
        {
            get { return _questID; }
            set { _questID = value; }
        }
        public int ObjectiveID
        {
            get { return _objectiveID; }
            set { _objectiveID = value; }
        }

        public void SetValues(int questId, int objectiveID, float radiusCollider)
        {
            _collider = this.gameObject.GetComponent<SphereCollider>();
            _questID = questId;
            _objectiveID = objectiveID;
            _collider.radius = radiusCollider;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                QuestManager.Instance.InformCompletitionQuest(_questID, _objectiveID);
            }
        }
    }
}
