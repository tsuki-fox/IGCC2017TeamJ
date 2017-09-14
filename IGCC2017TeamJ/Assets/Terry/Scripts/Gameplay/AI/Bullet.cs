using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [SerializeField]
    private float bulletSpeed = 20.0f;
    [SerializeField]
    private int bulletDamage = 20;
    [SerializeField]
    private List<string> canHitTags;
    [SerializeField]
    private float lifeTimeDuration = 5.0f;
    private float lifeTimeTimer = 5.0f;

    public void SetBulletSpeed(float _bulletSpeed) {
        bulletSpeed = _bulletSpeed;
    }

    public float GetBulletSpeed() {
        return bulletSpeed;
    }

    public void SetBulletDamage(int _bulletDamage) {
        bulletDamage = _bulletDamage;
    }

    public int GetBulletDamage() {
        return bulletDamage;
    }

	// Use this for initialization
	void Start () {
        lifeTimeTimer = lifeTimeDuration;

    }
	
	// Update is called once per frame
	void Update () {
        if ((lifeTimeTimer -= Time.deltaTime) <= 0.0f) {
            GameObject.Destroy(gameObject);
            return;
        }

        // Move the bullet.
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
        
        // Deal damage.
        // Raycast to ensure that nothing is blocking the explosion.
        RaycastHit[] result = Physics.RaycastAll(gameObject.transform.position, transform.forward, bulletSpeed * Time.deltaTime);
        for (int i = 0; i < result.Length; ++i) {
            GameObject hitGameObject = result[i].collider.gameObject;

            for (int j = 0; j < canHitTags.Count; ++j) {
                if (hitGameObject.tag == canHitTags[j]) {
                    Health hitHealth = hitGameObject.GetComponent<Health>();
                    if (hitHealth == null) {
                        continue;
                    }

                    hitHealth.DecreaseHealth(bulletDamage);
                    GameObject.Destroy(gameObject);
                    break;
                }
            }
        }
    }
}
