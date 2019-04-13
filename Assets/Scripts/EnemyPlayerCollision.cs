using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyPlayerCollision : MonoBehaviour
{
    public float Radius = 0.5f;

    private void Start()
    {
        GetComponent<CircleCollider2D>().radius = Radius;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        }
    }
}
