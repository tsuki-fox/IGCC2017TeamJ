using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BooleanInputButton : BooleanInput {

    enum ButtonState {
        ButtonState_OnDown,
        ButtonState_OnUp,
        ButtonState_StayDown,
        ButtonState_StayUp,
        ButtonState_None,
    }

    [SerializeField]
    private ButtonState activateValue = ButtonState.ButtonState_None;
    [SerializeField]
    private ButtonState deactivateValue = ButtonState.ButtonState_None;

    private bool CalculateResult(ButtonState _buttonState) {
        switch (_buttonState) {
            case ButtonState.ButtonState_OnDown:
                return Input.GetButtonDown(inputName);
                break;
            case ButtonState.ButtonState_OnUp:
                return Input.GetButtonUp(inputName);
                break;
            case ButtonState.ButtonState_StayUp:
                return !Input.GetButton(inputName);
                break;
            case ButtonState.ButtonState_StayDown:
                return Input.GetButton(inputName);
                break;
            default:
                return false;
                break;
        }
    }

    public override bool Activated() {
        return CalculateResult(activateValue);
    }

    public override bool Deactivated() {
        return CalculateResult(deactivateValue);
    }

}