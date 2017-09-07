using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class AxisInput : MonoBehaviour {

    [SerializeField]
    protected string inputName;
    [SerializeField]
    protected CharacterActionAxis[] actions;

    public string GetInputName() {
        return inputName;
    }

    public float GetAxisValue() {
        return Input.GetAxis(inputName);
    }

    private void Update() {
        for (uint i = 0; i < actions.Length; ++i) {
            Assert.AreNotEqual(actions[i], null);
            actions[i].SetInputValue(GetAxisValue());
        }

        Debug.Log(inputName + ": " + GetAxisValue());
    }

}