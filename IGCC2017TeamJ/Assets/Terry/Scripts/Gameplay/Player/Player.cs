using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//主人公
public class Player : MonoBehaviour {

    private bool isHiding = false;
    private int numHidingSpots = 0;
    [SerializeField]
    private string[] hideableTags;

    void InitEvents() {
        GameplayChannel.GetInstance().RequestPlayerEvent += RequestPlayerEvent;
    }

    void DeinitEvents() {
        GameplayChannel.GetInstance().RequestPlayerEvent -= RequestPlayerEvent;
    }

    void RequestPlayerEvent() {
        GameplayChannel.GetInstance().SendReplyPlayerEvent(gameObject);
    }

    public bool IsHiding() {
        return isHiding;
    }

    public int GetNumHidingSpots() {
        return numHidingSpots;
    }

    public string[] GetHideableTags() {
        return hideableTags;
    }

    private void Awake() {
        InitEvents();
    }

    private void OnDestroy() {
        DeinitEvents();
    }

    // Use this for initialization
    void Start () {
        Reset();
        GameplayChannel.GetInstance().SendReplyPlayerEvent(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        bool previousResult = isHiding;
        isHiding = (numHidingSpots > 0);

        if (isHiding != previousResult) {
            GameplayChannel.GetInstance().SendPlayerHidingEvent(isHiding);
        }
	}

    private void OnDisable() {
        Reset();
    }

    private void Reset() {
        isHiding = false;
        numHidingSpots = 0;
        GameplayChannel.GetInstance().SendPlayerHidingEvent(false);
    }

    private void OnTriggerEnter(Collider other) {
        for (int i = 0; i < hideableTags.Length; ++i) {
            if (other.gameObject.CompareTag(hideableTags[i])) {
                ++numHidingSpots;
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        for (int i = 0; i < hideableTags.Length; ++i) {
            if (other.gameObject.CompareTag(hideableTags[i])) {
                --numHidingSpots;
                break;
            }
        }
    }

}