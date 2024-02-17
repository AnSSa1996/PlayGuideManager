using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace PlayGuideFraemwork.PlayGuide
{
    public enum ePlayGuideTrigger
    {
        None,
        RoomStateEntry,
        RaceGameStart,
        ClawGameStart,
    }
    
    public partial class PlayGuideManager : Singleton<PlayGuideManager>
    {
        public bool IsPlayGuideForceStop = false;
        public Dictionary<ePlayGuideTrigger, List<PlayGuideTable>> PlayGuideTableDict = new Dictionary<ePlayGuideTrigger, List<PlayGuideTable>>(); 
        public PlayGuideScriptableObject playGuideScriptableObject = null;
        public List<PlayGuideStep> playGuideStep = null;
        public int currentPlayGuideStepIndex = 0;
        public int prevPlayGuideStepIndex = 0;

        public async UniTask PlayGuideFirstInit()
        {
            var playGuideTableScriptableObject = await ResourceManager.Instance.LoadAssetAsync<PlayGuideTable_ScriptableObject>("PlayGuideTable");
            if (playGuideTableScriptableObject.IsUnityNull())
            {
                Debug.LogError($"PlayGuideFirstInit : playGuideTableScriptableObject is Null");
                return;
            }
            
            foreach (var playGuideTable in playGuideTableScriptableObject.PlayGuideTableList)
            {
                if (PlayGuideTableDict.ContainsKey(playGuideTable.PlayGuideTrigger) == false)
                {
                    PlayGuideTableDict.Add(playGuideTable.PlayGuideTrigger, new List<PlayGuideTable>());
                }
                PlayGuideTableDict[playGuideTable.PlayGuideTrigger].Add(playGuideTable);
            }
        }
        
        public void PlayGuideTrigger(ePlayGuideTrigger playGuideTrigger)
        {
            if (IsPlayGuideForceStop) return;
            if (PlayGuideTableDict.ContainsKey(playGuideTrigger) == false) return;
            var playGuideTableList = PlayGuideTableDict[playGuideTrigger];
            foreach (var playGuideTable in playGuideTableList)
            {
                if (playGuideTable.IsUnityNull()) continue;
                var playGuideInfo = PublicTable.GetPlayGuideInfo(playGuideTable.PlayGuideIndex);
                if (playGuideInfo.IsUnityNull()) continue;
                if (playGuideInfo.playGuide_IsComplete) continue;
                if (CheckPlayGuideRequirementClear(playGuideTable) == false) continue;
                PlayGuideInit(playGuideTable.PlayGuideName);
                break;
            }
        }

        private bool CheckPlayGuideRequirementClear(PlayGuideTable playGuideTable)
        {
            if (playGuideTable.IsUnityNull()) return false;
            foreach (var prerequirementIndex in playGuideTable.PrerequirementPlayGuideIndex)
            {
                var clearCheck = PublicTable.GetPlayGuideInfo(prerequirementIndex).playGuide_IsComplete;
                if (clearCheck == false) return false;
            }
            return true;
        }

        public async void PlayGuideInit(string playGuide)
        {
            prevPlayGuideStepIndex = 0;
            currentPlayGuideStepIndex = 0;
            playGuideScriptableObject = await ResourceManager.Instance.LoadAssetAsync<PlayGuideScriptableObject>(playGuide);
            if (playGuideScriptableObject.IsUnityNull())
            {
                Debug.LogError($"PlayGuideInit : playGuideScriptableObject is Null");
                return;
            }

            playGuideStep = playGuideScriptableObject.PlayGuideSteps.ToList();
            PlayGuideStart();
        }

        public void PlayGuideStart()
        {
            if (playGuideScriptableObject.IsUnityNull())
            {
                Debug.LogError($"PlayGuideStart : playGuideScriptableObject is Null");
                return;
            }
            
            
            playGuideStep[prevPlayGuideStepIndex].Clear();
            playGuideStep[currentPlayGuideStepIndex].Start();
        }

        public void PlayGuideNext()
        {
            prevPlayGuideStepIndex = currentPlayGuideStepIndex;
            currentPlayGuideStepIndex++;
            if (playGuideStep.IsUnityNull() || playGuideStep.Count <= currentPlayGuideStepIndex)
            {
                PlayGuideClear();
                return;
            }
            
            PlayGuideStart();
        }

        public void PlayGuideClear()
        {
            if (playGuideScriptableObject.IsUnityNull()) return;
            if (playGuideScriptableObject.IsUnityNull() == false)
            {
                foreach (var step in playGuideScriptableObject.PlayGuideSteps)
                    step.Clear();
            }

            PublicTable.UpdateCompletePlayGuideInfo(playGuideScriptableObject.PlayGuideIndex);
            playGuideScriptableObject = null;
            playGuideStep = null;
        }
    }
}