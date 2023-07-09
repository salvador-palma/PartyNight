using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerToDestroy : MonoBehaviour
{
    [SerializeField] private float timer;

    private void Start() {
        Debug.Log(transform.position);
    }
    private void Update() {
        timer-=Time.deltaTime;
        if(timer<0){
            Destroy(gameObject);
        }
    }

   
}
