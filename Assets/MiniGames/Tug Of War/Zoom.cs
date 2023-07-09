using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Zoom : MonoBehaviour
{
    [SerializeField] float zoomScaling;
    [SerializeField] double zoomCurrentScale;
    [SerializeField] float zoomOutSpeed;
    [SerializeField] Transform rope;
    [SerializeField] Image rage;
    private bool isOn = true;


    //reset options
    float zoomInSpeed = 5;
    float colorShiftSpeed = 1;
    
    private Camera cam;
    private void Start() {
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        if(!isOn){
            if(cam.orthographicSize < zoomCurrentScale){
                cam.orthographicSize += zoomInSpeed * Time.deltaTime;
            }
            if(rage.color.a > 0){
                Color col = rage.color;
                col.a -= colorShiftSpeed/255;
                rage.color = col;
            }

            return;
        }
        if(cam.orthographicSize > zoomCurrentScale){
            cam.orthographicSize -= zoomOutSpeed * Time.deltaTime;
        }
        
        float f = Vector2.Distance(rope.transform.position, Vector2.zero);
        zoomCurrentScale = -(0.4*Mathf.Pow(f,2)) + 5;

        Color c = rage.color;
        c.a = 8*Mathf.Pow(f,2)/255;
        rage.color = c;
    }

    public void ZoomIn(){
        cam.orthographicSize += zoomScaling;
    }

    public void Reset(){
        isOn =false;
        zoomCurrentScale = 5f;
        
       
    }

    

    
}
