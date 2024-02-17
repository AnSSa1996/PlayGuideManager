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
    [CreateAssetMenu(fileName = "PlayGuideStep_Talk", menuName = "PlayGuide/PlayGuideStep_Talk")]
    public class PlayGuideStep_Talk : PlayGuideStep
    {
        public List<DialogueData> DialogueDataList = new List<DialogueData>();
        public bool isComplete = false;
        
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
            Dialogue_Popup dialoguePopup = null;
            yield return UIManager.Instance.OpenUI<Dialogue_Popup>().ToCoroutine<Dialogue_Popup>((result => dialoguePopup = result));
            if (dialoguePopup.IsUnityNull()) yield break;
            dialoguePopup.SetData(DialogueDataList, Complete);
            yield return new WaitUntil(() => isComplete);
        }

        private void Complete()
        {
            isComplete = true;
        }
    }
}