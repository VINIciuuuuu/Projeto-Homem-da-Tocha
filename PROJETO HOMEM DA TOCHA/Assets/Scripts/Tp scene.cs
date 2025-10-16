using UnityEngine;

public class TeleportToObject : MonoBehaviour
{
    public GameObject playergeral;

    public Transform player;

    public Transform destino;

    private bool podeTeleportar;

    private void Awake()
    {
        playergeral = GameObject.FindGameObjectWithTag("Player");

        if (playergeral == null)
        {
            playergeral = GameObject.FindGameObjectWithTag("Player");
        }
        player = playergeral.transform;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.transform.position = destino.position;
        }
    }
}
