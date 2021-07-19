using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
    }

    public Animator transicao;

    bool carregando;
    List<AudioSource> objsSom = new List<AudioSource>();
    bool resultado;

    void Start()
    {
        Application.targetFrameRate = 60;
        transicao.SetTrigger("Sair");
    }

    public void SelecionarCena(int indexCena)
    {
        if (!carregando)
        {
            if (indexCena < SceneManager.sceneCountInBuildSettings)
                StartCoroutine(CarregarCena(indexCena));
            else
                StartCoroutine(CarregarCena(0));
        }
    }

    public void FimJogo(bool vitoria)
    {
        resultado = vitoria;
        SelecionarCena(2);
    }

    public bool Resultado()
    {
        return resultado;
    }

    public int IndexCenaAtual()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public bool Carregando()
    {
        return carregando;
    }

    IEnumerator CarregarCena(int cena)
    {
        carregando = true;
        transicao.SetTrigger("Entrar");

        yield return new WaitForSeconds(1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(cena);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
                yield return new WaitForSeconds(0.15f);
                transicao.SetTrigger("Sair");
                carregando = false;
            }

            yield return null;
        }
    }

    public void ReproduzirEfeito(AudioClip clip, float volume, bool pitchAleatorio)
    {
        AudioSource _source = ObjSom();
        _source.clip = clip;
        _source.volume = volume;
        if(pitchAleatorio)
            _source.pitch = Random.Range(0.75f, 1.25f);
        _source.Play();
    }

    AudioSource ObjSom()
    {
        if (objsSom.Count > 0)
        {
            for (int i = 0; i < objsSom.Count; i++)
            {
                if (!objsSom[i].isPlaying)
                {
                    objsSom[i].transform.position = transform.position;
                    return objsSom[i];
                }
            }
        }

        GameObject som = new GameObject("Som");
        som.transform.position = transform.position;
        som.transform.parent = transform;
        AudioSource _source = som.AddComponent<AudioSource>();
        objsSom.Add(_source);

        return _source;
    }
}
