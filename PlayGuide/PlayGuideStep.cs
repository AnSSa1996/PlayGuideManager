using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayGuideFraemwork.PlayGuide
{
    [CreateAssetMenu(fileName = "PlayGuideStep", menuName = "PlayGuide/PlayGuide_Step")]
    public abstract class PlayGuideStep : ScriptableObject
    {
        public int Order = 0;
        public Coroutine startCoroutine = null;

        public virtual void Start()
        {
            Init();
            startCoroutine = PlayGuideManager.Instance.StartCoroutine(CoStart());
        }

        public IEnumerator CoStart()
        {
            yield return PlayGuideManager.Instance.StartCoroutine(Play());
            Next();
        }

        public virtual void Init()
        {
        }

        public virtual IEnumerator Play()
        {
            yield break;
        }

        public virtual void Clear()
        {
        }

        public virtual void Next()
        {   
            if (startCoroutine.IsUnityNull() == false) PlayGuideManager.Instance.StopCoroutine(startCoroutine);
            PlayGuideManager.Instance.PlayGuideNext();
        }
    }
}