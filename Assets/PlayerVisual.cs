using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
   [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer Hair;
    [SerializeField]private SpriteRenderer Eyes;
    
    public void setPlayerColor(Color c){
        spriteRenderer.color = c;
    }
    public void setPlayerHair(int index){
        Hair.sprite = Skins.GetInstance().getHair(index);
    }
    public void setPlayerEyes(int index){
        Eyes.sprite = Skins.GetInstance().getEye(index);
    }

    
}
