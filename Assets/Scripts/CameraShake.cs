using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instancia;

    void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
    }

    public Camera mainCamera;

    Vector3 posicaoInicialCamera;
    bool tremendo;

    void Start()
    {
        posicaoInicialCamera = mainCamera.transform.position;
    }

    public void ComecarTremerCamera(float tempo, float intensidade)
    {
        if (!tremendo)
        {
            StartCoroutine(TremerCamera(tempo, intensidade));
        }
    }

    IEnumerator TremerCamera(float tempo, float intensidade)
    {
        tremendo = true;
        float suavizacaoTremida = intensidade / tempo;

        while (tempo > 0)
        {
            float cameraOffsetX = Random.Range(-1f, 1f) * intensidade;
            float cameraOffsetY = Random.Range(-1f, 1f) * intensidade;

            mainCamera.transform.position += new Vector3(cameraOffsetX, cameraOffsetY, 0);
            intensidade = Mathf.MoveTowards(intensidade, 0, suavizacaoTremida * Time.deltaTime);

            tempo -= Time.deltaTime;
            yield return null;
            mainCamera.transform.position = posicaoInicialCamera;
        }

        mainCamera.transform.position = posicaoInicialCamera;
        tremendo = false;
    }
}
