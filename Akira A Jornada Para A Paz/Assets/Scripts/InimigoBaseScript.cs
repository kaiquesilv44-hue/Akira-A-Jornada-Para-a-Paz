using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class InimigoBaseScript : MonoBehaviour
{
    public int Vida = 3;
    public Transform Ponto1;
    public Transform Ponto2;
    private Transform PlayerTransform;
    private bool Indo = true;
    private bool Atacando = false;
    public Collider2D ColliderdeAtaque;
    public bool Imortal = false;
    SpriteRenderer sr;
    Rigidbody2D rb;

    public void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Ponto1.SetParent(null);
        Ponto2.SetParent(null);
    }
    public void Update()
    {
       if (Indo && !Atacando)
        {
            transform.position = Vector2.MoveTowards(transform.position, Ponto2.position, 2 * Time.deltaTime);
            if (Vector2.Distance(transform.position, Ponto2.position) < 0.1f)
            {
                Indo = false;
            }
        }
        else if (!Indo && !Atacando)
        {
            transform.position = Vector2.MoveTowards(transform.position, Ponto1.position, 2 * Time.deltaTime);
            if (Vector2.Distance(transform.position, Ponto1.position) < 0.1f)
            {
                Indo = true;
            }
        }

       if (Atacando)
        {
            transform.position = Vector2.MoveTowards(transform.position, PlayerTransform.position, 2 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Atacando = true;
            PlayerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Atacando = false;
            PlayerTransform = null;
        }
    }
    
    public void ReceberDano(int dano)
    {
        if(Imortal) return;
        Vida -= dano;
        if (Vida <= 0)
        {
            Debug.Log ("Inimigo morreu.");
            Morrer();
            return;
        }
        if (Vida <= 0)
            return;
        StartCoroutine(receberDano());
        Debug.Log("Inimigo recebeu dano. Vida atual: " + Vida);
    }

    void Morrer()
    {
        Destroy(gameObject);
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
}
