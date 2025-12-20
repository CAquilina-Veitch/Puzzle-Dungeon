using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class HealthHeartUI : MonoBehaviour
    {
        [SerializeField] private Image outlineImage;
        [SerializeField] private Image fillImage;
        private HeartState current;

        public void SetState(HeartState state)
        {
            current = state;
            fillImage.enabled = state == HeartState.Full;
        }

        public enum HeartState
        {
            Empty = 0,
            Full = 1,
        }
    }
}