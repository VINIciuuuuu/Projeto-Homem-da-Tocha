using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Luzia : MonoBehaviour
{
    // Coisas da Luzia inicio

    public float MoveSpeed = 3f; // movimento do jogador.

    private bool correndo; // verdadeiro ou falso para o estado correndo.

    public Rigidbody2D rb; // define que rigidbody2d seja o rb.

    Vector2 movement; // define que o vector2 e o movimento.

    // Coisas da Luzia fim

    private void Moremove()
    {
        if (correndo)
        {
            if (movement.x != 0 && movement.y != 0) // se movimento x e movimento y estiver sendo forem apertados ao mesmo tempo, movespeed = 0.
            {
                MoveSpeed = 0f;
            }
            else
            {
                MoveSpeed = 5f; // se nao moveSpeed = 3.
            }
        }
        else
        {
            if (movement.x != 0 && movement.y != 0) // se movimento x e movimento y estiver sendo forem apertados ao mesmo tempo, movespeed = 0.
            {
                MoveSpeed = 0f;
            }
            else
            {
                MoveSpeed = 3f; // se nao moveSpeed = 3.
            }
        }
    }

    private void Destruircamera() // destroi a camera de um level que n seja a camera principal.
    {
        GameObject cam = GameObject.FindGameObjectWithTag("cameraruim"); // procura um gameobject com a tag "cameraruim"
        
        if (cam != null) // se existir um que tenha a tag
        {
            Destroy(cam); // destroi ela
        }
    }

    void Start()
    {

    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // não destroi o player ao carregar uma cena nova
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal"); // pega o o botao A e D.
        movement.y = Input.GetAxisRaw("Vertical"); // pega o o botao W e S.
        correndo = Input.GetKey(KeyCode.LeftShift); // pega o o botao left shift.

        Moremove(); // chama a funcao Moremove que esta escrita la em cima.
        Destruircamera(); // chama para destruia a camera de um level que n seja a camera principal.
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * MoveSpeed * Time.fixedDeltaTime); //faz o personagem se mover.
    }
}