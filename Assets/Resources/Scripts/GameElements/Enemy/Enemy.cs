using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{

    public class Enemy: MonoBehaviour 
    {

        private bool _destroyThisObject = false;
        private string _name = "";

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (CombatTurnManager.Instance.StartedBattle == false)
                {
                    CombatTurnManager.Instance.PreloadTurn = ActualTurn.EnemyTurn;
                }
                EnemyData ed = Resources.Load<EnemyData>(ControlManager.Instance.PathToEnemyScriptableObjects + this.Name);
                if (CombatTurnManager.Instance.AddEnemy(ed))
                {
                    _destroyThisObject = true;    //DestroyImmediate doesnt work here so we used outside
                }
            }
            else if (other.tag == "Weapon")
            {
                if (CombatTurnManager.Instance.StartedBattle == false)
                {
                    CombatTurnManager.Instance.PreloadTurn = ActualTurn.PlayerTurn;
                }
                EnemyData ed = Resources.Load<EnemyData>(ControlManager.Instance.PathToEnemyScriptableObjects + this.Name);
                if (CombatTurnManager.Instance.AddEnemy(ed)) {
                    _destroyThisObject = true;
                }
                
            }
        }

        private void Update()
        {
            if (_destroyThisObject) DestroyImmediate(this.gameObject);
        }
    }
    
}

