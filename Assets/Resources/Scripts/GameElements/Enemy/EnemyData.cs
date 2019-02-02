using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    /// <summary>
    ///Data used for the actual enemy in the combat turn based, this entity does not
    ///have level because level will be calculated in base of the player and 
    ///will be load when the player contacts the Enemy GameObject which will use their
    ///own name to load that scriptableObject
    /// </summary>
    /// 
    [CreateAssetMenu(menuName = "Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public TypeEnemy TypeEnemy;
        public string Name;
        public Stats Stats;
        public int Experience;
        public int Gold;
        public List<float> IncreaseByLevel;
        public string ModelName;
        public string SpriteName;
        public RuntimeAnimatorController AnimatorController;
    }

}

