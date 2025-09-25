using UnityEngine;

public class Luzia : MonoBehaviour
{

    public float MoveSpeed = 3f;

    public Rigidbody2D rb;

    Vector2 movement;
    

    private void Travarluzia()
    {
        if (movement.x != 0 && movement.y != 0)
        {
            MoveSpeed = 0f;
        }
        else
        {
            MoveSpeed = 3f;
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        Travarluzia();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * MoveSpeed * Time.fixedDeltaTime);
    }
}
