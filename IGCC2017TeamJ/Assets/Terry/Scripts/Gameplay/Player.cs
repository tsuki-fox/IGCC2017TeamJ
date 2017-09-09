using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//主人公
public class Player : MonoBehaviour {

    private bool inBush = false;
    private int numBushes = 0;
    [SerializeField]
    private string[] hideableTags;

    public bool IsInBush() {
        return inBush;
    }

    public int GetNumBushes() {
        return numBushes;
    }

    public string[] GetHideableTags() {
        return hideableTags;
    }

	// Use this for initialization
	void Start () {
        Reset();
	}
	
	// Update is called once per frame
	void Update () {
        inBush = (numBushes > 0);
	}

    private void OnDisable() {
        Reset();
    }

    private void Reset() {
        inBush = false;
        numBushes = 0;
    }

    private void OnTriggerEnter(Collider other) {
        for (int i = 0; i < hideableTags.Length; ++i) {
            if (other.gameObject.CompareTag(hideableTags[i])) {
                ++numBushes;
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        for (int i = 0; i < hideableTags.Length; ++i) {
            if (other.gameObject.CompareTag(hideableTags[i])) {
                --numBushes;
                break;
            }
        }
    }

}