using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    /// <summary>
    /// This class has no other meaning that being the template for a proper configuration file but shows how 
    /// it shall be done in a real context;
    /// </summary>
    [CreateAssetMenu(menuName = "ConfigurationValues")]
    public class ConfigurationValues : ScriptableObject
    {

        [SerializeField] private bool _areYouAMemer = false;
        public bool AreYouAMember
        {
            get { return _areYouAMemer; }
            set { _areYouAMemer = value; }
        }
    }

}
