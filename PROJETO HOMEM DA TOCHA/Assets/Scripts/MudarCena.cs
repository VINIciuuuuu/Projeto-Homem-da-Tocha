using UnityEngine;
using UnityEngine.SceneManagement;

public class MudarCena : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) // detecta a colisão
    {
        if (collision.CompareTag("Player")) // se estiver com a tag player
        {
            SceneManager.LoadSceneAsync(2); // carrega a cena da cidade
        }
    }
}
