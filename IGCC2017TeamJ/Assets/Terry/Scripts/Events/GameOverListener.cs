using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverListener : MonoBehaviour {

    [SerializeField]
    private string winSceneName;
    [SerializeField]
    private string loseSceneName;

	// Use this for initialization
	void Start () {
        InitEvents();
	}

    private void OnDestroy() {
        DeinitEvents();
    }

    // Update is called once per frame
    void Update () {
		
	}

    void InitEvents() {
        GameplayChannel.GetInstance().WinEvent += Win;
        GameplayChannel.GetInstance().PlayerDeathEvent += Lose;
    }

    void DeinitEvents() {
        GameplayChannel.GetInstance().WinEvent -= Win;
        GameplayChannel.GetInstance().PlayerDeathEvent -= Lose;
    }

    void Win() {
        SceneManager.LoadScene(winSceneName);
    }

    void Lose(CharacterDeathInfo _info) {
        SceneManager.LoadScene(loseSceneName);
    }

}