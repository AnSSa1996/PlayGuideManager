using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayGuideFraemwork.PlayGuide
{
    [CreateAssetMenu(fileName = "PlayGuide_ScriptableObject", menuName = "PlayGuide/PlayGuide_ScriptableObject")]
    [System.Serializable]
    public class PlayGuideScriptableObject : ScriptableObject
    {
        public int PlayGuideIndex = 0;
        public List<PlayGuideStep> PlayGuideSteps = new List<PlayGuideStep>();

        public void CleanEmptyList()
        {
            PlayGuideSteps.RemoveAll(x => x == null);
        }
    }
    
}
