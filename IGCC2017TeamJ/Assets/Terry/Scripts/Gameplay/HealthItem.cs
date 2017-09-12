using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour {

    [SerializeField]
    private int healValue = 20;

    private void RegisterEvents() {
        GameplayChannel.GetInstance().ResetItemsEvent += ResetItem;
    }

    private void UnregisterEvents() {
        GameplayChannel.GetInstance().ResetItemsEvent -= ResetItem;
    }

    private void Awake() {
        RegisterEvents();
    }

    private void OnDestroy() {
        UnregisterEvents();
    }

    private void ResetItem() {
        gameObject.SetActive(true);
    }
    
    public int GetHealValue() {
        return healValue;
    }

    public void SetHealValue(int _healValue) {
        healValue = Mathf.Clamp(_healValue, 0, 100);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Health playerHealth = other.gameObject.GetComponent<Health>();
            if (playerHealth == null) {
                return;
            }
            if (playerHealth.IsFullHealth()) {
                return;
            }

            playerHealth.IncreaseHealth(healValue);
            gameObject.SetActive(false);
        }
    }

}
