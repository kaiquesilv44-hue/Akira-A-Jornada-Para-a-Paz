using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class AkiraScript : MonoBehaviour
{
    Rigidbody2D rb;


    private float Direcao;

    private int Life = 3;

    public float speed = 5f;
    public float jumpForce = 5f;

    public bool CanJump = true;


    void Start()
    {
     rb = GetComponent<Rigidbody2D>();
    }

   
    void Update()
    {
      
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(Direcao * speed, rb.linearVelocityY);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Chão"))
        {
            CanJump = true;
        }   
    }
}
