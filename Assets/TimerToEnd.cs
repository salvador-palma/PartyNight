using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TimerToEnd : MonoBehaviour
{
    private float timerMax = 1;
    private float timerMaxTimer = 1f;
    private int seconds = 15;
    [SerializeField] private TextMeshProUGUI EndText;
    public bool activated = false;
    private bool ended = false;

    public void Init(int time){
        seconds = time;
        activated = true;
        ended = false;
    }
    void Update()
    {   
        if(!activated){return;}
        timerMaxTimer -= Time.deltaTime;
        if(timerMaxTimer <=0){
            timerMaxTimer = timerMax;
            EndText.text = ""+seconds;
            seconds--;   
            if(seconds < 0){
                seconds = 0;
                try{
                    if(!ended){
                        ended = true;
                        GameState.Instance.FinishGameServerRpc();
                    }
                    
                }catch{
                    Debug.Log("Usual error 2");
                }
                
            }
        }

    }
}
