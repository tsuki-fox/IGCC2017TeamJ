using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoFly : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        if (rigidbody == null) {
            return;
        }

        if (rigidbody.velocity.y > 0.0f) {
            Vector3 newVelocity = rigidbody.velocity;
            newVelocity.y = 0.0f;
            rigidbody.velocity = newVelocity;
        }
	}

}