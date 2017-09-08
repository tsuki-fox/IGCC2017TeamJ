using UnityEngine;
using System.Collections;

//Did this with the help of http://wiki.unity3d.com/index.php?title=CameraFacingBillboard
public class Billboard : MonoBehaviour {

	public enum BILLBOARD_AXIS { //Which axis do we use as our up axis.
		UP,
		DOWN,
		LEFT,
		RIGHT,
		FORWARD,
		BACKWARD,
	}

	public BILLBOARD_AXIS billboardAxis;
	public Camera camera;
	public bool reverseDirection;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (camera == null) {
			if (Camera.main == null) {
				return;
			}
			camera = Camera.main;
		}

		//This is the position we want to look at.
		Vector3 targetPosition;
		//When billboarding, the object need to have the same rotation as the camera, rather than just looking at the camera.
		//What I don't understand is why we are taking the camera's rotation and not the reverse of it.
		//If we want to face towards the camera, don't we need to face the opposite of where the camera is pointing?
		//Alright, apparently this is cause Unity's Quad faces the wrong way.
		if (reverseDirection) {
            targetPosition = transform.position + (camera.transform.rotation * Vector3.back);
		} else {
            targetPosition = transform.position + (camera.transform.rotation * Vector3.forward);
        }

		//Rotate our up axis by the quaternion.
		Vector3 targetOrientation = camera.transform.rotation * GetAxis();
		gameObject.GetComponent<Transform>().LookAt(targetPosition, targetOrientation);
	}

	private Vector3 GetAxis() {
		switch (billboardAxis) {
			case BILLBOARD_AXIS.UP:
				return Vector3.up;
			case BILLBOARD_AXIS.DOWN:
				return Vector3.down;
			case BILLBOARD_AXIS.LEFT:
				return Vector3.left;
			case BILLBOARD_AXIS.RIGHT:
				return Vector3.right;
			case BILLBOARD_AXIS.FORWARD:
				return Vector3.forward;
			case BILLBOARD_AXIS.BACKWARD:
				return Vector3.back;
			default:
				return new Vector3(0, 1, 0);
		}
	}

}