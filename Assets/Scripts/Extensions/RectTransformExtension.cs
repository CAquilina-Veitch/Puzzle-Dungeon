using UnityEngine;

public static class RectTransformExtension
{
    public static RectTransform rectTransform(this MonoBehaviour obj)
    {
        var rect = obj.transform as RectTransform;
        if (rect == null) Debug.LogError(rect.name + " is not a RectTransform");
        return rect;
    }
}
