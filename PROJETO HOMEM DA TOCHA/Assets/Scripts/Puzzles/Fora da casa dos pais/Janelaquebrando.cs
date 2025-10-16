using UnityEngine;
using UnityEngine.SceneManagement;

public class Janelaquebrando : MonoBehaviour
{
    public GameObject BotaoE;
    private SpriteRenderer botaoSR;
    private bool podeinteragir;
    public InternalIventory ScriptIntInv;

    void Start()
    {
        BotaoE = GameObject.FindGameObjectWithTag("BotaoEicon");
        ScriptIntInv = FindAnyObjectByType<InternalIventory>();

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

    public void Update()
    {
        if (podeinteragir && Input.GetKeyDown(KeyCode.E))
        {
            ScriptIntInv.Tpjanela();
        }
    }
}
