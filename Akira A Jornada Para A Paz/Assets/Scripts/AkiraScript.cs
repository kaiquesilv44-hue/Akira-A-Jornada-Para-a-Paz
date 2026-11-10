using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AkiraScript : MonoBehaviour
{
    Rigidbody2D rb;
    VidaManagerScript vm;
    Animator an;
    SpriteRenderer sr;
    InimigoBaseScript inimi;

    public float Direcao;          // direção efetiva usada pelo personagem (lida por outros scripts)
    private float inputDirecao;    // o que o jogador está apertando de verdade

    private int Vida = 3;
    private int Arma = 1;

    private bool AtacandoP = false;
    private bool AtacandoA = false;
    private bool AtacandoD = false;
    private bool Imortal = false;

    public float speed = 10f;
    public float MaxSpeed = 5f;
    public float jumpForce = 5f;
    public GameObject ParticulaParede;
    public GameObject ParticulaChao;
    public Transform LocalPartParede;
    public Transform LocalPartChao;
    public Transform LocaldeAtaque;
    public GameObject AreadeAcerto;
    private bool Dashing = false;
    private bool CanDash = true;
    private bool Fumacando = false;

    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public PhysicsMaterial2D akira;
    public PhysicsMaterial2D parede;

    public Collider2D col;
    public Collider2D colAtaque;

    public bool CanJump = true;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vm = FindFirstObjectByType<VidaManagerScript>();
        an = GetComponent<Animator>();
        inimi = FindFirstObjectByType<InimigoBaseScript>();
        sr = GetComponent<SpriteRenderer>();
    }


    private void FixedUpdate()
    {
        if (Dashing || Vida <= 0) return;

        rb.AddForce(new Vector2(Direcao * speed, 0), ForceMode2D.Force);
        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -MaxSpeed, MaxSpeed);
    }

    private void Update()
    {
        // Durante o ataque parado e o dash o personagem não anda.
        // Quando terminam, Direcao volta a ler o que o jogador ainda está apertando,
        // sem precisar soltar e apertar a tecla de novo.
        Direcao = (AtacandoP || AtacandoD) ? 0f : inputDirecao;

        an.SetBool("andando", false);
        if (Direcao > 0 && Vida > 0)
        {
            transform.localScale = new Vector3(4, 4, 1);
            an.SetBool("andando", true);
        }
        else if (Direcao < 0 && Vida > 0)
        {
            transform.localScale = new Vector3(-4, 4, 1);
            an.SetBool("andando", true);
        }

        if (Direcao == 0)
        {
            col.sharedMaterial = akira;
            an.SetBool("atacandoA", false);
        }
        else
        {
            col.sharedMaterial = parede;
            StartCoroutine(Fumaça());
        }
    }

    public void Move(InputAction.CallbackContext value)
    {
        inputDirecao = value.ReadValue<float>();
    }

    public void Jump(InputAction.CallbackContext value)
    {
        if (value.performed && CanJump && Vida > 0)
        {
            an.SetBool("pulo", true);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            CanJump = false;
        }

        if (value.canceled && rb.linearVelocityY > 0)
        {
            rb.linearVelocityY *= 0.2f;
        }
    }


    //Codigos de Ataque!
    public void Atacar(InputAction.CallbackContext value)
    {
        if (Vida <= 0) return;
        switch (Arma)
        {
            case 1:
                // ataque parado (não dispara durante o dash)
                if (value.performed && !AtacandoP && Direcao == 0 && !Dashing)
                {
                    AtacandoP = true;
                    an.SetBool("atacandoP", true);
                    CanDash = false;
                    StartCoroutine(AtacandoPCoroutine());
                }
                // ataque andando
                if (value.performed && AtacandoA == false && Direcao != 0 && CanDash && CanJump)
                {
                    AtacandoA = true;
                    an.SetBool("atacandoA", true);
                    StartCoroutine(AtacandoACoroutine());
                }
                // ataque durante o dash
                if (value.performed && AtacandoD == false && Dashing)
                {
                    AtacandoD = true;
                    an.SetBool("atacandoD", true);
                    StartCoroutine(AtacandoDCoroutine());
                }
                break;
            case 2:
                Debug.Log("Atacando com a Arco");
                break;
            case 3:
                Debug.Log("Atacando com a Bastão");
                break;
            case 4:
                Debug.Log("Atacando com a Kusarigama");
                break;
        }
    }

    public void TrocarArma(InputAction.CallbackContext context)
    {
        switch (context.control.name)
        {
            case "upArrow":
                Arma = 1;
                break;
            case "leftArrow":
                Arma = 2;
                Debug.Log("Arma atual: " + Arma);
                break;
            case "downArrow":
                Arma = 3;
                Debug.Log("Arma atual: " + Arma);
                break;
            case "rightArrow":
                Arma = 4;
                Debug.Log("Arma atual: " + Arma);
                break;
        }
    }

    public void ReceberDano(int dano)
    {
        StartCoroutine(receberDano());
        if (Vida >= 1)
        {
            Vida -= dano;
            vm.AtualizarVida(Vida);
            if (Vida <= 0)
            {
                Morrendo();
            }
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
        an.SetBool("vivo", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            ReceberDano(10);
        }

        if (collision.gameObject.CompareTag("Chão"))
        {
            CanJump = true;
            an.SetBool("pulo", false);
        }
        if (collision.gameObject.CompareTag("Parede"))
        {
            CanJump = true;
            an.SetBool("deslizando", true);
        }
        if (collision.gameObject.CompareTag("Inimigo") && !Imortal && !AtacandoA && !AtacandoD && !AtacandoP)
        {
            ReceberDano(1);
            StartCoroutine(receberDano());
        }
        if (collision.gameObject.CompareTag("Cura"))
        {
            ReceberCura(1);
        }
        if (collision.gameObject.CompareTag("Porta"))
        {
            SceneManager.LoadScene("Fase1");
        }
        if (collision.gameObject.CompareTag("Inimigo") && (AtacandoA || AtacandoD || AtacandoP))
        {
            collision.gameObject.GetComponent<InimigoBaseScript>().ReceberDano(1);
        }
        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Yurei") && (AtacandoA || AtacandoD || AtacandoP))
        {
            collision.gameObject.GetComponent<YureiScript>().ReceberDano(1);
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Parede"))
        {
            an.SetBool("deslizando", false);
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && CanDash && Vida > 0)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    public void Deslizando()
    {
        Instantiate(ParticulaParede, LocalPartParede.position, Quaternion.identity);
    }

    void AtualizarMovimento()
    {
        Direcao = (AtacandoP || AtacandoD) ? 0f : inputDirecao;
        an.SetBool("andando", Direcao != 0 && Vida > 0);
    }

    IEnumerator DashCoroutine()
    {
        CanDash = false;
        Dashing = true;
        an.SetTrigger("dash");

        float gravidadeOriginal = rb.gravityScale;
        rb.gravityScale = 0f; // dash reto, sem cair

        float sentido = transform.localScale.x > 0 ? 1f : -1f;
        rb.AddForce(new Vector2(sentido * 10f, 0f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = gravidadeOriginal;
        Dashing = false;

        yield return new WaitForSeconds(dashCooldown);
        CanDash = true;
    }


    IEnumerator AtacandoPCoroutine()
    {
        yield return new WaitForSeconds(1.08f);
        AtacandoP = false;
        an.SetBool("atacandoP", false);
        CanDash = true;
        AtualizarMovimento();
    }

    IEnumerator AtacandoACoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        AtacandoA = false;
        an.SetBool("atacandoA", false);
    }

    IEnumerator AtacandoDCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        AtacandoD = false;
        an.SetBool("atacandoD", false);
        AtualizarMovimento();
    }

    IEnumerator Fumaça()
    {
        if (Fumacando || !CanJump) yield break;
        Fumacando = true;
        yield return new WaitForSeconds(0.2f);
        Fumacando = false;
        Instantiate(ParticulaChao, LocalPartChao.position, Quaternion.identity);
    }
    
    IEnumerator receberDano()
    {
        sr.color = Color.red;
        Imortal = true;
        yield return new WaitForSeconds(0.2f);
        sr.color = Color.white;
        yield return new WaitForSeconds(1f);
        Imortal = false;
    }

}