using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    void Start()
    {
        Moldura = GameObject.FindGameObjectWithTag("MfamTuri");
        ScriptLuzia = FindAnyObjectByType<Luzia>();
        painelUI.SetActive(false);
        botaoConfirmar.onClick.AddListener(VerificarSenha);

        if (Moldura != null)
        {
            MolduraSR = Moldura.GetComponent<SpriteRenderer>();
            
            if (MolduraSR != null)
            {
                Color cor = MolduraSR.color;
                cor.a = 0f;
                MolduraSR.color = cor;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Color cor = MolduraSR.color;
            cor.a = 0f;
            MolduraSR.color = cor;

            ScriptLuzia.LuziaVolta();
        }

        if (Cadeadoiniciado)
        {
            painelUI.SetActive(true);
            ScriptLuzia.MoveSpeed = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            painelUI.SetActive(false);
            Cadeadoiniciado = false;
            ScriptLuzia.MoveSpeed = 3f;
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
        gameObject.SetActive(false);
        Cadeadoiniciado = false;
        
        Color cor = MolduraSR.color;
        cor.a = 1f;
        MolduraSR.color = cor;
    }

    public void Comecarcadeado()
    {
        Cadeadoiniciado = true;
    }
}
