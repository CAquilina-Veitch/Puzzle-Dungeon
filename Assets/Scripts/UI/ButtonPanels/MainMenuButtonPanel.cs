using System;
using Scripts.Cutscenes;
using Scripts.UI.Buttons;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainMenuButtonPanel : ExclusiveButtonPanel<MainMenuButtonPanel.MainMenuButtons>
{
    [SerializeField] private AnimatedUIImage cursor;
    [SerializeField] private RectTransform cursorImageRectTransform;
    [SerializeField] private float cursorPunchScale = 2;
    [SerializeField] private float cursorPunchDuration;
    [SerializeField] private SoundEffectPlayer sfxPlayer;
    
    public enum MainMenuButtons
    {
        Null,
        Continue,
        NewGame,
        Options,
        Credits,
        Quit
    }

    public CutsceneStepDefiner def = new();
    protected override void OnButtonPressed(ValuedUIButton<MainMenuButtons> button)
    {
        var buttonPressed = button.Value;
        Debug.Log("Button pressed: " + buttonPressed);
        //cursorImageRectTransform.localScale = Vector3.one;
        //cursorImageRectTransform.DOPunchScale(Vector3.one * cursorPunchScale, cursorPunchDuration);

        int goodOrEvil = UnityEngine.Random.Range(0,2);

        var clickSound = button.Value is MainMenuButtons.NewGame or MainMenuButtons.Continue
            ? SoundEffectType.MenuSelect
            : (goodOrEvil == 0 ? SoundEffectType.Good : SoundEffectType.Evil);
        sfxPlayer.PlaySoundEffect(clickSound);
        
        switch (buttonPressed)
        {
            case MainMenuButtons.Continue:
                break;
            case MainMenuButtons.NewGame:
                CutsceneManager.Instance.TryPlayCutscene(CutsceneID.TestCutscene1);
                
                
                //SceneManager.Instance.LoadScene(SceneID.Overworld);
                break;
            case MainMenuButtons.Options:
                UIManager.Instance.SetUIWindowActive(UIWindow.Options,true);
                break;
            case MainMenuButtons.Credits:
                break;
            case MainMenuButtons.Quit:
                Application.Quit();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(buttonPressed), buttonPressed, null);
        }
    }

    protected override void OnButtonHovered(ValuedUIButton<MainMenuButtons> button)
    {
        cursor.rectTransform().anchoredPosition = 
            new Vector2(cursor.rectTransform().anchoredPosition.x, button.rectTransform().anchoredPosition.y);
        
        if (sfxPlayer == null) return;
        
        sfxPlayer.PlaySoundEffect(sfx: SoundEffectType.MenuHover);
    }
}