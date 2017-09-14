using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour {

    private float tranSitionTime;

    
    // Use this for initialization
    void Start () {
     
	}
	
	// Update is called once per frame
	void Update () {
        tranSitionTime += Time.deltaTime;
        if (tranSitionTime >= 2)
        {
            tranSitionTime = 0.0f;
            ReturnTitle();
        }
    }

    public void ReturnTitle()
    {
        Application.LoadLevel("Main");
    }
}
