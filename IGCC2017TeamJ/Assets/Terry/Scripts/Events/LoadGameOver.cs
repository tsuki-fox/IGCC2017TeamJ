using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameOver : MonoBehaviour {

    public List<string> triggerTags;
    public string loadSceneName = "End";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider col)
    {
        for (int i = 0; i < triggerTags.Count; ++i) {
            if (col.gameObject.CompareTag(triggerTags[i])) {
                SceneManager.LoadScene(loadSceneName);
            }
        }
	}
}
