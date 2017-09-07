using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerRotationInput : AxisInput {

    [SerializeField]
    protected string inputName2;
    [SerializeField]
    private float deadZone;

    public string GetInputName2() {
        return inputName2;
    }

    public override float GetAxisValue() {
        return Mathf.Atan2(Input.GetAxis(inputName2), Input.GetAxis(inputName)) / (Mathf.PI * 2.0f);
    }

    // Update is called once per frame
    private void Update() {
        Vector2 inputVector = new Vector2(Input.GetAxis(inputName), Input.GetAxis(inputName2));
        if (inputVector.sqrMagnitude < deadZone * deadZone) {
            return;
        }

        for (uint i = 0; i < actions.Length; ++i) {
            Assert.AreNotEqual(actions[i], null);
            actions[i].SetInputValue(GetAxisValue());
        }
    }
}