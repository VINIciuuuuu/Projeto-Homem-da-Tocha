using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Luzia : MonoBehaviour
{
    // Coisas da Luzia in�cio

    public float MoveSpeed = 3f; // movimento do jogador.

    private bool correndo; // verdadeiro ou falso para o estado correndo.

    // movimento fim

    public GameObject interactarea; // �rea de intera��o.

    public Rigidbody2D rb; // define que rigidbody2d seja o rb.

    Vector2 movement; // define que o vector2 � o movimento.

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
                MoveSpeed = 5f; // se n�o moveSpeed = 3.
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
                MoveSpeed = 3f; // se n�o moveSpeed = 3.
            }
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal"); // pega o o bot�o A e D.
        movement.y = Input.GetAxisRaw("Vertical"); // pega o o bot�o W e S.
        correndo = Input.GetKey(KeyCode.LeftShift); // pega o o bot�o left shift.

        Moremove(); // chama a fun��o Moremove que est� escrita l� em cima.
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * MoveSpeed * Time.fixedDeltaTime); //faz o personagem se mover.
    }
}
