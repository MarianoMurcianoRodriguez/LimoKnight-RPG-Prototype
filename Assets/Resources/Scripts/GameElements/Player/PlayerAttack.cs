using UnityEngine;
using System.Collections;

namespace LimoKnight
{
    
    public class PlayerAttack : MonoBehaviour
    {
        private float _speedAttack;
        private GameObject _weapon;
        private bool _weaponEquipped = false;
        private bool _canAttack = false;
        private float _timeSinceLastAttack = 0f;
        
        public void SetWeapon(GameObject weapon, float speedAttack)
        {
            _weapon = weapon;
            _speedAttack = speedAttack;
            _weaponEquipped = true;
        }

        private void Update()
        {
            _timeSinceLastAttack += _timeSinceLastAttack + Time.deltaTime;
            if (ControlManager.Instance.PlayerContext == PlayerContextType.InDungeon)
            {
               if (_weaponEquipped)
                {
                    //left click
                    if (Input.GetMouseButtonDown(0))
                        TryToAttack();
                }
            }
        }

        private void TryToAttack()
        {
            if (_timeSinceLastAttack < _speedAttack)
                return;
            Debug.LogError("NO ANIMATION IMPLEMENTED ON PLAYERATTACK > TRYTOATTACK");
            //Animation of attack here
        }
    }
}
