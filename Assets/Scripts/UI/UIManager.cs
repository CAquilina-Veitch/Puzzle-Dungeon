using System.Collections.Generic;
using R3;
using Scripts.Behaviours;

public enum UIWindow
{
    None = 0,
    Menu = 10,
    Pause = 11,
    Options = 12,
    
    PlayerUI = 20,
}
public class UIManager : SingletonBehaviour<UIManager>
{
    public ReadOnlyReactiveProperty<HashSet<UIWindow>> CurrentUIWindows => currentUIWindows;
    private readonly ReactiveProperty<HashSet<UIWindow>> currentUIWindows = new(new());

    public bool SetUIWindowActive(UIWindow window, bool activeValue)
    {
        var cache = currentUIWindows.CurrentValue;
        var success = activeValue ? cache.Add(window) : cache.Remove(window);
        if (success) 
            currentUIWindows.Value = cache;
        return success;
    }

    public bool ToggleUIWindowActive(UIWindow window)
    {
        var wasEnabled = currentUIWindows.CurrentValue.Contains(window);
        SetUIWindowActive(window, !wasEnabled);
        return !wasEnabled;
    }
    protected override void OnAwake()
    {
        
    }
}
