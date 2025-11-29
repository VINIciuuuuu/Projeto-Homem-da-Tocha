using UnityEngine;

public class NPCFollowPlayer : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    void Update()
    {
        if (player != null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, player.position) > 1f) 
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }
}
