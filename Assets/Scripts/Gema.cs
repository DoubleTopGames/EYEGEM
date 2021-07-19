using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipoGema { RETANGULO, TRIANGULO, OCTOGONO, LOSANGO };

public class Gema : MonoBehaviour
{
    public List<Sprite> spritesGemas;
    public TipoGema tipo;
    public SpriteRenderer spriteGema;
    public float velocidade;
    public float multAltura;

    Collider2D col;
    bool movendo;
    Vector2 posInicial;
    Vector2 posAlvo;
    float passo;
    float tempoRestanteMover;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if(tempoRestanteMover > 0)
        {
            //transform.position = Vector2.MoveTowards(transform.position, posAlvo, passo * Time.deltaTime);
            transform.position = Vector2.Lerp(transform.position, posAlvo, passo * Time.deltaTime);
            tempoRestanteMover -= Time.deltaTime;

            spriteGema.transform.localPosition = new Vector2(0, Mathf.Sin(Mathf.Lerp(0, 179, 1 - tempoRestanteMover) *  Mathf.Deg2Rad) * multAltura);
        }
        else if(movendo)
        {
            movendo = false;
            transform.position = posAlvo;
            col.enabled = true;
        }
    }

    public void AlterarTipo(TipoGema novoTipo)
    {
        tipo = novoTipo;
        spriteGema.sprite = spritesGemas[(int)tipo];
    }

    public void AlterarColisao(bool habilitado)
    {
        col.enabled = habilitado;
    }

    public void VoarLonge(Vector2 posVoar)
    {
        posInicial = transform.position;
        posAlvo = posVoar;
        float distancia = Vector2.Distance(transform.position, posAlvo);
        passo = distancia * velocidade;
        tempoRestanteMover = distancia / passo;
        movendo = true;
    }
}
