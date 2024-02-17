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
    [CreateAssetMenu(fileName = "PlayGuideStep_RunnerGameCheckChangeLane", menuName = "PlayGuide/PlayGuideStep_RunnerGameCheckChangeLane")]
    public class PlayGuideStep_RunnerGameCheckChangeLane : PlayGuideStep
    {
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
            var runnerLogic = GameManager.Instance.GameLogicBase as RunnerLogic;
            if(runnerLogic.IsUnityNull()) yield break;
            var currentLane = runnerLogic.runnerRunnerCharacter.CurrentLane;
            while (runnerLogic.runnerRunnerCharacter.CurrentLane == currentLane)
            {
                yield return null;
            }
        }
    }
}