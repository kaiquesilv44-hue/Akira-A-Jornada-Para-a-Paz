using UnityEngine;

public class SeguirFundoScript : MonoBehaviour
{
    AkiraScript Akira;
    public GameObject fundo;
    public Sprite Caverna;
    public Sprite Floresta;
    void Start()
    {
        Akira = FindFirstObjectByType<AkiraScript>();
    }
    void Update()
    {
        transform.position = Akira.transform.position;
    }

   public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Caverna"))
        {
            fundo.GetComponent<SpriteRenderer>().sprite = Caverna;
        }
        if (collision.gameObject.CompareTag("Floresta"))
        {
            fundo.GetComponent<SpriteRenderer>().sprite = Floresta;
        }
    }
}
