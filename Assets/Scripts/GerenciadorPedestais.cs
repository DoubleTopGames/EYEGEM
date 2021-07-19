using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GerenciadorPedestais : MonoBehaviour
{
    public static GerenciadorPedestais instancia;

    void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
    }

    public List<Pedestal> pedestais;
    public List<Transform> posicoesPedestais;
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;
    public Boss boss;

    int gemasColocadas;

    void Start()
    {
        foreach (Pedestal pedestal in pedestais)
        {
            pedestal.DefinirXY(xMin, xMax, yMin, yMax);
        }

        MudarPosicaoPedestais();
    }

    public void GemaColocada()
    {
        gemasColocadas++;
        if (gemasColocadas == pedestais.Count)
        {
            StartCoroutine(AtacarBoss());
        }
    }

    public void GemaRetirada()
    {
        gemasColocadas--;
    }

    IEnumerator AtacarBoss()
    {
        foreach (Pedestal pedestal in pedestais)
        {
            StartCoroutine(pedestal.LancarBoss());
        }

        yield return new WaitForSeconds(3.85f);

        boss.PerderVida();
        StartCoroutine(RandomizarPedestais());
    }

    IEnumerator RandomizarPedestais()
    {
        foreach (Pedestal pedestal in pedestais)
        {
            pedestal.Sumir();
        }

        yield return new WaitForSeconds(1f);

        MudarPosicaoPedestais();

        foreach (Pedestal pedestal in pedestais)
        {
            pedestal.Aparecer();
        }
    }

    void MudarPosicaoPedestais()
    {
        List<Transform> posicoes = new List<Transform>(posicoesPedestais);
        foreach (Pedestal pedestal in pedestais)
        {
            Transform posicao = posicoes[Random.Range(0, posicoes.Count)];
            pedestal.transform.position = posicao.position;
            posicoes.Remove(posicao);
        }
    }
}
