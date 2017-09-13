using UnityEngine;
using System.Collections;
using UnityEditor.Events;

public class Health : MonoBehaviour {

    [SerializeField]
    private bool startWithMaxHealth = true;
    [SerializeField]
    private int currentHealth = 100;
    private int previousHealth;
    [SerializeField]
    private int maxHealth = 100;
    private int previousMaxHealth;

    void SendDeathEventIfDead() {
        if (IsDead()) {
            CharacterDeathInfo info;
            info.name = gameObject.name;
            info.tag = gameObject.tag;
            info.deathPosition = gameObject.transform.position;

            GameplayChannel.GetInstance().SendPlayerDeathEvent(info);
        }
    }

    void Awake() {
    }

    // Use this for initialization
    void Start() {
        previousMaxHealth = maxHealth;
        if (startWithMaxHealth) {
            currentHealth = maxHealth;
        }
        previousHealth = currentHealth;

        SendDeathEventIfDead();
    }

    // Update is called once per frame
    void Update() {
    }

    public int GetCurrentHealth() {
        return currentHealth;
    }

    public void SetCurrentHealth(int _currentHealth) {
        int temp = currentHealth; //Store our current health.
        currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
        if (temp != currentHealth) { //There was a change in health.
            previousHealth = temp;
        }

        SendDeathEventIfDead();
    }

    public void SetMaxHealth(int _maxHealth) {
        int temp = currentHealth; //Store our current health.
        int tempMax = maxHealth; //Store our current maxHealth.

        maxHealth = Mathf.Max(0, _maxHealth);
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (temp != currentHealth) { //There was a change in health.
            previousHealth = temp;
        }
        if (tempMax != maxHealth) { //There was a change in max health.
            previousMaxHealth = tempMax;
        }

        SendDeathEventIfDead();
    }

    public int GetMaxHealth() {
        return maxHealth;
    }

    public float GetHealthRatio() {
        return (float)currentHealth / (float)maxHealth;
    }

    public void DecreaseHealth(int _healthLost) {
        if (_healthLost <= 0) {
            return;
        }
        int temp = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - _healthLost);
        if (temp != currentHealth) { //There was a change in health.
            previousHealth = temp;
        }

        SendDeathEventIfDead();
    }

    public void IncreaseHealth(int _healthGain) {
        if (_healthGain <= 0) {
            return;
        }

        int temp = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + _healthGain);
        if (temp != currentHealth) { //There was a change in health.
            previousHealth = temp;
        }

        SendDeathEventIfDead();
    }

    public int GetPreviousHealth() {
        return previousHealth;
    }

    public int GetPreviousMaxHealth() {
        return previousMaxHealth;
    }

    public bool IsFullHealth() {
        return currentHealth == maxHealth;
    }

    public bool IsAlive() {
        return currentHealth > 0;
    }

    public bool IsDead() {
        return !IsAlive();
    }

}