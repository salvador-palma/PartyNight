using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour {

    public static PlayerNetwork LocalInstance {get; private set;}
    public static float speed = 300f;
    public float current_speed = 0f;
    Rigidbody2D rb;
    [SerializeField] public Transform skinTr;
    public bool Finished=false;
    //[SerializeField] private PlayerVisual playerVisual;

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            LocalInstance = this;
            MiniGame.Instance.Setup();
            
        }

       
    }
    public virtual void Start() {
        
        rb = gameObject.GetComponent<Rigidbody2D>();
        PlayerData playerData = GameState.Instance.getPlayerData(OwnerClientId);
        skinTr.gameObject.GetComponent<SpriteRenderer>().color = GameState.Instance.getColor(playerData.colorID);
    }
    public virtual void Update()
    {
        if(!IsOwner || Finished){return;}
        if(Input.GetKeyDown(KeyCode.Space)){
            MiniGame.Instance.MiniGameExtension.Interact(OwnerClientId);
        }
        Vector2 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = new Vector2(mousePos.x - skinTr.position.x, mousePos.y - skinTr.position.y);
        
        skinTr.up = direction;
    }
    public virtual void FixedUpdate() {
        if(!IsOwner || Finished){return;}
        rb.velocity = new Vector2(Input.GetAxis("Horizontal")*current_speed*Time.deltaTime,Input.GetAxis("Vertical")*current_speed*Time.deltaTime);
    }

    public void SetFinish(){
        if(IsOwner && !Finished){
            Finished=true;
            GameUI.Instance.ShowWin();
            MiniGame.Instance.MiniGameExtension.SetFinish();
            //GameState.Instance.PlayerFinishServerRpc();
        }
    }

   
}
