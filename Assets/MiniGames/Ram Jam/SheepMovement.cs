using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class SheepMovement : NetworkBehaviour 
{
    [SerializeField] private float speed;
    [SerializeField] private float TurnSpeed;
    [SerializeField] private float wanderTime;
    private float wanderTimeTimer;
    [SerializeField] private float wanderAngle;
    private bool canWalk;

    private Quaternion targetRot;
    private void Start() {
        
        wanderTimeTimer = wanderTime;
        transform.Rotate(0,0,Random.Range(-180,180));
    }

    private void Update() {
        if(!canWalk){return;}
        wanderTimeTimer-= Time.deltaTime;
        if(wanderTimeTimer<=0){
            wanderTimeTimer = wanderTime;
            WanderRotate();
        }else{
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, TurnSpeed*Time.deltaTime);
        }
    }

    private void WanderRotate(){
        targetRot = Quaternion.Euler(0,0, transform.rotation.z + Random.Range(-wanderAngle, wanderAngle));
    }

    private void FixedUpdate() {
        if(!canWalk){return;}
        transform.position += transform.up * speed * Time.deltaTime;
    }

    public void InitSheep(){
        canWalk = true;
    }
}
