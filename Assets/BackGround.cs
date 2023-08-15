using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float LoopSpeed;
    public Renderer ren;
   
    void Update()
    {
        ren.material.mainTextureOffset += new Vector2(LoopSpeed*Time.deltaTime,0f);
    }
}
