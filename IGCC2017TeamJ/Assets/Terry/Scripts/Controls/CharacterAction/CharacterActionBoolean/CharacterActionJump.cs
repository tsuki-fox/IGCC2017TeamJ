using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionJump : CharacterActionBoolean {

    [SerializeField]
    private float jumpImpulse = 100.0f;
    private bool canJump = false;

    public float GetJumpImpulse() {
        return jumpImpulse;
    }

    public bool GetCanJump() {
        return canJump;
    }

    public override void ResetAction() {
    }

    private void ConstantUpdate() {
    }

    private void ActiveUpdate() {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        if (rigidbody == null) {
            return;
        }

        if (canJump) {
            rigidbody.AddForce(new Vector3(0, jumpImpulse, 0), ForceMode.Impulse);
        }
        canJump = false;
        DeactivateAction();
    }

    private void Update() {
        ConstantUpdate();

        if (IsActionActive()) {
            ActiveUpdate();
        }
    }

    public void OnCollisionEnter(Collision collision) {
        // If we collided with the floor.
        if (collision.transform.position.y < transform.position.y) {
            canJump = true;
        }
    }

    public void OnCollisionExit(Collision collision) {
        canJump = false;
    }

}