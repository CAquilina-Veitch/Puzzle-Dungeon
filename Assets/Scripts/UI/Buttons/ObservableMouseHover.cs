using R3;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObservableMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isHoverDetectionEnabled = true;
        
    private readonly ReactiveProperty<bool> isHovered = new();
    public ReadOnlyReactiveProperty<bool> IsHovered => isHovered;
        
    protected void EnableHoverDetection(bool newIsEnabled)
    {
        isHoverDetectionEnabled = newIsEnabled;
        if (!newIsEnabled && isHovered.CurrentValue)
            isHovered.Value = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHoverDetectionEnabled) return;
        isHovered.Value = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHoverDetectionEnabled) return;
        isHovered.Value = false;
    }
}