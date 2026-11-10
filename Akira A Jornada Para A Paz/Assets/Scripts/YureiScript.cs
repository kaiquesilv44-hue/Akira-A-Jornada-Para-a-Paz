using System.Collections;
using UnityEngine;

public class YureiScript : MonoBehaviour
{
    public int Vida = 6;
    public bool Imortal = false;
    [SerializeField] private Transform[] pontos; // arraste os 4 pontos aqui
    [SerializeField] private float velocidade = 3f;
    [SerializeField] private float distanciaMinima = 0.05f;
    public bool Atacando = false;
    Animator an;
    SpriteRenderer sr;
    AkiraScript Akira;

    private int indiceAtual = 0;

    private void Start()
    {
        if (pontos.Length > 0)
            transform.position = pontos[0].position;
        an = GetComponent<Animator>();
        Akira = FindFirstObjectByType<AkiraScript>();
        StartCoroutine(ataque());
        sr = GetComponent<SpriteRenderer>();
    }

    public void ReceberDano(int dano)
    {
        if (Imortal) return;
        Vida -= dano;
        if (Vida <= 0)
        {
            Debug.Log("Inimigo morreu.");
            an.SetTrigger("Morrendo");
            return;
        }
        if (Vida <= 0)
            return;
        StartCoroutine(receberDano());
        Debug.Log("Inimigo recebeu dano. Vida atual: " + Vida);
    }

    private void Update()
    {
        if (!Atacando)
        {
            if (pontos.Length == 0) return;

            Transform alvo = pontos[indiceAtual];
            transform.position = Vector3.MoveTowards(transform.position, alvo.position, velocidade * Time.deltaTime);

            if (Vector3.Distance(transform.position, alvo.position) < distanciaMinima)
            {
                indiceAtual = (indiceAtual + 1) % pontos.Length;
            }
        }
        else
        {
            Transform alvo = Akira.transform;
            transform.position = Vector3.MoveTowards(transform.position, alvo.position, 8 * Time.deltaTime);
        }
    }
    IEnumerator receberDano()
    {
        sr.color = Color.red;
        Imortal = true;
        yield return new WaitForSeconds(0.5f);
        sr.color = Color.white;
        yield return new WaitForSeconds(0.5f);
        Imortal = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Atacando)
        {
           an.SetTrigger("Atacandoo");
            Akira.ReceberDano(1);
            Atacando = false;
        }
    }

    private void Ataque()
    {
        Atacando = true;
        an.SetTrigger("Atacando");
    }
    public void Morrer()
    {
        Destroy(gameObject);
    }

    IEnumerator ataque()
    {
        int Temp = Random.Range(10, 20);
        yield return new WaitForSeconds(Temp);
        Ataque();
        StartCoroutine(ataque());
    }
}
