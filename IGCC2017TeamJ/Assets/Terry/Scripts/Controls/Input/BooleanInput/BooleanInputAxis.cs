using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BooleanInputAxis : BooleanInput {

    [SerializeField]
    private bool canActivate = true;
    [SerializeField, Range(-1.0f, 1.0f)]
    private float activateValue = 0.5f;
    [SerializeField]
    private bool canDeactivate = false;
    [SerializeField, Range(-1.0f, 1.0f)]
    private float deactivateValue = -0.5f;

    public override bool Activated() {
        return canActivate && Input.GetAxis(inputName) >= activateValue;
    }

    public override bool Deactivated() {
        return canDeactivate && Input.GetAxis(inputName) <= deactivateValue;
    }

}