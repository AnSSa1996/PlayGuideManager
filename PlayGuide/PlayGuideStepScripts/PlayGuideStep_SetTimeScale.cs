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
    [CreateAssetMenu(fileName = "PlayGuideStep_SetTimeScale", menuName = "PlayGuide/PlayGuideStep_SetTimeScale")]
    public class PlayGuideStep_SetTimeScale : PlayGuideStep
    {
        public float TimeSclae = 1.0f;
        
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
            GameManager.Instance.SetTimeScale(TimeSclae);
        }
    }
}