//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class HackingTime : MonoBehaviour {

//    [SerializeField]
//    float hackTime;
//    // 保存用
//    float _hackTime;

//    // ハッキングしているかを確認するためのフラグ
//    bool hackFlag;

//	// Use this for initialization
//	void Start () {
//        // 初期化
//        _hackTime = hackTime;

//        hackFlag = false;

//        string hackNum = (_hackTime).ToString();
//        // 表示
//        GetComponent<UnityEngine.UI.Text>().text = hackNum;

//    }
	
//	// Update is called once per frame
//	void Update () {
//        // 経過処理
//        if (hackTime >= 0 && hackFlag != false)
//        {
//            hackTime -= Time.deltaTime;
//            GetComponent<UnityEngine.UI.Text>().text =
//            ((int)hackTime).ToString();
//            if(hackTime < 0)
//            {
//                hackFlag = false;
//            }
//        }

//        // 秒数を戻す
//        if (hackTime <= 0)
//        {
//            hackTime = _hackTime;
//        }
//    }

//    // フラグオン関数
//    public void SetVisible()
//    {
//        hackFlag = true;
//    }

//    // ハッキングの時間をセットする関数
//    public float SetHackTime(float hacktime)
//    {
//        return hackTime = hacktime;
//    }
//}
