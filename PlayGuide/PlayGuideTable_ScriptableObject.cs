using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace PlayGuideFraemwork.PlayGuide
{
    [CreateAssetMenu(fileName = "PlayGuideTable_ScriptableObject", menuName = "PlayGuide/PlayGuideTable_ScriptableObject")]
    [System.Serializable]
    public class PlayGuideTable_ScriptableObject : ScriptableObject
    {
        public List<PlayGuideTable> PlayGuideTableList = new List<PlayGuideTable>();
    }
    
    [System.Serializable]
    public class PlayGuideTable
    {
        public ePlayGuideTrigger PlayGuideTrigger = ePlayGuideTrigger.None;
        public int PlayGuideIndex = 0;
        public string PlayGuideName = string.Empty;
        public List<int> PrerequirementPlayGuideIndex = new List<int>();
    }
}
