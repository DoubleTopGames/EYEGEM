using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float velocidade;
    public Transform posicaoGema;
    public SpriteRenderer spritePlayer;
    public float velocidadeSubida;
    public int vidaMax;
    public List<GameObject> objsVida;
    public AudioClip somDano;
    public AudioClip somPegarGema;
    public AudioClip somSoltarGema;

    Rigidbody2D rb;
    Animator anim;
    Vector2 dirMovimento;
    List<GameObject> gemas = new List<GameObject>();
    List<GameObject> pedestais = new List<GameObject>();
    float posSprite = 0;
    int vida;
    PiscarSprite piscarSprite;

    void Start()
    {
        vida = vidaMax;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        piscarSprite = GetComponent<PiscarSprite>();
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        dirMovimento = new Vector2(inputX, inputY);

        anim.SetBool("Direita", inputX > 0);
        anim.SetBool("Esquerda", inputX < 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (posicaoGema.childCount > 0)
            {
                Transform gema = posicaoGema.GetChild(0);
                Transform pedestal = null;
                if (pedestais.Count > 0)
                {
                    pedestal = pedestais[0].GetComponent<Pedestal>().ColocarGema(gema.gameObject);
                }

                gema.parent = pedestal;
                gema.localRotation = Quaternion.Euler(Vector3.zero);
                if (pedestal != null)
                {
                    gema.localPosition = Vector2.zero;
                    gema.GetComponent<Gema>().AlterarColisao(false);
                }
                else
                    gema.GetComponent<Gema>().AlterarColisao(true);

                GameManager.instancia.ReproduzirEfeito(somSoltarGema, 0.15f, true);
            }
            else if (gemas.Count > 0)
            {
                PegarGema(gemas[0]);
            }
            else if (pedestais.Count > 0)
            {
                GameObject gema = pedestais[0].GetComponent<Pedestal>().RetirarGema();
                if (gema != null)
                    PegarGema(gema);
            }
        }

        posSprite += Time.deltaTime * velocidadeSubida;
        float y = (Mathf.Sin(posSprite * Mathf.Deg2Rad) / 8) + 0.125f;
        spritePlayer.transform.localPosition = new Vector2(spritePlayer.transform.localPosition.x, y);
    }

    void FixedUpdate()
    {
        rb.velocity = dirMovimento.normalized * Time.fixedDeltaTime * velocidade;
    }

    void PegarGema(GameObject gema)
    {
        gema.transform.parent = posicaoGema;
        gema.transform.localPosition = Vector2.zero;
        gema.transform.localRotation = Quaternion.Euler(Vector3.zero);
        gema.GetComponent<Gema>().AlterarColisao(false);

        GameManager.instancia.ReproduzirEfeito(somPegarGema, 0.15f, true);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.tag)
        {
            case "Gema":
                gemas.Add(col.gameObject);
                break;
            case "Pedestal":
                pedestais.Add(col.gameObject);
                break;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        switch (col.tag)
        {
            case "Gema":
                gemas.Remove(col.gameObject);
                break;
            case "Pedestal":
                pedestais.Remove(col.gameObject);
                break;
        }
    }

    public void PerderVida()
    {
        vida--;
        if (vida >= 0)
        {
            objsVida[vida].GetComponent<Image>().enabled = false;
            CameraShake.instancia.ComecarTremerCamera(0.25f, 1f);
            StartCoroutine(piscarSprite.Piscar(5));
            
            GameManager.instancia.ReproduzirEfeito(somDano, 0.15f, true);
        }

        if (vida <= 0)
        {
            gameObject.SetActive(false);
            GameManager.instancia.FimJogo(false);
        }
    }
}
