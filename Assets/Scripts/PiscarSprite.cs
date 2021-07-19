using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiscarSprite : MonoBehaviour
{
    public SpriteRenderer sprite;
    public bool piscando;

    public IEnumerator Piscar(int vezes)
    {
        piscando = true;

        for (int i = 0; i < vezes; i++)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.05f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
        
        piscando = false;
    }
}
