using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpCamera : MonoBehaviour {

    public Camera rotateCamera;

    //public GameObject Player;

	// Use this for initialization
	void Start () {
        rotateCamera = Camera.main;
        //Player = GameObject.Find("Cylinder");
	}
	
	// Update is called once per frame
	void Update () {
        // 常にカメラを見る処理
        transform.rotation = rotateCamera.transform.rotation;
	}

    public void Disable()
    {
        // 死んだら消えてもらうぞい☆
        gameObject.SetActive(false);
    }



}
