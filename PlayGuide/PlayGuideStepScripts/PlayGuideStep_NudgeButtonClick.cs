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
    [CreateAssetMenu(fileName = "PlayGuideStep_NudgeButtonClick", menuName = "PlayGuide/PlayGuideStep_NudgeButtonClick")]
    public class PlayGuideStep_NudgeButtonClick : PlayGuideStep
    {
        public string UIName = string.Empty;
        public string WidgetName = string.Empty;
        public bool isComplete = false;
        
        protected UIButton uiButton = null;
        protected Common_Nudge_Popup _nudgePopup = null;
        
        public override void Start()
        {
            base.Start();
        }

        public override void Init()
        {
            base.Init();
            isComplete = false;
        }

        public override IEnumerator Play()
        {
            yield return base.Play();
            var ui = UIManager.Instance.GetUI(UIName);
            if (ui.IsUnityNull()) yield break;
            uiButton = ui.GetControl<UIButton>(WidgetName);
            if(uiButton.IsUnityNull()) yield break;
            
            yield return UIManager.Instance.OpenUI<Common_Nudge_Popup>(UIPriority.High).ToCoroutine<Common_Nudge_Popup>(result => _nudgePopup = result);
            if(_nudgePopup.IsUnityNull()) yield break;
            _nudgePopup.SetData(uiButton.transform, uiButton.transform.parent, uiButton.transform.GetSiblingIndex());
            uiButton.onClick.AddListener(StartCoComplete);
            yield return new WaitUntil(() => isComplete);
        }
        
        private void StartCoComplete()
        {
            PlayGuideManager.Instance.StartCoroutine(CoComplete());
        }

        private IEnumerator CoComplete()
        {
            var commonNudgePopup = UIManager.Instance.GetUI<Common_Nudge_Popup>();
            if (commonNudgePopup.IsUnityNull()) yield break;
            commonNudgePopup.ResetNudgeEffect();
            uiButton.onClick.RemoveListener(StartCoComplete);
            var animator = uiButton.GetComponent<Animator>();
            if (animator != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
                {
                    yield return null;
                }
            }
            
            commonNudgePopup.Close();
            isComplete = true;
        }
    }
}