using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PiscarTexto : MonoBehaviour
{
    [Multiline]
    public string textoExibir;
    public float tempoAceso;
    public float tempoApagado;

    TMP_Text texto;
    float piscar;
    
    void Start()
    {
        texto = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if(piscar <= 0)
        {
            texto.text = texto.text.Length == 0 ? textoExibir : "";
            piscar = texto.text.Length == 0 ? tempoApagado : tempoAceso;
        }
        else
            piscar -= Time.deltaTime;
    }
}
