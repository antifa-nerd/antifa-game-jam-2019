using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    public bool Alerted = false;

    private Coroutine ResettingAlerted = null;

    // number of seconds before the enemy stops being triggered.
    public float ResettingTime = 3;

    private Coroutine TriggeringAlerted = null;

    // number of seconds the enemy needs to get triggered.
    public float TriggerTime = 0.3f;

    public GameObject Exclamation;

    // Start is called before the first frame update
    void Start()
    {
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
            TriggeringAlerted = StartCoroutine(GettingAlerted());
        }
    }

    private IEnumerator GettingAlerted()
    {
        yield return new WaitForSeconds(TriggerTime);
        Alerted = true;
        Exclamation.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (TriggeringAlerted != null)
            {
                StopCoroutine(TriggeringAlerted);
                TriggeringAlerted = null;
            }
            ResettingAlerted = StartCoroutine(ResetAlerted());
        }
    }

    private IEnumerator ResetAlerted()
    {
        yield return new WaitForSeconds(ResettingTime);
        Alerted = false;
        Exclamation.SetActive(false);
        ResettingAlerted = null;
    }
}
