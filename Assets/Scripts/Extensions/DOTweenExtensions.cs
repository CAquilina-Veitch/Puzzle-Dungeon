using DG.Tweening;
using UnityEngine;

namespace Runtime.Extensions
{
    public static class DOTweenExtensions
    {
        public static Tween SetStateWithTween(this CanvasGroup canvasGroup, bool isEnabled, float duration = 0f)
        {
            canvasGroup.interactable = isEnabled;
            canvasGroup.blocksRaycasts = isEnabled;

            Tween tween = canvasGroup.DOFade(isEnabled ? 1f : 0f, duration);
            return tween;
        }
    }
}