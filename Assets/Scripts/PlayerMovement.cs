using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f;
    public float stepTimeDirect = 0.15f;
    public float stepTimeDiagonal = 0.20f;

    float lastUpdate = 0.15f;
    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (movement.x!=0)
                movement.x=Mathf.Sign(movement.x);
            if (movement.y!=0)
                movement.y=Mathf.Sign(movement.y);

        if (Mathf.Abs(movement.x)==1 && Mathf.Abs(movement.y)==1)
        {
            if (lastUpdate > stepTimeDiagonal){
                transform.position+=movement;
                lastUpdate = 0f;
            }
        }
        else
        {
            if (lastUpdate > stepTimeDirect){
                transform.position+=movement;
                lastUpdate = 0f;
            }
        }


        lastUpdate+=Time.deltaTime;
    }

}
