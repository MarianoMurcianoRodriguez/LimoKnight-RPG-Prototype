using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public class TransitionPanel : MonoBehaviour
    {
        private bool _shouldLoadNextPanel = false;

        public bool ShouldLoadNextPanel
        {
            get { return _shouldLoadNextPanel; }
        }

        public void SwapNextPanelToLoad()
        {
            if (_shouldLoadNextPanel == false) _shouldLoadNextPanel = true;
            else _shouldLoadNextPanel = false;
        }
       
    }
}

