using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    public bool Alerted
    {
        get;
        private set;
    }

    private void SetAlerted(bool value)
    {
        Alerted = value;
        var ev = OnAlertedChanged;
        if (ev != null)
        {
            ev(this, value);
        }
    }

    private Coroutine ResettingAlerted = null;

    // number of seconds before the enemy stops being triggered.
    public float ResettingTime = 3;

    private Coroutine TriggeringAlerted = null;

    // number of seconds the enemy needs to get triggered.
    public float TriggerTime = 0.3f;

    public GameObject Exclamation;
    [SerializeField] private float ExclamationOffset = 0.5f;

    public event EventHandler<bool> OnAlertedChanged;

    private EnemyFieldOfView EnemyFieldOfView;

    // Start is called before the first frame update
    void Start()
    {
        if (Exclamation != null)
        {
            Exclamation.SetActive(false);
        }
        EnemyFieldOfView = GetComponent<EnemyFieldOfView>();
        EnemyFieldOfView.OnPlayerEnter += OnPlayerEnter;
        EnemyFieldOfView.OnPlayerExit += OnPlayerExit;
    }

    private void OnDisable()
    {
        EnemyFieldOfView.OnPlayerEnter -= OnPlayerEnter;
        EnemyFieldOfView.OnPlayerExit -= OnPlayerExit;
    }

    private void LateUpdate()
    {
        Exclamation.transform.position = new Vector2( this.transform.position.x + ExclamationOffset, this.transform.position.y + ExclamationOffset );
    }

    // Update is called once per frame
    private void OnPlayerEnter(object source, EventArgs args)
    {
        if (ResettingAlerted != null)
        {
            StopCoroutine(ResettingAlerted);
            ResettingAlerted = null;
        }
        TriggeringAlerted = StartCoroutine(GettingAlerted());
    }

    private IEnumerator GettingAlerted()
    {
        yield return new WaitForSeconds(TriggerTime);
        SetAlerted(true);
        Exclamation.SetActive(true);
    }

    private void OnPlayerExit(object source, EventArgs args)
    {
        if (TriggeringAlerted != null)
        {
            StopCoroutine(TriggeringAlerted);
            TriggeringAlerted = null;
        }
        ResettingAlerted = StartCoroutine(ResetAlerted());
    }

    private IEnumerator ResetAlerted()
    {
        yield return new WaitForSeconds(ResettingTime);
        SetAlerted(false);
        Exclamation.SetActive(false);
        ResettingAlerted = null;
    }
}
