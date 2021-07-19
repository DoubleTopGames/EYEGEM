using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.instancia.SelecionarCena(GameManager.instancia.IndexCenaAtual() + 1);
        }
    }
}
