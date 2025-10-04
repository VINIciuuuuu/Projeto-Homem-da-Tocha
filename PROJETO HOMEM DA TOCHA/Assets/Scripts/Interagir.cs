using UnityEngine;
using UnityEngine.UIElements;

public class Interagir : MonoBehaviour
{
    private bool podeinteragir;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            podeinteragir = true;
        }
        else
        {
            podeinteragir = false;
        }
    }
    private void Update()
    {
        if (podeinteragir && Input.GetKeyDown(KeyCode.E))
        {
            Destroy(gameObject);
        }
    }
}
