using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionBooleanDebug : CharacterActionBoolean {

    [SerializeField]
    private float cooldownDuration = 0.1f;
    private float cooldownTimer = 0.0f;

    public override void ResetAction() {
        cooldownTimer = 0.0f;
    }

    private void ConstantUpdate() {
        cooldownTimer = Mathf.Max(0.0f, cooldownTimer - Time.deltaTime);

        if (cooldownTimer <= 0.0f && IsActionActive()) {
            DeactivateAction();
        }
    }

    private void ActiveUpdate() {
        if (cooldownTimer <= 0.0f) {
            Debug.Log("CharacterActionBooleanDebug::ActiveUpdate");
            cooldownTimer = cooldownDuration;
        }
    }

    private void Update() {
        ConstantUpdate();

        if (IsActionActive()) {
            ActiveUpdate();
        }
    }

}