using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
   [SerializeField] private SpriteRenderer spriteRenderer;

    
    public void setPlayerColor(Color c){
        spriteRenderer.color = c;
    }
}
