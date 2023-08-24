using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinVisual : MonoBehaviour
{
    public static int HairID;
    public static int EyeID;
    public int ID;
    public int Mode; //0-Eyes 1-Hair 2-Special
    [SerializeField] private Transform ProfileButton;
    private void Start() {
        GetComponent<Button>().onClick.AddListener(()=>{
            SwitchButtonSkin();
        });
    }
    public void setSettings(int mode,int id){
        Mode = mode;
        ID = id;
    }

    public void SwitchButtonSkin(){
        getProfileButtonChild().sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
    }

    public SpriteRenderer getProfileButtonChild(){
        switch(Mode){
            case 0:
                EyeID = ID;
                return ProfileButton.Find("Eyes").GetComponent<SpriteRenderer>();
            case 1:
                HairID = ID;
                return ProfileButton.Find("Hair").GetComponent<SpriteRenderer>();
            default:
                throw new ArgumentException("No Mode set for this index");
        }
    }
}
