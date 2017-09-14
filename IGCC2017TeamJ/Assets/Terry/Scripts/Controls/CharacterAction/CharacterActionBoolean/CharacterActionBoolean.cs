using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterActionBoolean : CharacterAction {

    [SerializeField]
    private bool resetOnActivate = false; // If true, the action is already active, it will be reseted if Activate is called.
    private bool actionActive = false;

    public bool IsActionActive() {
        return actionActive;
    }

    public bool ResetsOnActivate() {
        return resetOnActivate;
    }

    void Start() {
    }

	// Update is called once per frame
	void Update () {
	}

    public void ActivateAction() {
        if (resetOnActivate) {
            ResetAction();
        }
        actionActive = true;
    }

    public void DeactivateAction() {
        actionActive = false;
    }

    public virtual void ResetAction() {
        // Reset the Action.
    }

}