using UnityEngine;
using System.Collections;

public class AxisAlignedBillboard : MonoBehaviour {

	public enum BILLBOARD_AXIS {
		X,
		Y,
		Z,
	}

	public BILLBOARD_AXIS billboardAxis;
	public Camera mainCamera;

	// Use this for initialization
	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {
		if (mainCamera == null) {
			mainCamera = Camera.main;
			if (mainCamera == null) {
				return;
			}
		}

		Vector3 direction = mainCamera.transform.position - gameObject.transform.position;
		Quaternion rotation = Quaternion.LookRotation(direction);
		Vector3 rotationEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

		switch (billboardAxis) {
			case BILLBOARD_AXIS.X:
                rotationEulerAngles.x = rotation.eulerAngles.x;
				break;
			case BILLBOARD_AXIS.Y:
                rotationEulerAngles.y = rotation.eulerAngles.y;
				break;
			case BILLBOARD_AXIS.Z:
                rotationEulerAngles.z = rotation.eulerAngles.z;
				break;
			default:
				print(gameObject.name + " Invalid Billboard Axis!");
				break;
		}
		
		rotation.eulerAngles = rotationEulerAngles;
		gameObject.transform.rotation = rotation;
	}
}