using System.Collections;
using UnityEngine;

public class FantasmaScript : MonoBehaviour
{
    public GameObject yurei;
    Animator an;

    private void Start()
    {
        an = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            an.SetTrigger("Spawn");
        }
    }

    public void destruir()
    {
        yurei.SetActive(true);
        Destroy(gameObject);
    }
}
