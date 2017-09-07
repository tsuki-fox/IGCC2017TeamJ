using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
abstract public class BooleanInput : MonoBehaviour {

    [SerializeField]
    protected string inputName;
    [SerializeField]
    private CharacterActionBoolean[] actions;

    public string GetInputName() {
        return inputName;
    }

    abstract public bool Activated();
    abstract public bool Deactivated();

    private void Update() {
        if (Activated()) {
            for (uint i = 0; i < actions.Length; ++i) {
                Assert.AreNotEqual(actions[i], null);
                actions[i].ActivateAction();
            }
        }

        if (Deactivated()) {
            for (uint i = 0; i < actions.Length; ++i) {
                Assert.AreNotEqual(actions[i], null);
                actions[i].DeactivateAction();
            }
        }
    }

}