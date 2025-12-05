using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target; // O objeto (jogador) a ser seguido. Arraste no Inspector.
    public float moveSpeed = 3f; // Velocidade do seguidor.
    public float stoppingDistance = 1f; // Dist�ncia m�nima para parar do alvo.
    public bool Corrabocao = false;
    private Animator animator;

    private Rigidbody2D rb;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Corrabocao = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Corrabocao = true;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D n�o encontrado no seguidor! Adicione um.");
            enabled = false;
            return;
        }
        if (target == null)
        {
            Debug.LogWarning("Target n�o atribu�do para o seguidor!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (Corrabocao)
        {
            moveSpeed = 5f;
        }
        else
        {
            moveSpeed = 3f;
        }
    }

    void FixedUpdate() // Use FixedUpdate para opera��es com Rigidbody2D
    {
        // Calcula a dist�ncia entre o seguidor e o alvo
        float distanceToTarget = Vector2.Distance(rb.position, target.position);

        if (distanceToTarget > stoppingDistance)
        {
            // Calcula a dire��o para o alvo
            Vector2 direction = (target.position - (Vector3)rb.position).normalized;


            if (direction.x > 0)
            {
                animator.SetBool("IsWalkiing", true);
            }
            else
            {
                animator.SetBool("IsWalkiing", true);
            }

            if (direction.y > 0)
            {
                animator.SetBool("IsWalkiing", true);
            }
            else
            {
                animator.SetBool("IsWalkiing", true);
            }

            animator.SetFloat("InputX", direction.x);
            animator.SetFloat("InputY", direction.y);

            // Define a velocidade do Rigidbody2D
            rb.linearVelocity = direction * moveSpeed;

            // Opcional: Rotacionar o seguidor para olhar para o alvo
            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // rb.rotation = angle;
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Para de se mover quando est� perto o suficiente
            animator.SetBool("IsWalkiing", false);
        }
    }
}