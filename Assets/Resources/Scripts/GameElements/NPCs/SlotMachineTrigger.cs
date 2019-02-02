using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public class SlotMachineTrigger : EnviromentTrigger
    {
        public override void DoTrigger()
        {
            GameManager.Instance.SwapContextToMinigameFromWorld();
        }
    }
}
