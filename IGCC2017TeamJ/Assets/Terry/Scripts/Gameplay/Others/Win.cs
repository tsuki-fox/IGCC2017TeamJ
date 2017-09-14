using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : MonoBehaviour {

    public List<string> winTags;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Win Triggered!");
        for (int i = 0; i < winTags.Count; ++i)
        {
            if (other.gameObject.CompareTag(winTags[i]))
            {
                GameplayChannel.GetInstance().SendWinEvent();
            }
        }
    }

}
