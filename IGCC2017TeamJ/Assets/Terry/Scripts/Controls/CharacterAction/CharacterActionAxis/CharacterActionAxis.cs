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

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
    
}
