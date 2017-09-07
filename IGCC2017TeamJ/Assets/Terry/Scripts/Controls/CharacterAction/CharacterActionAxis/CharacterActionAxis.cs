using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionAxis : CharacterAction {

    private float inputValue = 0.0f;

    public float GetInputValue() {
        return inputValue;
    }

    public void SetInputValue(float _inputValue) {
        inputValue = _inputValue;
    }
    
}
