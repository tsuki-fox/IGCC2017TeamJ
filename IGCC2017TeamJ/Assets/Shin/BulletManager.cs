using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {

    [SerializeField]
    GameObject bullet;

    [SerializeField]

    Transform muzzle;

    [SerializeField]

    float speed = 10;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
	}

   public void AttackBullet()
    {
        // 弾丸の複製
        GameObject bullets = GameObject.Instantiate(bullet) as GameObject;

        Vector3 force;
        force = this.gameObject.transform.forward * speed;
        // Rigidbodyに力を加えて発射
        bullets.GetComponent<Rigidbody>().AddForce(force);
        // 弾丸の位置を調整
        bullets.transform.position = muzzle.position;

    }


 
}
