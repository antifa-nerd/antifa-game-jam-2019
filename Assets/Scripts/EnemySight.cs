using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Debug.Log("Start!");
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        // if (other.gameObject.tag == "Player")
        // {
        UnityEngine.Debug.Log("Collision!");
        // }
    }
}
