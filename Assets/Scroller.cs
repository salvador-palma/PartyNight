using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{

    public Transform img1;
    public Transform img2;
    public float speed;
    public float x_Limit = -38.39f;
    public float x_Stay = 38.39f;

    public bool activated = false;
    
    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if(!activated){return;}
        img1.transform.position = new Vector2(img1.transform.position.x - Time.deltaTime * speed, img1.transform.position.y);
        img2.transform.position = new Vector2(img2.transform.position.x - Time.deltaTime * speed, img2.transform.position.y);

        if(img1.transform.position.x <= x_Limit){
            img1.transform.position =new Vector2(x_Stay, img1.transform.position.y);
        }

        if(img2.transform.position.x <= x_Limit){
            img2.transform.position =new Vector2(x_Stay, img2.transform.position.y);
        }
    }
}
