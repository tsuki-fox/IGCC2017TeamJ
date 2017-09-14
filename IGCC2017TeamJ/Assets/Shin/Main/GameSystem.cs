using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{

  


    enum STAGE
    {
        FIRST,
        SECOND,
        THIRD
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ゲームをスタートするときの関数
    public void GameStart()
    {
        Application.LoadLevel("Game");
    }

    public void EndGame()
    {
        Application.LoadLevel("End");
    }

    // メニューに戻る時の関数
    public void ReturnMenu()
    {
        Application.LoadLevel("Main");
    }

   

    public void StageTransition()
    {
        //switch()
    }

    // ゲーム終了するときの関数
    public void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
	Application.Quit();
#endif
    }
}