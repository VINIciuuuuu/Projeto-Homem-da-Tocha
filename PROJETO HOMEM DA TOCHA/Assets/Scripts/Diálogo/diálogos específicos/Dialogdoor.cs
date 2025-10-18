using UnityEngine;
using UnityEngine.Events;

/*
Este código DEVE ser anexado a cada GameObject de NPC ou Objeto Interativo.
Este código que irá dizer quando o jogador pode interagir ou não com um NPC.
*/

public class Dialogdoor : MonoBehaviour
{
    [Header("Configuração de Diálogo")]
    [SerializeField] private DialogueData npcDialogueData; //Asset do dialogo do NPC

    public UnityEvent NPCDialogueStartEvents;
    public UnityEvent NPCDialogueEndEvents;
    public GameObject BotaoE;
    private SpriteRenderer botaoSR;
    private bool varapega = false;

    [Header("Configuração de Interação")]
    [SerializeField] private string playerTag = "Player"; // Tag do seu jogador
    [SerializeField] private KeyCode interactionKey1 = KeyCode.E; // tecla para iniciar dialogo. Pode ser alterada no inspetor
    [SerializeField] private KeyCode interactionKey2 = KeyCode.Return; // tecla para inciar dialog.

    private bool PlayerInRange = false; //define quando o jogador pode iniciar um dialog
    private Luzia scriptLuzia; //cria uma variavel para o codigo do jogador (Luzia) para fazer com que não se mova durante um dialog

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            PlayerInRange = true;
            Color cor = botaoSR.color;
            cor.a = 1f;
            botaoSR.color = cor;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            PlayerInRange = false;
            Color cor = botaoSR.color;
            cor.a = 0f;
            botaoSR.color = cor;
        }
    }
    private void Start()
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

    private void Awake()
    {
        scriptLuzia = FindAnyObjectByType<Luzia>();
        if (scriptLuzia == null)
        {
            Debug.Log("Script da Luzia não encontrado");
        }
        //avisa quando codigo de luzia não for encontrado

        if (npcDialogueData == null)
        {
            Debug.Log("NPC sem dialogo");
        }
        //avisa quando Asset d dialogo do npc não for encontrado
    }

    void Update()
    {
        if (!varapega && PlayerInRange && !DialogManager.Instance.IsRunning && (Input.GetKeyDown(interactionKey1) || Input.GetKeyDown(interactionKey2)))
        {
            IniciarDialogo();
        }
        //caso o jogador esteja no alcance, nenhum outro dialogo esteja rodando e ele aperte X tecla o dialog inicia
    }

    private void IniciarDialogo()
    {
        NPCDialogueStartEvents?.Invoke();
        //Puxa eventos antes do dialogo iniciar

        DialogManager.Instance.StartDialogue(npcDialogueData);
        //Fala com o DialogManager para ele começar o dialogo e dando o Asset do dialogo do NPC

        DialogManager.Instance.onDialogueEndGlobal.AddListener(GlobalDialogueEnded);
        //Puxa evento quando termina o dialogo

        if (scriptLuzia != null)
        {
            scriptLuzia.MoveSpeed = 0f;
            Debug.Log($"Velocidade de Luzia definido para {scriptLuzia.MoveSpeed}");
        }
        //Caso tenha o codigo Luzia, ele se tornará 0
    }

    // Este método é chamado quando QUALQUER diálogo no sistema é encerrado pelo DialogManager
    private void GlobalDialogueEnded()
    {
        // Remove a assinatura para evitar que este NPC responda a outros diálogos ou a eventos múltiplos
        DialogManager.Instance.onDialogueEndGlobal.RemoveListener(GlobalDialogueEnded);

        // Restaura o movimento do jogador
        if (scriptLuzia != null)
        {
            scriptLuzia.MoveSpeed = 3f; // Ou o valor padrão de movimento da Luzia
            Debug.Log($"Velocidade da Luzia restaurada para 3f pelo NPC '{gameObject.name}'.");
        }

        // Chame os eventos específicos DESTE NPC que devem acontecer DEPOIS do diálogo terminar
        NPCDialogueEndEvents?.Invoke();
        Debug.Log($"Diálogo do NPC '{gameObject.name}' fechado.");
    }

    public void Pegouvara()
    {
        varapega = true;
    }
}