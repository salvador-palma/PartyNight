using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinVisual : MonoBehaviour
{
    
    public int ID;
    public int Mode; //0-Eyes 1-Hair 2-Special

    private void Start() {
        GetComponent<Button>().onClick.AddListener(()=>{
            //TODO
        });
    }
}
