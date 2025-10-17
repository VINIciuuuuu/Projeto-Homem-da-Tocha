using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class Cadeado : MonoBehaviour
{
    [Header("Referências da UI")]
    public GameObject painelUI;
    public TMP_InputField inputSenha;
    public Button botaoConfirmar;
    public TextMeshProUGUI mensagem;
    public string senhaCorreta = "3912";
    bool Cadeadoiniciado = false;
    public Luzia ScriptLuzia;
    public GameObject Moldura;
    private SpriteRenderer MolduraSR;
    private bool podeinteragirmoldura;
    public GameObject BotaoE;
    private SpriteRenderer botaoSR;
    private bool Terminocadeado = false;
    private bool podever = true;


    void Start()
    {
        Moldura = GameObject.FindGameObjectWithTag("MfamTuri");
        ScriptLuzia = FindAnyObjectByType<Luzia>();
        painelUI.SetActive(false);
        botaoConfirmar.onClick.AddListener(VerificarSenha);
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

        if (Moldura != null)
        {
            MolduraSR = Moldura.GetComponent<SpriteRenderer>();

            if (MolduraSR != null)
            {
                Molduraoff();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {  
        if (collision.CompareTag("Player"))
        {
            if (podever)
            {
                podeinteragirmoldura = true;
                Color cor = botaoSR.color;
                cor.a = 1f;
                botaoSR.color = cor;
            }
            else
            {
                Color cor = botaoSR.color;
                cor.a = 0f;
                botaoSR.color = cor;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            podeinteragirmoldura = false;
            Color cor = botaoSR.color;
            cor.a = 0f;
            botaoSR.color = cor;
        }
    }

    void Update()
    {
        if (!Terminocadeado && podeinteragirmoldura && Input.GetKeyDown(KeyCode.E) || !Terminocadeado && podeinteragirmoldura && Input.GetKeyDown(KeyCode.Return))
        {
            Comecarcadeado();
        }

        if (Cadeadoiniciado)
        {
            painelUI.SetActive(true);
            ScriptLuzia.Luziapara();
            Color cor = botaoSR.color;
            cor.a = 0f;
            botaoSR.color = cor;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            painelUI.SetActive(false);
            Cadeadoiniciado = false;
            ScriptLuzia.LuziaVolta();
        }

        if (Terminocadeado && Input.GetKeyDown(KeyCode.E) || Terminocadeado && Input.GetKeyDown(KeyCode.Return))
        {
            Molduraoff();
            podever = false;
            ScriptLuzia.LuziaVolta();
        }
    }

    void VerificarSenha()
    {
        if (inputSenha.text == senhaCorreta)
        {
            mensagem.text = "Aberto!";
            AbrirCadeado();
        }
        else
        {
            mensagem.text = "Senha incorreta!";
            inputSenha.text = "";
        }
    }

    void AbrirCadeado()
    {
        ScriptLuzia.Luziapara();
        painelUI.SetActive(false);
        Cadeadoiniciado = false;
        Terminocadeado = true;

        Molduraon();
    }

    public void Comecarcadeado()
    {
        Cadeadoiniciado = true;
    }

    private void Molduraon()
    {
        Color cor = MolduraSR.color;
        cor.a = 1f;
        MolduraSR.color = cor;
    }

    private void Molduraoff()
    {
        Color cor = MolduraSR.color;
        cor.a = 0f;
        MolduraSR.color = cor;
        podever = true;
    }
}
