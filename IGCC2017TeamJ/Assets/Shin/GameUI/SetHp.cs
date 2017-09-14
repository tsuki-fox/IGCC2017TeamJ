using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SetHp : MonoBehaviour
{

    // 敵のステータス
    //private EnemyStatus eStatus;

    // カウンター
    private int count;

    // UISlider
    private Slider hpSlider;
    
    // 体力のマックス値
    private int maxHp;


    // Use this for initialization
    void Start()
    {
        //eStatus = transform.root.GetComponent(EnemyStatus);
        hpSlider = GetComponent<Slider>();
        //maxHp = eStatus.GetMaxHp();
        hpSlider.value = hpSlider.maxValue;
        count = maxHp;


    }

    // Update is called once per frame
    void Update()
    {
        //var countNow = estatus.GetHp();

        // Hp減る計算処理
        //hpSlider.value = (countNow * hpSlider.maxValue) / maxHp;
        //count = countNow;

    }
}
