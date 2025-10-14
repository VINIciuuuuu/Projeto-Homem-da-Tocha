using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TpAndando : MonoBehaviour
{
    public int cenatp = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadSceneAsync(cenatp);
        }
    }
}
