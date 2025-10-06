using UnityEngine;
using UnityEngine.SceneManagement;

public class MudarCena : MonoBehaviour
{
    public int cena = 0;

    private void OnTriggerEnter2D(Collider2D collision) // detecta a colisão
    {
        if (collision.CompareTag("Player")) // se estiver com a tag player
        {
            SceneManager.LoadSceneAsync(cena); // carrega a cena da cidade
        }
    }

    public void Menuinicial()
    {
        SceneManager.LoadSceneAsync(11);
    }
}
