using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Padrao { EXPLOSAOSIMPLES, EXPLOSAOCONTINUA, EXPLOSAOALTERNADA, GIROUNICO, TRESGIROS, ALEATORIO };

public class Boss : MonoBehaviour
{
    public Transform player;
    public Transform olho;
    public float distOlho;
    public int qtdPosicoes1Fase;
    public int qtdPosicoes2Fase;
    public float distanciaPonto;
    public float distanciaIntencao;
    public GameObject objTiro;
    public List<Padrao> padroesFase1;
    public List<Padrao> padroesFase2;
    public float velFase2;
    public float intervaloTiros;
    public float velocidadeTiro;
    public int vidaMax;
    public int vidaSegundaFase;
    public SpriteRenderer spriteBoss;
    public AudioClip somTiro;
    public AudioClip somDano;
    public AudioClip somIndicacao;
    public GameObject indicacao;

    List<Transform> posicoesTiro = new List<Transform>();
    bool disparando;
    float mult = 1;
    float tempoTiro;
    List<Padrao> padroes = new List<Padrao>();
    List<GameObject> tiros = new List<GameObject>();
    int vida;
    int qtdPosicoes;
    int indexPadrao;
    PiscarSprite piscarSprite;
    bool acordado = false;

    void Start()
    {
        vida = vidaMax;

        qtdPosicoes = qtdPosicoes1Fase;
        posicoesTiro = GerarPontos(qtdPosicoes, distanciaPonto);
        tempoTiro = intervaloTiros;
        padroes.AddRange(padroesFase1);
        RandomizarPadroes();

        piscarSprite = GetComponent<PiscarSprite>();
        olho.gameObject.SetActive(false);
    }

    void Update()
    {
        if (acordado)
        {
            Vector2 posicaoOlho = piscarSprite.piscando ? Vector2.zero : (Vector2)(player.position - transform.position).normalized * distOlho;
            olho.localPosition = posicaoOlho;

            if (Input.GetKeyDown(KeyCode.G))
                PerderVida();

            if (!disparando)
            {
                tempoTiro -= Time.deltaTime * mult;
                if (tempoTiro <= 0)
                {
                    Padrao padraoEscolhido = padroes[indexPadrao];

                    disparando = true;
                    tempoTiro = intervaloTiros;

                    IEnumerator padrao = null;

                    switch (padraoEscolhido)
                    {
                        case Padrao.EXPLOSAOSIMPLES:
                            padrao = ExplosaoSimples();
                            break;
                        case Padrao.EXPLOSAOCONTINUA:
                            padrao = ExplosaoContinua();
                            break;
                        case Padrao.EXPLOSAOALTERNADA:
                            padrao = ExplosaoAlternada();
                            break;
                        case Padrao.GIROUNICO:
                            padrao = GiroUnico();
                            break;
                        case Padrao.TRESGIROS:
                            padrao = TresGiros();
                            break;
                        case Padrao.ALEATORIO:
                            padrao = Aleatorio();
                            break;
                    }

                    StartCoroutine(MostrarIntencao(padrao, (int)padraoEscolhido + 1));

                    if (indexPadrao == padroes.Count - 1)
                        RandomizarPadroes();
                    else
                        indexPadrao++;
                }
            }
        }
    }

    void RandomizarPadroes()
    {
        indexPadrao = 0;
        List<Padrao> clone = new List<Padrao>();
        int tamanho = padroes.Count;
        for (int i = 0; i < tamanho; i++)
        {
            Padrao _padrao = padroes[Random.Range(0, padroes.Count)];
            clone.Add(_padrao);
            padroes.Remove(_padrao);
        }

        padroes = new List<Padrao>(clone);
    }

    List<Transform> GerarPontos(int qtd, float distancia)
    {
        List<Transform> pontos = new List<Transform>();
        // float angulo = 360f / qtd / 2;
        // float incremento = 360f / qtd;

        float angulo = Mathf.Round((360f / qtd / 2) * 100) / 100;
        float incremento = angulo * 2;

        for (int i = 0; i < qtd; i++)
        {
            float rad = angulo * Mathf.Deg2Rad;
            Vector2 posPonto = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized * distancia;
            GameObject novaPosicao = new GameObject("Posicao(" + (i + 1) + ") - " + qtd);
            novaPosicao.transform.parent = transform;
            novaPosicao.transform.localPosition = posPonto;
            pontos.Add(novaPosicao.transform);

            angulo += incremento;
        }

        return pontos;
    }

    GameObject NovoTiro()
    {
        for (int i = 0; i < tiros.Count; i++)
        {
            if (!tiros[i].activeSelf)
                return tiros[i];
        }

        for (int i = 0; i < 5; i++)
        {
            GameObject tiro = Instantiate(objTiro, Vector2.zero, Quaternion.identity);
            tiros.Add(tiro);
            tiro.SetActive(false);
        }

        return tiros[tiros.Count - 1];
    }

    void Disparar(Transform posicao)
    {
        GameObject tiro = NovoTiro();
        tiro.transform.position = posicao.position;
        tiro.SetActive(true);
        tiro.GetComponent<Tiro>().Atirar((posicao.position - transform.position).normalized * velocidadeTiro * mult);
    }

    IEnumerator MostrarIntencao(IEnumerator padrao, int intencao)
    {
        List<Transform> pontos = GerarPontos(intencao, distanciaIntencao);
        foreach (Transform ponto in pontos)
        {
            Instantiate(indicacao, ponto.position, Quaternion.identity, ponto);
        }

        GameManager.instancia.ReproduzirEfeito(somIndicacao, 0.2f, true);

        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < pontos.Count; i++)
        {
            Destroy(pontos[i].gameObject);
        }

        pontos.Clear();

        StartCoroutine(padrao);
    }

    IEnumerator ExplosaoSimples()
    {
        AtirarTodos();
        yield return new WaitForSeconds(3f / mult);
        disparando = false;
    }

    IEnumerator GiroUnico()
    {
        StartCoroutine(Giro());
        yield return new WaitForSeconds(1.5f / mult);
        disparando = false;
    }

    IEnumerator ExplosaoContinua()
    {
        for (int i = 0; i < 5; i++)
        {
            AtirarTodos();
            yield return new WaitForSeconds(0.75f / mult);
        }

        yield return new WaitForSeconds(2f / mult);
        disparando = false;
    }

    IEnumerator TresGiros()
    {
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(Giro());
            yield return new WaitForSeconds(1.25f);
        }

        yield return new WaitForSeconds(2f / mult);
        disparando = false;
    }

    IEnumerator Aleatorio()
    {
        List<Transform> posicoes = new List<Transform>(PosicoesAleatorias());
        int indexPosicao = 0;

        for (int i = 0; i < 30; i++)
        {
            Disparar(posicoes[indexPosicao]);

            ReproduzirSomTiro();

            if (indexPosicao == posicoes.Count - 1)
            {
                posicoes = new List<Transform>(PosicoesAleatorias());
                indexPosicao = 0;
            }
            else
                indexPosicao++;

            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(3f / mult);
        disparando = false;
    }

    List<Transform> PosicoesAleatorias()
    {
        List<Transform> posicoes = new List<Transform>(posicoesTiro);
        List<Transform> clone = new List<Transform>();

        for (int i = posicoesTiro.Count; i > 0; i--)
        {
            Transform posicao = posicoes[Random.Range(0, posicoes.Count)];
            clone.Add(posicao);
            posicoes.Remove(posicao);
        }

        return clone;
    }

    IEnumerator ExplosaoAlternada()
    {
        bool par = true;
        List<Transform> posicoesPar = new List<Transform>();
        List<Transform> posicoesImpar = new List<Transform>();
        for (int i = 0; i < posicoesTiro.Count; i++)
        {
            if (i % 2 == 0)
                posicoesPar.Add(posicoesTiro[i]);
            else
                posicoesImpar.Add(posicoesTiro[i]);
        }

        for (int i = 0; i < 10; i++)
        {
            List<Transform> posicoes = new List<Transform>(par ? posicoesPar : posicoesImpar);
            foreach (Transform posicao in posicoes)
            {
                Disparar(posicao);
            }
            par = !par;
            ReproduzirSomTiro();
            yield return new WaitForSeconds(0.5f / mult);
        }

        yield return new WaitForSeconds(2f / mult);
        disparando = false;
    }

    void AtirarTodos()
    {
        foreach (Transform posicao in posicoesTiro)
        {
            Disparar(posicao);
        }
        ReproduzirSomTiro();
    }

    IEnumerator Giro()
    {
        foreach (Transform posicao in posicoesTiro)
        {
            Disparar(posicao);
            ReproduzirSomTiro();
            yield return new WaitForSeconds(0.15f / mult);
        }
    }

    public void PerderVida()
    {
        acordado = true;
        olho.gameObject.SetActive(true);

        StopAllCoroutines();
        disparando = false;

        if (vida-- <= 0)
            GameManager.instancia.FimJogo(true);

        StartCoroutine(piscarSprite.Piscar(6));
        CameraShake.instancia.ComecarTremerCamera(0.2f, 0.75f);
        GameManager.instancia.ReproduzirEfeito(somDano, 0.15f, true);

        if (vida == vidaSegundaFase)
            EntrarSegundaFase();
    }

    void EntrarSegundaFase()
    {
        padroes.Clear();
        padroes.AddRange(padroesFase2);
        mult = velFase2;

        List<Transform> posicoesRemover = new List<Transform>(posicoesTiro);

        for (int i = 0; i < posicoesRemover.Count; i++)
        {
            Destroy(posicoesRemover[i].gameObject);
        }

        qtdPosicoes = qtdPosicoes2Fase;
        posicoesTiro.Clear();
        posicoesTiro = GerarPontos(qtdPosicoes, distanciaPonto);

        RandomizarPadroes();
    }

    void ReproduzirSomTiro()
    {
        GameManager.instancia.ReproduzirEfeito(somTiro, 0.125f, true);
    }
}
