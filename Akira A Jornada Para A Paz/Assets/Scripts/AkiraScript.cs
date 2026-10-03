using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class AkiraScript : MonoBehaviour
{
    Rigidbody2D rb;
    VidaManagerScript vm;


    private float Direcao;

    private int Vida = 3;

    public float speed = 10f;
    public float MaxSpeed = 5f;
    public float jumpForce = 5f;

    public PhysicsMaterial2D Chao;
    public PhysicsMaterial2D Parede;

    public Collider2D col;

    public bool CanJump = true;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vm = FindFirstObjectByType<VidaManagerScript>();
    }

   
    void Update()
    {
        bool QuerParar = Direcao == 0;
        col.sharedMaterial = QuerParar ? Chao : Parede;
        Debug.Log(rb.linearVelocityX);
    }
    private void FixedUpdate()
    {
        rb.AddForce(new Vector2(Direcao * speed, 0), ForceMode2D.Force);
        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -MaxSpeed, MaxSpeed);
    }

    public void Move(InputAction.CallbackContext value)
    {
       Direcao = value.ReadValue<float>();
    }

    public void Jump(InputAction.CallbackContext value)
    {
        if (value.performed && CanJump)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            CanJump = false;
        }
    }
    private void ReceberDano(int dano)
    {
        Vida -= dano;
        vm.AtualizarVida(Vida);
        if(Vida <= 0)
        {
            Morrendo();
        }
    }

    private void ReceberCura(int cura)
    {
        Vida += cura;
        vm.AtualizarVida(Vida);
    }

    private void Morrendo()
    {
        Debug.Log("Akira morreu");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Chão"))
        {
            CanJump = true;
        }
        if (collision.gameObject.CompareTag("Inimigo"))
        {
            ReceberDano(1);
        }
        if (collision.gameObject.CompareTag("Cura"))
        {
            ReceberCura(1);
        }
    }
}
