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
    [CreateAssetMenu(fileName = "PlayGuideStep_SetActiveRunnerGameSwipeGuidePopup", menuName = "PlayGuide/PlayGuideStep_SetActiveRunnerGameSwipeGuidePopup")]
    public class PlayGuideStep_SetActiveRunnerGameSwipeGuidePopup : PlayGuideStep
    {
        public bool isActive = false;
        
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
            if (isActive) UIManager.Instance.OpenUI<RunnerGame_Swipe_Guide_Popup>();
            else UIManager.Instance.CloseUI<RunnerGame_Swipe_Guide_Popup>();
        }
    }
}