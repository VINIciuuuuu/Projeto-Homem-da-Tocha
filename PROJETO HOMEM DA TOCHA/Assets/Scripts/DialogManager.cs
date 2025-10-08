using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Referências de UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject dialoguePanel;

    [Header("Falas")]
    [TextArea(2, 5)]
    [SerializeField] private string[] lines;

    [Header("Comportamento")]
    [SerializeField] private bool autoStart = false;
    [SerializeField] private bool useTypewriter = false;
    [SerializeField, Range(0.01f, 0.1f)] private float charDelay = 0.02f;

    [Header("Eventos")]
    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    private int index = -1;
    private bool isRunning = false;
    private Coroutine typingRoutine;
    public Interagir scriptNPCDialog;
    public Luzia scriptLuzia;

    private void Awake()
    {
        if (nextButton != null) { }
        nextButton.onClick.AddListener(Next);
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Start()
    {
        if (autoStart) StartDialogue();
        scriptNPCDialog = FindAnyObjectByType<Interagir>();
        scriptLuzia = FindAnyObjectByType<Luzia>();
        //Referencia os dois Scripts sem a necessidade de ser colocado no inspetor, mesmo q apareça lá
    }

    private void Update()
    {
        if (isRunning)
        {
            scriptLuzia.MoveSpeed = 0f;
        }
        else
        {
            scriptLuzia.MoveSpeed = 3f;
        }
    }
    public void StartDialogue()
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("[DialogueManager] Nenhuma fala configurada.");
            return;
        }

        index = -1;
        isRunning = true;
        dialoguePanel?.SetActive(true);
        onDialogueStart?.Invoke();
        Next();
    }

    public void Next()
    {
        if (!isRunning) return;

        index++;
        if (index >= lines.Length)
        {
            EndDialogue();
            return;
        }

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        if (useTypewriter)
            typingRoutine = StartCoroutine(TypeText(lines[index]));
        else
            dialogueText.text = lines[index];
    }

    private IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(charDelay);
        }
        typingRoutine = null;
    }

    private void EndDialogue()
    {
        isRunning = false;
        dialoguePanel?.SetActive(false);
        onDialogueEnd?.Invoke();
        scriptNPCDialog.DialogoFechado();
        //Puxa a função do Dialogo com NPC
    }
}