using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UIFramework;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PlayGuideFraemwork.PlayGuide
{
    [CreateAssetMenu(fileName = "PlayGuideStep_WaitForSecondsRealTime", menuName = "PlayGuide/PlayGuideStep_WaitForSecondsRealTime")]
    public class PlayGuideStep_WaitForSecondsRealTime : PlayGuideStep
    {
        public float WaitTime = 0f;
        
        public override void Start()
        {
            base.Start();
        }

        public override void Init()
        {
            base.Init();
        }

        public override IEnumerator Play()
        {
            yield return base.Play();
            yield return new WaitForSecondsRealtime(WaitTime);
        }
    }
}