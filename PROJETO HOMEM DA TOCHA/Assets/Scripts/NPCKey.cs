using UnityEngine;
using DialogueEditor;

public class NPCKey : MonoBehaviour
{
    public static NPCKey Instance;

    public GameObject BotaoE;
    private SpriteRenderer botaoSR;
    public NPCConversation myConversation;
    public KeyCode InteractionKey = KeyCode.E;
    private bool Podeinteragir;
    private bool Emdialogo;

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
            Podeinteragir = true;
            Color cor = botaoSR.color;
            cor.a = 1f;
            botaoSR.color = cor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Podeinteragir = false;
            Color cor = botaoSR.color;
            cor.a = 0f;
            botaoSR.color = cor;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(InteractionKey) && Podeinteragir && !Emdialogo)
        {
            ConversationManager.Instance.StartConversation(myConversation);
        }

        if (Emdialogo)
        {
            Luzia.Instance.Luziapara();
        }
        else
        {
            Luzia.Instance.Luziavolta();
        }

        if (ConversationManager.Instance != null)
        {
            if (ConversationManager.Instance.IsConversationActive)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                    ConversationManager.Instance.SelectPreviousOption();

                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    ConversationManager.Instance.SelectNextOption();

                else if (Input.GetKeyDown(KeyCode.E))
                    ConversationManager.Instance.PressSelectedOption();
            }
        }
        Emdialogo = ConversationManager.Instance.IsConversationActive;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ConversationManager.Instance.EndConversation();
        }
        Debug.Log(Emdialogo);
    }

    public void Emdialogoff()
    {
        Emdialogo = false;
    }
}