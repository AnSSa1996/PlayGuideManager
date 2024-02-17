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
    [CreateAssetMenu(fileName = "PlayGuideStep_RunnerWrongItemTimeSet", menuName = "PlayGuide/PlayGuideStep_RunnerWrongItemTimeSet")]
    public class PlayGuideStep_RunnerWrongItemTimeSet : PlayGuideStep
    {
        public float time = 0f;
        
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
            var ui = UIManager.Instance.GetUI<RunnerInGame_ScreenUI>();
            ui.SetWrongAnswerItem(time);
        }
    }
}