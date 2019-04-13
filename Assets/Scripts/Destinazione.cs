using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destinazione : MonoBehaviour
{

    public string oggettoRichiesto="messaggio";
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag=="Player"){
            
            if (other.gameObject.GetComponent<PlayerInventario>().inventario.Exists(x => x==oggettoRichiesto))
                Destroy(gameObject);
            
        }
    }
}
