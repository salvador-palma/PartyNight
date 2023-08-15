using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class Logo : MonoBehaviour
{
    [SerializeField] private float progress;
    [SerializeField] private float speed;
    [SerializeField] private Image img;
    [SerializeField] private float step;
    private void Start() {
        img = GetComponent<Image>();
    }
    private void Update() {
        if(img.fillAmount < progress){
            img.fillAmount += speed*Time.deltaTime;
        }
    }

    public void newTask(int steps){
        img.fillAmount = 0;
        progress= 0;
        step = 1f/steps;
    }
    public void Progress(){
        
        progress+=step;
        //Debug.Log(img.fillAmount);
    }
}
