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
    [CreateAssetMenu(fileName = "PlayGuideStep_NudgeWidgetTime", menuName = "PlayGuide/PlayGuideStep_NudgeWidgetTime")]
    public class PlayGuideStep_NudgeWidgetTime : PlayGuideStep
    {
        public string UIName = string.Empty;
        public string WidgetName = string.Empty;
        public float nudgeTime = 0f;
        
        protected RectTransform widget = null;
        protected Common_Nudge_Popup _nudgePopup = null;
        
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
            var ui = UIManager.Instance.GetUI(UIName);
            if (ui.IsUnityNull()) yield break;
            widget = ui.GetControl<RectTransform>(WidgetName);
            if(widget.IsUnityNull()) yield break;
            
            yield return UIManager.Instance.OpenUI<Common_Nudge_Popup>(UIPriority.High).ToCoroutine<Common_Nudge_Popup>(result => _nudgePopup = result);
            if(_nudgePopup.IsUnityNull()) yield break;
            _nudgePopup.SetData(widget.transform, widget.transform.parent, widget.transform.GetSiblingIndex());
            yield return new WaitForSecondsRealtime(nudgeTime);
            var commonNudgePopup = UIManager.Instance.GetUI<Common_Nudge_Popup>();
            if (commonNudgePopup.IsUnityNull()) yield break;
            commonNudgePopup.ResetNudgeEffect();
            commonNudgePopup.Close();
            
        }
    }
}