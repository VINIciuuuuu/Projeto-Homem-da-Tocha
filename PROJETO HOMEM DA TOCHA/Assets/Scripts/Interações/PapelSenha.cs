using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PapelSenha : MonoBehaviour
{
    public GameObject BotaoE;
    private SpriteRenderer botaoSR;
    private bool podeinteragir;
    public Luzia scriptLuzia;
    bool Horadopapel = false;
    private SpriteRenderer papelSR;
    public GameObject Paperpassword;

    void Start()
    {
        BotaoE = GameObject.FindGameObjectWithTag("BotaoEicon");
        Paperpassword = GameObject.FindGameObjectWithTag("Papeldasenha");
        scriptLuzia = FindAnyObjectByType<Luzia>();

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
        
        if (Paperpassword != null)
        {
            papelSR = Paperpassword.GetComponent<SpriteRenderer>();
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
        if (Horadopapel)
        {
            scriptLuzia.Luziapara();
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
            {
                Papeloff();
                return;
            }
            return;
        }

        if (podeinteragir && Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
        {
            Color corPapel = papelSR.color;
            corPapel.a = 1f;
            papelSR.color = corPapel;

            Horadopapel = true;
        }
    }

    public void Papeloff()
    {
        Horadopapel = false;
        scriptLuzia.LuziaVolta();
        Color corPapel = papelSR.color;
        corPapel.a = 0f;
        papelSR.color = corPapel;
    }
}