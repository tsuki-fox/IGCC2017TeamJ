using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionRotate : CharacterActionAxis {

    enum RotateAxis {
        RotateAxis_X,
        RotateAxis_Y,
        RotateAxis_Z
    }

    [SerializeField]
    private RotateAxis rotateAxis = RotateAxis.RotateAxis_Y;

	// Update is called once per frame
	void Update () {
        switch (rotateAxis) {
            case RotateAxis.RotateAxis_X:
                transform.eulerAngles = new Vector3(GetInputValue() * 360.0f, transform.rotation.y, transform.rotation.z);
                break;
            case RotateAxis.RotateAxis_Y:
                transform.eulerAngles = new Vector3(transform.rotation.x, GetInputValue() * 360.0f, transform.rotation.z);
                break;
            case RotateAxis.RotateAxis_Z:
                transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, GetInputValue() * 360.0f);
                break;
            default:
                break;
        }
	}

}