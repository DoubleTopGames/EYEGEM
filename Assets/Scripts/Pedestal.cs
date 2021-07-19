using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    public Transform posicaoGema;
    public Gema gemaAtual;
    public List<Sprite> spritesPrevia;
    public SpriteRenderer previa;
    public float acrescimoIntervalo;
    public LayerMask verificarGema;
    public AudioClip somTrocar;
    public bool iniciarComGema;
    public Animator simbolo;

    TipoGema tipoAlvo;
    bool interagir = true;
    float xMin;
    float xMax;
    float yMin;
    float yMax;
    bool randomizando;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        AlterarTipoAlvo();

        if(iniciarComGema)
        {
            gemaAtual.transform.parent = ColocarGema(gemaAtual.gameObject);
            gemaAtual.transform.localRotation = Quaternion.Euler(Vector3.zero);
            gemaAtual.transform.localPosition = Vector2.zero;
            gemaAtual.AlterarColisao(false);
        }
    }

    void AlterarTipoAlvo()
    {
        tipoAlvo = (TipoGema)Random.Range(0, 4);
        gemaAtual.AlterarTipo(tipoAlvo);

        previa.sprite = spritesPrevia[(int)tipoAlvo];
    }

    public Transform ColocarGema(GameObject _gema)
    {
        Transform localGema = null;

        if (interagir)
        {
            Gema gema = _gema.GetComponent<Gema>();
            if (gema.tipo == tipoAlvo && posicaoGema.childCount == 0)
            {
                localGema = posicaoGema;
                gemaAtual = gema;
                previa.enabled = false;
                GerenciadorPedestais.instancia.GemaColocada();
                simbolo.SetBool("Ativo", true);
            }
        }

        return localGema;
    }

    public GameObject RetirarGema()
    {
        GameObject gema = null;

        if (interagir)
        {
            if (posicaoGema.childCount > 0)
            {
                gema = posicaoGema.GetChild(0).gameObject;
                GemaRemovida();
            }
        }

        return gema;
    }

    public void DefinirXY(float _xMin, float _xMax, float _yMin, float _yMax)
    {
        xMin = _xMin;
        xMax = _xMax;
        yMin = _yMin;
        yMax = _yMax;
    }

    void GemaRemovida()
    {
        gemaAtual.transform.parent = null;
        gemaAtual = null;
        previa.enabled = true;
        simbolo.SetBool("Ativo", false);
        GerenciadorPedestais.instancia.GemaRetirada();
    }

    public IEnumerator LancarBoss()
    {
        interagir = false;

        yield return new WaitForSeconds(0.15f);
        StartCoroutine(RandomizarGema());
        while(randomizando)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        gemaAtual.VoarLonge(Vector2.zero);

        yield return new WaitForSeconds(0.65f);
        gemaAtual.VoarLonge(PosicaoVerificada());
        GemaRemovida();
    }

    IEnumerator RandomizarGema()
    {
        randomizando = true;
        float intervalo = 0.05f;
        for (int i = 0; i < 15; i++)
        {
            GameManager.instancia.ReproduzirEfeito(somTrocar, 0.1f, true);
            AlterarTipoAlvo();
            yield return new WaitForSeconds(intervalo + (i * acrescimoIntervalo));
        }
        randomizando = false;
    }

    Vector2 PosicaoVerificada()
    {
        for (int i = 0; i < 100; i++)
        {
            Vector2 posicaoVerificar = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax));
            if(!Physics2D.OverlapCircle(posicaoVerificar, 0.25f, verificarGema))
                return posicaoVerificar;
        }

        return new Vector2(5, 5);
    }

    public void Sumir()
    {
        animator.SetTrigger("Desativar");
    }

    public void Aparecer()
    {
        animator.SetTrigger("Ativar");
    }

    public void HabilitarInteracao()
    {
        interagir = true;
    }
}
