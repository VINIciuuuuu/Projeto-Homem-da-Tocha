using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Porta : MonoBehaviour
{
    public GameObject BotaoE;
    private SpriteRenderer botaoSR;
    private bool podeinteragir;
    public int cena = 0;

    void Start()
    {
        BotaoE = GameObject.FindGameObjectWithTag("BotaoEicon");

        if (BotaoE != null)
        {
            botaoSR = BotaoE.GetComponent<SpriteRenderer>();

            if (botaoSR != null)
            {
                Color cor = botaoSR.color;
                cor.a = 0f;
                botaoSR.color = cor;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            podeinteragir = true;
            Color cor = botaoSR.color;
            cor.a = 1f;
            botaoSR.color = cor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            podeinteragir = false;
            Color cor = botaoSR.color;
            cor.a = 0f;
            botaoSR.color = cor;
        }
    }

    void Update()
    {
        if (podeinteragir && Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadSceneAsync(cena);
        }
    }
}
