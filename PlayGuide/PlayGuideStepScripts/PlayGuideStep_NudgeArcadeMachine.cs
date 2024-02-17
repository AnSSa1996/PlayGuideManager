using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UIFramework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace PlayGuideFraemwork.PlayGuide
{
    [CreateAssetMenu(fileName = "PlayGuideStep_Nudge3DClick", menuName = "PlayGuide/PlayGuideStep_Nudge3DClick")]
    public class PlayGuideStep_NudgeArcadeMachine : PlayGuideStep
    {
        public string ArcadeMachineName = string.Empty;
        public float NudgeTime = 2.0f;
        protected ArcadeMachine _arcadeMachine = null;
        
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
            _arcadeMachine = GameObject.Find(ArcadeMachineName)?.GetComponent<ArcadeMachine>();
            if(_arcadeMachine.IsUnityNull()) yield break;
            GameManager.Instance.SetCameraFollow(_arcadeMachine.transform);
            _arcadeMachine.SelectMaterial();
            yield return new WaitForSeconds(NudgeTime);
            GameManager.Instance.SetCameraFollow(GameManager.Instance.PlayerGameObject.transform);
            _arcadeMachine.UnSelectMaterial();
        }
    }
}