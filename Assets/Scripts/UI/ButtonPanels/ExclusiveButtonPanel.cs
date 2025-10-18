using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using Scripts.UI.Buttons;
using UnityEngine;

public abstract class ExclusiveButtonPanel<T> : MonoBehaviour
{
    [SerializeField] private List<ValuedUIButton<T>> buttonList;
    [SerializeField] private int defaultHoveredValue = -1;

    private void OnValidate()
    {
        var buttonsInChildren = GetComponentsInChildren<ValuedUIButton<T>>().ToList();
        if (buttonsInChildren.Count <= 0)
        {
            Debug.LogWarning($"No buttons found in {gameObject.name}");
            return;
        }
        buttonList = buttonsInChildren.ToList();

    }

    private void Awake()
    {
        foreach (var button in buttonList)
        {
            button.CurrentButtonState.Where(x => x is ButtonState.Pressed).Skip(1)
                .Subscribe(_ => OnButtonPressed(button)).AddTo(this);

            button.CurrentButtonState.Where(x => x is ButtonState.Hovered).Skip(1)
                .Subscribe(_ => OnButtonHovered(button)).AddTo(this);
        }
    }

    private void OnEnable()
    {
        if (defaultHoveredValue >= 0 && defaultHoveredValue < buttonList.Count)
            OnButtonHovered(buttonList[defaultHoveredValue]);
    }

    protected abstract void OnButtonPressed(ValuedUIButton<T> buttonPressed);
    protected abstract void OnButtonHovered(ValuedUIButton<T> buttonHovered);
}