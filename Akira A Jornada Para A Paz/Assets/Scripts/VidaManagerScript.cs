using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class VidaManagerScript : MonoBehaviour
{
    private List <GameObject> vidas = new List<GameObject>();

    public GameObject vidaPrefab;
    
    private float vidaSpacing = 0f;

    void Start()
    {
        AtualizarVida(3);
    }

    public void AtualizarVida(int novaVida)
    {
        foreach(GameObject vida in vidas)
        {
            Destroy(vida);
        }
        vidas.Clear();
        for(int i = 0; i < novaVida; i++)
        {
            GameObject vida = Instantiate(vidaPrefab, transform);
            vida.GetComponent<RectTransform>().anchoredPosition = new Vector3(i * vidaSpacing, -5, 0);
            vidas.Add(vida);
            vidaSpacing = 80f;
        }
    }
}

 