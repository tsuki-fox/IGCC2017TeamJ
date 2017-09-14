using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        Health healthComponent = other.gameObject.GetComponent<Health>();
        if (healthComponent != null) {
            healthComponent.SetCurrentHealth(0);
        }
    }

}
