using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Interagir : MonoBehaviour
{
    [SerializeField] private int sceneDialogo = 15;
    private bool dialogoAberto = false;
    private bool podeinteragir;
    [Header("Acão do objeto")]
    [Tooltip("Olhe no código para ver cada ação")]
    public int action;
    public int cena;
    public Luzia scriptLuzia;
    public GameObject BotaoE;
    private SpriteRenderer botaoSR;
    public Cadeado CadScript;
    public GameObject PapelSenha;
    bool Horadopapel = false;

    private SpriteRenderer papelSR;

    /*
    Todas as a ações:

    0 - Destroi o objeto.
    1 - Interagir resulta em trocar de cena.
    2 - interação com NPC.
    3 - interação com o papel da senha.
    4 - interação com cadeado.
     o resto ainda vamos fazer

    */

    private void Start()
    {
        CadScript = FindAnyObjectByType<Cadeado>();
        scriptLuzia = FindAnyObjectByType<Luzia>();
        BotaoE = GameObject.FindGameObjectWithTag("BotaoEicon");
        PapelSenha = GameObject.FindGameObjectWithTag("Papeldasenha");

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
        if (PapelSenha != null)
        {
            papelSR = PapelSenha.GetComponent<SpriteRenderer>();
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
            dialogoAberto = false;
            Color cor = botaoSR.color;
            cor.a = 0f;
            botaoSR.color = cor;
        }
    }

    private void Update()
    {
        if (Horadopapel)
        {
            scriptLuzia.MoveSpeed = 0f;
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
            {
                Papeloff();
                return;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
        {
            Horadopapel = false;
        }

        if (podeinteragir && Input.GetKeyDown(KeyCode.E )|| podeinteragir && Input.GetKeyDown(KeyCode.Return))
        {
            if (action == 0)
            {
                Destroy(gameObject);
            }
            
            if (action == 1)
            {
                SceneManager.LoadSceneAsync(cena);
            }

            if (action == 2)
            {
                if (!dialogoAberto)
                {
                    SceneManager.LoadScene(sceneDialogo, LoadSceneMode.Additive);
                    dialogoAberto = true;
                }
            }
            if (action == 3)
            {
                Color corPapel = papelSR.color;
                corPapel.a = 1f;
                papelSR.color = corPapel;

                Horadopapel = true;
            }

            if (action == 4)
            {
                CadScript.Comecarcadeado();
            }
        }
        // fora da lista
        if (dialogoAberto)
        {
            Color cor = botaoSR.color;
            cor.a = 0f;
            botaoSR.color = cor;
        }
    }

    public void DialogoFechado()
    {
        dialogoAberto = false;
        SceneManager.UnloadSceneAsync(15);
        //Está função é para resetar a bool dialogoAberto para que outro possa ser aberto
        //MoveSpeed volta ao normal quando o dialogo é fechado
        //UnloadSceneAsyn fecha a cena de dialog q foi aberta
        //Está função será aberta no DialogManager da cena de dialogo.
    }

    public void Papeloff()
    {
        Horadopapel = false;
        scriptLuzia.MoveSpeed = 3f;
        Color corPapel = papelSR.color;
        corPapel.a = 0f;
        papelSR.color = corPapel;
    }

    public void Papelon()
    {

    }
}
