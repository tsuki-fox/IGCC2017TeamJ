using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActionPlayerAttack : CharacterActionBoolean {

    [SerializeField]
    GameObject powerCord;

	// Use this for initialization
	void Start () {
        powerCord = GameObject.Instantiate(powerCord);
        powerCord.transform.SetParent(gameObject.transform);
        powerCord.transform.localPosition = new Vector3();
        powerCord.transform.localRotation = new Quaternion();
        powerCord.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
	
	// Update is called once per frame
	void Update () {
		if (IsActionActive()) {
            powerCord.SetActive(true);
            DeactivateAction();
        }
	}

    public override void ResetAction() {
        powerCord.SetActive(false);
    }

}
