using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiro : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 direcaoAtual;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void Atirar(Vector2 direcao)
    {
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();
        rb.velocity = direcao;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
            col.GetComponent<Player>().PerderVida();

        if(col.CompareTag("Player") || col.CompareTag("Barreira"))
            gameObject.SetActive(false);
    }
}
