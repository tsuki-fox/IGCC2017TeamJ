using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerCamera : MonoBehaviour {

    [SerializeField]
    private GameObject target = null;

    [SerializeField]
    private float followDistanceX = 6.0f;
    [SerializeField]
    private float followDistanceZ = 3.0f;
    [SerializeField]
    private float followSpeed = 35.0f;
    [SerializeField]
    private Vector3 positionOffset = new Vector3(0.0f, 20.0f, -4.0f);

    [SerializeField]
    private float minSize = 9.0f;
    [SerializeField]
    private float maxSize = 13.0f;
    [SerializeField]
    private float sizeChangeSpeed = 5.0f;

    [SerializeField]
    private Vector3 minimumPosition = new Vector3(-100, -100, -100);
    [SerializeField]
    private Vector3 maximumPosition = new Vector3(100, 100, 100);

	// Use this for initialization
	void Start () {
		
	}

    private float LimitPosition(float _current, float _min, float _max) {
        if (_current < _min) {
            return _min;
        }
        else if (transform.position.x > maximumPosition.x) {
            return _max;
        } else {
            return _current;
        }
    }

	// Update is called once per frame
	void Update () {
        if (target != null) {
            Vector3 desiredPosition = transform.position;

            Vector3 targetPosition = target.transform.position + positionOffset;

            float directionToTargetX = targetPosition.x - transform.position.x;
            float directionToTargetZ = targetPosition.z - transform.position.z;
            float distanceToTargetX = Mathf.Abs(directionToTargetX);
            float distanceToTargetZ = Mathf.Abs(directionToTargetZ);

            // 相机与玩家离得太远了。
            if (distanceToTargetX > followDistanceX) {
                desiredPosition.x = transform.position.x + (Mathf.Min(distanceToTargetX - followDistanceX, followSpeed * Time.deltaTime) * Mathf.Sign(directionToTargetX));
            }

            if (distanceToTargetZ > followDistanceZ) {
                desiredPosition.z = transform.position.z + (Mathf.Min(distanceToTargetZ - followDistanceZ, followSpeed * Time.deltaTime) * Mathf.Sign(directionToTargetZ));
            }

            VelocityLimit playerVelocityLimit = target.GetComponent<VelocityLimit>();
            Rigidbody playerRigidbody = target.GetComponent<Rigidbody>();
            Camera playerCamera = gameObject.GetComponent<Camera>();
            Assert.AreNotEqual(null, playerCamera);
            if (playerVelocityLimit == null || playerRigidbody == null) {
                desiredPosition.y = targetPosition.y;
            } else {
                desiredPosition.y = targetPosition.y;

                float playerVelocity = playerRigidbody.velocity.magnitude;
                float desiredSize = minSize + (playerVelocity / playerVelocityLimit.GetMaxVelocity() * (maxSize - minSize));
                desiredSize = Mathf.Lerp(playerCamera.orthographicSize, desiredSize, sizeChangeSpeed * Time.deltaTime);
                playerCamera.orthographicSize = Mathf.Clamp(desiredSize, minSize, maxSize);
            }
            
            transform.position = desiredPosition;

            Debug.Log(transform.forward);
        }

        // Bounds checking.
        transform.position = new Vector3(LimitPosition(transform.position.x, minimumPosition.x, maximumPosition.x),
            LimitPosition(transform.position.y, minimumPosition.y, maximumPosition.y),
            LimitPosition(transform.position.z, minimumPosition.z, maximumPosition.z));

        // Raycast to the player. If there are obstacles between, turn them transparent.
        Vector3 directionToPlayer = target.transform.position - gameObject.transform.position;
        float raycastDistance = directionToPlayer.magnitude;
        RaycastHit[] result = Physics.RaycastAll(transform.position, directionToPlayer, raycastDistance);
        for (int i = 0; i < result.Length; ++i) {
            Collider hitCollider = result[i].collider;
            GameObject hitGameObject = hitCollider.gameObject;
            WallTransparency hitWallTransparency = hitGameObject.GetComponent<WallTransparency>();
            if (hitWallTransparency == null) {
                continue;
            }

            hitWallTransparency.TurnTransparent();
            //Debug.Log("PlayerCamera::Update - Turning Object Transparent.");
        }
    }
}