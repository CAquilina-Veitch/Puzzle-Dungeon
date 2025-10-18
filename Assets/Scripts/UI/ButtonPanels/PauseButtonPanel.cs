using System;
using Scripts.UI.Buttons;
using UnityEngine;

public class PauseButtonPanel : ExclusiveButtonPanel<PauseButtonPanel.PauseButtons>
{
    public enum PauseButtons
    {
        Null,
        Continue,
        Options,
        Credits,
        Menu,
        Quit
    }
    [SerializeField] private SoundEffectPlayer sfxPlayer; 

    protected override void OnButtonPressed(ValuedUIButton<PauseButtons> button)
    {
        var buttonPressed = button.Value;
        Debug.Log("Button pressed: " + buttonPressed);

        int GoodOrEvil = UnityEngine.Random.Range(0, 2);

        var clickSound = button.Value is PauseButtons.Continue
            ? SoundEffectType.MenuSelect
            : (GoodOrEvil == 0 ? SoundEffectType.Good : SoundEffectType.Evil);
        sfxPlayer.PlaySoundEffect(clickSound);

        switch (buttonPressed)
        {
            case PauseButtons.Null:
                break;
            case PauseButtons.Continue:
                break;
            case PauseButtons.Options:
                break;
            case PauseButtons.Credits:
                break;
            case PauseButtons.Menu:
                break;
            case PauseButtons.Quit:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void OnButtonHovered(ValuedUIButton<PauseButtons> buttonHovered)
    {
        if (sfxPlayer == null) return;
            
        sfxPlayer.PlaySoundEffect(sfx: SoundEffectType.MenuHover);
    }
        
}