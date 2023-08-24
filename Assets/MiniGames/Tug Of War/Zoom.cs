using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Zoom : MonoBehaviour
{
    [SerializeField] float zoomScaling;
    [SerializeField] float zoomJump;
    [SerializeField] double zoomCurrentScale;
    [SerializeField] Transform rope;
    [SerializeField] Image rage;
    private bool isOn = true;


    
    
    private Camera cam;
    private void Start() {
        cam = GetComponent<Camera>();
        
    }
    void Update()
    {
        if(!isOn){return;}
        float x = Mathf.Abs(rope.transform.position.x);
        zoomCurrentScale = 5 - 0.904f*x;
        Color c = rage.color;
        c.a = 102f * x/255f;
        rage.color = c;

        if(cam.orthographicSize < zoomCurrentScale){
            cam.orthographicSize += zoomScaling * Time.deltaTime;
        }
    }

    public void ZoomIn(){
        cam.orthographicSize -= zoomJump;
    }

    public void Reset(){
        isOn =false;
        cam.orthographicSize = 5f;
        Color c = rage.color;
        c.a = 0f;
        rage.color = c;
    }

    

    
}
