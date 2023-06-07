using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class PlayerNetwork : NetworkBehaviour {

    
    public float speed;
    Rigidbody2D rb;
    [SerializeField] private Transform skinTr;
    




  
    private void Start() {
        
        rb = gameObject.GetComponent<Rigidbody2D>();
        gameObject.transform.Find("Nick").GetComponent<TextMeshProUGUI>().text = "IDK";
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner){return;}

        Vector2 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = new Vector2(mousePos.x - skinTr.position.x, mousePos.y - skinTr.position.y);
        
        skinTr.up = direction;



    }
    private void FixedUpdate() {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal")*speed*Time.deltaTime,Input.GetAxis("Vertical")*speed*Time.deltaTime);
    }
}
