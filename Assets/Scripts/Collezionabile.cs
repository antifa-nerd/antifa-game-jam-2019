using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collezionabile : MonoBehaviour
{
    // Start is called before the first frame update

    public string nome = "messaggio";

    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag=="Player"){
            other.gameObject.GetComponent<PlayerInventario>().inventario.Add(nome);
            Destroy(gameObject);
        }
    }
}
