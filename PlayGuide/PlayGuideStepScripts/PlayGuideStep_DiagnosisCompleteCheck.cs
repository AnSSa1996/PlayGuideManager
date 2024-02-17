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
    [CreateAssetMenu(fileName = "PlayGuideStep_DiagnosisCompleteCheck", menuName = "PlayGuide/PlayGuideStep_DiagnosisCompleteCheck")]
    public class PlayGuideStep_DiagnosisCompleteCheck : PlayGuideStep
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
            var dialogCompelete = ES3.Load($"{User.CurrentUserInfo.username}_Diagnosis_Compelte", false);
            while (dialogCompelete == false)
            {
                dialogCompelete = ES3.Load($"{User.CurrentUserInfo.username}_Diagnosis_Compelte", false);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}