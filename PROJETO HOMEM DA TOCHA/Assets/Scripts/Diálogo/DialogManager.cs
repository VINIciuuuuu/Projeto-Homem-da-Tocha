using System.Collections;
using System.Collections.Generic; // Necess�rio para List
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [Header("Refer�ncias de UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameTextB;
    [SerializeField] private TextMeshProUGUI nameTextA;
    [SerializeField] private Image portraitImageA;
    [SerializeField] private Image portraitImageB;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject optionsPanel; // NOVO: Painel que cont�m os bot�es de op��o
    [SerializeField] private List<Button> optionButtons; // NOVO: Lista dos seus bot�es de op��o

    [Header("Comportamento")]
    [SerializeField] private bool useTypewriter = false;
    [SerializeField, Range(0.01f, 0.1f)] private float charDelay = 0.02f;

    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    private string[] currentLines;
    private UnityEvent currentOnDialogueStartEventFromData;
    private UnityEvent currentOnDialogueEndEventFromData;
    private List<DialogueOption> currentOptions; // NOVO: As op��es do DialogueData atual

    private int index = -1;
    private Coroutine typingRoutine;
    public Luzia ScriptLuzia;

    public bool IsRunning { get; private set; } = false;

    private static DialogManager instance;

    public static DialogManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DialogManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("DialogueManager");
                    instance = obj.AddComponent<DialogManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (nextButton != null)
            nextButton.onClick.AddListener(Next);

        // Adiciona listeners para os bot�es de op��o
        for (int i = 0; i < optionButtons.Count; i++)
        {
            int optionIndex = i; // Captura o �ndice para o closure
            optionButtons[i].onClick.AddListener(() => SelectOption(optionIndex));
        }

        dialoguePanel?.SetActive(false);
        optionsPanel?.SetActive(false); // NOVO: Esconde o painel de op��es no in�cio
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
        {
            nextButton.onClick.Invoke();
        }
    }

    private void Start()
    {
        ScriptLuzia = FindAnyObjectByType<Luzia>();
    }

    public void StartDialogue(DialogueData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[DialogManager] Nenhum DialogueData fornecido para iniciar.");
            return;
        }
        if (IsRunning)
        {
            Debug.LogWarning("[DialogManager] Tentando iniciar um di�logo enquanto outro j� est� rodando.");
            return;
        }

        currentLines = data.lines;
        currentOnDialogueStartEventFromData = data.onDialogueStartEvent;
        currentOnDialogueEndEventFromData = data.onDialogueEndEvent;
        currentOptions = data.options; // NOVO: Carrega as op��es
        ScriptLuzia.Luziapara();

        if (currentLines == null || currentLines.Length == 0)
        {
            Debug.LogWarning($"[DialogManager] DialogueData '{data.name}' n�o possui falas configuradas.");
            EndDialogue(); // Se n�o tem falas, termina o di�logo imediatamente (ou pode ir direto para op��es se `hasOptions` for true)
            return;
        }

        index = -1;
        IsRunning = true;
        dialoguePanel?.SetActive(true);
        optionsPanel?.SetActive(false); // Garante que op��es estejam escondidas ao iniciar um novo di�logo

        if (nameTextA != null) nameTextA.text = data.characterName1;
        if (nameTextB != null) nameTextB.text = data.characterName2;
        if (portraitImageA != null) portraitImageA.sprite = data.characterPortrait1;
        if (portraitImageB != null) portraitImageB.sprite = data.characterPortrait2;

        onDialogueStart?.Invoke();
        currentOnDialogueStartEventFromData?.Invoke();

        Next();
    }

    public void Next()
    {
        if (!IsRunning) return;

        // Se est� digitando, pula para o final da frase atual
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            dialogueText.text = currentLines[index];
            typingRoutine = null;
            return;
        }

        index++;
        if (index >= currentLines.Length)
        {
            // Fim das falas sequenciais. Verificar se h� op��es.
            if (currentOptions != null && currentOptions.Count > 0)
            {
                DisplayOptions(); // NOVO: Mostra as op��es
            }
            else
            {
                EndDialogue(); // Se n�o h� op��es, encerra o di�logo
            }
            return;
        }

        if (useTypewriter)
            typingRoutine = StartCoroutine(TypeText(currentLines[index]));
        else
            dialogueText.text = currentLines[index];
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

    private void DisplayOptions() // NOVO: M�todo para exibir as op��es
    {
        nextButton.gameObject.SetActive(false); // Esconde o bot�o "Pr�ximo"
        optionsPanel?.SetActive(true); // Mostra o painel de op��es

        // Desativa todos os bot�es primeiro
        foreach (Button btn in optionButtons)
        {
            btn.gameObject.SetActive(false);
        }

        // Ativa e configura apenas as op��es dispon�veis
        for (int i = 0; i < currentOptions.Count && i < optionButtons.Count; i++)
        {
            DialogueOption option = currentOptions[i];
            Button btn = optionButtons[i];

            // Opcional: Adicione l�gica para verificar se a op��o est� dispon�vel
            // if (!option.IsAvailable()) continue; 

            btn.gameObject.SetActive(true);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = option.optionText;
            // O listener j� est� adicionado no Awake, ele chamar� SelectOption(i)
        }
    }

    private void SelectOption(int optionIndex) // NOVO: M�todo para lidar com a sele��o de uma op��o
    {
        if (optionIndex < 0 || optionIndex >= currentOptions.Count) return;

        DialogueOption selectedOption = currentOptions[optionIndex];

        // Chame os eventos espec�ficos da op��o selecionada
        selectedOption.onOptionSelected?.Invoke();

        optionsPanel?.SetActive(false); // Esconde o painel de op��es
        nextButton.gameObject.SetActive(true); // Reativa o bot�o "Pr�ximo" para o pr�ximo di�logo

        // Se a op��o leva a um novo di�logo, inicie-o
        if (selectedOption.nextDialogue != null)
        {
            EndDialogue(false); // Encerra o di�logo atual sem invocar onDialogueEnd global ainda
            StartDialogue(selectedOption.nextDialogue); // Inicia o pr�ximo di�logo na �rvore
        }
        else
        {
            // Se a op��o n�o leva a um novo di�logo, o fluxo termina aqui
            EndDialogue();
        }
    }

    private void EndDialogue(bool invokeGlobalEndEvent = true) // Modificado para controlar o onDialogueEnd global
    {
        IsRunning = false;
        dialoguePanel?.SetActive(false);
        optionsPanel?.SetActive(false); // Garante que o painel de op��es esteja escondido
        nextButton.gameObject.SetActive(true); // Garante que o bot�o 'Next' esteja vis�vel para o pr�ximo di�logo (se um for iniciado em seguida)
        ScriptLuzia.LuziaVolta();

        currentOnDialogueEndEventFromData?.Invoke(); // Evento do DialogueData atual

        if (invokeGlobalEndEvent)
        {
            onDialogueEnd?.Invoke(); // Evento global do DialogManager (somente se o fluxo de di�logo realmente terminou)
        }

        // Limpa as refer�ncias para evitar refer�ncias cruzadas indesejadas
        currentLines = null;
        currentOnDialogueStartEventFromData = null;
        currentOnDialogueEndEventFromData = null;
        currentOptions = null; // Limpa as op��es
    }
}