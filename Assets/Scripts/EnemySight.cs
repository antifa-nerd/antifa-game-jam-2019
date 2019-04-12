using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    public bool Alerted = false;

    private Coroutine ResettingAlerted = null;

    public float ResettingTime = 3;

    public GameObject Exclamation;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Debug.Log("Start!");
        Exclamation.SetActive(false);
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (ResettingAlerted != null)
            {
                StopCoroutine(ResettingAlerted);
                ResettingAlerted = null;
            }
            Alerted = true;
            Exclamation.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            ResettingAlerted = StartCoroutine(ResetAlerted());
        }
    }

    private IEnumerator ResetAlerted()
    {
        yield return new WaitForSeconds(3);
        Alerted = false;
        Exclamation.SetActive(false);
        ResettingAlerted = null;
    }
}
