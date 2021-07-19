using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Final : MonoBehaviour
{
    public TMP_Text textoResultado;
    [Multiline]
    public string textoVitoria;
    [Multiline]
    public string textoDerrota;

    void Start()
    {
        textoResultado.text = GameManager.instancia.Resultado() ? textoVitoria : textoDerrota;
    }
}
