using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Luzia : MonoBehaviour
{
    // Coisas da Luzia inicio

    public static Luzia Instance;

    public float MoveSpeed = 3f; // movimento do jogador.

    private bool correndo; // verdadeiro ou falso para o estado correndo.

    public Rigidbody2D rb; // define que rigidbody2d seja o rb.

    Vector2 movement; // define que o vector2 e o movimento.

    public bool Luziaparada = false;

    // Coisas da Luzia fim

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // não destroi o player ao carregar uma cena nova
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 1)
        {
            Destroy(gameObject);
        }
    }

    private void Moremove()
    {
        if (correndo)
        {
            {
                MoveSpeed = 5f; // se nao moveSpeed = 3.
            }
        }
        else
        {
            {
                MoveSpeed = 3f; // se nao moveSpeed = 3.
            }
        }
    }

    private void Destruircamera() // destroi a camera de um level que n seja a camera principal.
    {
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera"); // procura um gameobject com a tag "MainCamera"

        if (cam != null) // se existir um que tenha a tag
        {
            Destroy(cam); // destroi ela
        }
    }

    void Update()
    {
        if (Luziaparada)
        {
            MoveSpeed = 0f;
        }
        else
        {
            Moremove();
        }

        movement.x = Input.GetAxisRaw("Horizontal"); // pega o o botao A e D.
        movement.y = Input.GetAxisRaw("Vertical"); // pega o o botao W e S.
        correndo = Input.GetKey(KeyCode.LeftShift); // pega o o botao left shift.

        Destruircamera();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * MoveSpeed * Time.fixedDeltaTime); //faz o personagem se mover.
    }

    public void Luziapara()
    {
        Luziaparada = true;
    }

    public void Luziavolta()
    {
        Luziaparada = false;
        Debug.Log("voltou");
        Debug.Log(Luziaparada);
    }
}