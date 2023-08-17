using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Unity.Netcode;

public class TimerToNext : MonoBehaviour
{
    private float timerMax = 1;
    private float timerMaxTimer = 1f;
    private int seconds = 7;
    [SerializeField] private TextMeshProUGUI NextText;
    public bool activated = false;
    void Update()
    {   
        timerMaxTimer -= Time.deltaTime;
        if(timerMaxTimer <=0){
            timerMaxTimer = timerMax;
            NextText.text = "Next Game in " + seconds;
            seconds--;   
            if(seconds <= 0){
                seconds = 0;
            }
        }

    }
}
