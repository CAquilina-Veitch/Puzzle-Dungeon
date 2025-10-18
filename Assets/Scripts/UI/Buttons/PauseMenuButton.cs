using R3;
using TMPro;
using UnityEngine;

namespace Scripts.UI.Buttons
{
    public sealed class PauseMenuButton : ValuedUIButton<PauseButtonPanel.PauseButtons>
    {
        [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private float scaleOnHover = 1.2f;

        private void Awake() => IsHovered.Subscribe(OnHover).AddTo(this);

        private void OnHover(bool isHovered)
        {
            if (isHovered)
            {
                textField.rectTransform.localScale = Vector3.one;/*
                DOTween.Sequence(textField.rectTransform.DOScale(Vector3.one, 0))
                    .Append(textField.rectTransform.DOPunchScale(Vector3.one * scaleOnHover, 0.2f));*/
            }
        }
    }
}