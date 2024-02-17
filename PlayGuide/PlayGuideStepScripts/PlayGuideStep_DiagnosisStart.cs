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
    [CreateAssetMenu(fileName = "PlayGuideStep_DiagnosisStart", menuName = "PlayGuide/PlayGuideStep_DiagnosisStart")]
    public class PlayGuideStep_DiagnosisStart : PlayGuideStep
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
            UIManager.Instance.OpenUI<Common_Diagnosis_Popup>();
        }
    }
}