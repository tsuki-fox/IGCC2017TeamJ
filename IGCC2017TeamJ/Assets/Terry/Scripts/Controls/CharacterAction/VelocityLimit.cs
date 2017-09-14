using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class VelocityLimit : MonoBehaviour {

    [SerializeField]
    private float maxVelocity = 5.0f;
    [SerializeField]
    private bool includeXAxis = true;
    [SerializeField]
    private bool includeYAxis = false;
    [SerializeField]
    private bool includeZAxis = true;

    public void SetMaxVelocity(float _maxVelocity) {
        maxVelocity = _maxVelocity;
    }

    public float GetMaxVelocity() {
        return maxVelocity;
    }

    public bool GetIncludeXAxis() {
        return includeXAxis;
    }

    public bool GetIncludeYAxis() {
        return includeYAxis;
    }

    public bool GetIncludeZAxis() {
        return includeZAxis;
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        if (rigidbody == null) {
            return;
        }
        //Assert.AreNotEqual(null, rigidbody);

        Vector3 velocity = new Vector3(includeXAxis ? rigidbody.velocity.x : 0.0f,
                                       includeYAxis ? rigidbody.velocity.y : 0.0f,
                                       includeZAxis ? rigidbody.velocity.z : 0.0f);

        if (velocity.magnitude > maxVelocity) {
            velocity = velocity.normalized * maxVelocity;
            rigidbody.velocity = new Vector3(includeXAxis ? velocity.x : rigidbody.velocity.x,
                                             includeYAxis ? velocity.y : rigidbody.velocity.y,
                                             includeZAxis ? velocity.z : rigidbody.velocity.z);
        }
	}
}