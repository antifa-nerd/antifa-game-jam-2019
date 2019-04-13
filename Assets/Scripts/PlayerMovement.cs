using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed=1f;
    public float stepTimeDirect = 0.15f;
    public float stepTimeDiagonal = 0.20f;
    public bool griglia;

    float lastUpdate = 0.15f;
    // Update is called once per frame

    private Rigidbody2D rb;

    private bool isMoving;


    private void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        isMoving=false;
    }

    void FixedUpdate()
    {
        Vector3 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isMoving=(input.x!=0)||(input.y!=0);


        if (isMoving){
            Vector3 movement = input;
            rb.position+=(Vector2)movement.normalized*speed*Time.deltaTime;
            /*
            if (movement.x!=0)
                movement.x=Mathf.Sign(movement.x);
            if (movement.y!=0)
                movement.y=Mathf.Sign(movement.y);
            

            if (Mathf.Abs(movement.x)==1 && Mathf.Abs(movement.y)==1)
            {
                if (lastUpdate > stepTimeDiagonal){
                    rb.position+=(Vector2)movement;
                    lastUpdate = 0f;
                }
            }
            else
            {
                if (lastUpdate > stepTimeDirect){
                    rb.position+=(Vector2)movement;
                    lastUpdate = 0f;
                }
            }
            */
            

            

        }



        lastUpdate+=Time.deltaTime;
    }

}
