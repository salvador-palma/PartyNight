using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamZooms : MonoBehaviour
{
    

    private float fixedSize = 5f;
    private float speedZoomOut = 0.2f;
    [SerializeField] private Camera cam;
    [SerializeField] private bool active = true;


    //CAMERA RESET SETTINGS
    float defSize;
    float speedCamSizeReset = 0.1f;
    Vector2 defPos;
    float speedCamReset = 10f;

    bool inReset = false;

    private void Start() {
        defSize = cam.orthographicSize;
        defPos = cam.transform.position;
    }
    void Update()
    {
        
        if(fixedSize > cam.orthographicSize && active){
            cam.orthographicSize += speedZoomOut * Time.deltaTime;
            return;
        }
        
    }

    void Zoom(float dest){
        cam.orthographicSize = dest;
    }

    

    



    
}
