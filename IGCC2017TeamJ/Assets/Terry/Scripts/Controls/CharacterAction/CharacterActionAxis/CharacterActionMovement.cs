using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionMovement : CharacterActionAxis {

    [SerializeField]
    private Vector3 moveForce;

    public Vector3 GetMoveForce() {
        return moveForce;
    }

    public void SetMoveForce(Vector3 _moveForce) {
        moveForce = _moveForce;
    }

    private void Update() {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.AddForce(moveForce * GetInputValue(), ForceMode.Force);
    }

}