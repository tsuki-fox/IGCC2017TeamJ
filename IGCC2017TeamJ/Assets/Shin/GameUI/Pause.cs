using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {

    public GameObject MainMenu;

    //public GameObject Exit;

	// Use this for initialization
	void Start () {
        OnUnClick();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClick()
    {
        // puaseボタンを押している間は時間を止める
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            MainMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            MainMenu.SetActive(true);
        }
    }

    public void OnUnClick()
    {
        MainMenu.SetActive(false);
    }
}
