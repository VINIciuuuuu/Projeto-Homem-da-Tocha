using System.Collections;
using System.Collections.Generic; // Necessário para List
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [Header("Referências de UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameTextB;
    [SerializeField] private TextMeshProUGUI nameTextA;
    [SerializeField] private Image portraitImageA;
    [SerializeField] private Image portraitImageB;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject optionsPanel; // NOVO: Painel que contém os botões de opção
    [SerializeField] private List<Button> optionButtons; // NOVO: Lista dos seus botões de opção

    [Header("Comportamento")]
    [SerializeField] private bool useTypewriter = false;
    [SerializeField, Range(0.01f, 0.1f)] private float charDelay = 0.02f;

    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    private string[] currentLines;
    private UnityEvent currentOnDialogueStartEventFromData;
    private UnityEvent currentOnDialogueEndEventFromData;
    private List<DialogueOption> currentOptions; // NOVO: As opções do DialogueData atual

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

        // Adiciona listeners para os botões de opção
        for (int i = 0; i < optionButtons.Count; i++)
        {
            int optionIndex = i; // Captura o índice para o closure
            optionButtons[i].onClick.AddListener(() => SelectOption(optionIndex));
        }

        dialoguePanel?.SetActive(false);
        optionsPanel?.SetActive(false); // NOVO: Esconde o painel de opções no início
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
            Debug.LogWarning("[DialogManager] Tentando iniciar um diálogo enquanto outro já está rodando.");
            return;
        }

        currentLines = data.lines;
        currentOnDialogueStartEventFromData = data.onDialogueStartEvent;
        currentOnDialogueEndEventFromData = data.onDialogueEndEvent;
        currentOptions = data.options; // NOVO: Carrega as opções
        ScriptLuzia.Luziapara();

        if (currentLines == null || currentLines.Length == 0)
        {
            Debug.LogWarning($"[DialogManager] DialogueData '{data.name}' não possui falas configuradas.");
            EndDialogue(); // Se não tem falas, termina o diálogo imediatamente (ou pode ir direto para opções se `hasOptions` for true)
            return;
        }

        index = -1;
        IsRunning = true;
        dialoguePanel?.SetActive(true);
        optionsPanel?.SetActive(false); // Garante que opções estejam escondidas ao iniciar um novo diálogo

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

        // Se está digitando, pula para o final da frase atual
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
            // Fim das falas sequenciais. Verificar se há opções.
            if (currentOptions != null && currentOptions.Count > 0)
            {
                DisplayOptions(); // NOVO: Mostra as opções
            }
            else
            {
                EndDialogue(); // Se não há opções, encerra o diálogo
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

    private void DisplayOptions() // NOVO: Método para exibir as opções
    {
        nextButton.gameObject.SetActive(false); // Esconde o botão "Próximo"
        optionsPanel?.SetActive(true); // Mostra o painel de opções

        // Desativa todos os botões primeiro
        foreach (Button btn in optionButtons)
        {
            btn.gameObject.SetActive(false);
        }

        // Ativa e configura apenas as opções disponíveis
        for (int i = 0; i < currentOptions.Count && i < optionButtons.Count; i++)
        {
            DialogueOption option = currentOptions[i];
            Button btn = optionButtons[i];

            // Opcional: Adicione lógica para verificar se a opção está disponível
            // if (!option.IsAvailable()) continue; 

            btn.gameObject.SetActive(true);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = option.optionText;
            // O listener já está adicionado no Awake, ele chamará SelectOption(i)
        }
    }

    private void SelectOption(int optionIndex) // NOVO: Método para lidar com a seleção de uma opção
    {
        if (optionIndex < 0 || optionIndex >= currentOptions.Count) return;

        DialogueOption selectedOption = currentOptions[optionIndex];

        // Chame os eventos específicos da opção selecionada
        selectedOption.onOptionSelected?.Invoke();

        optionsPanel?.SetActive(false); // Esconde o painel de opções
        nextButton.gameObject.SetActive(true); // Reativa o botão "Próximo" para o próximo diálogo

        // Se a opção leva a um novo diálogo, inicie-o
        if (selectedOption.nextDialogue != null)
        {
            EndDialogue(false); // Encerra o diálogo atual sem invocar onDialogueEnd global ainda
            StartDialogue(selectedOption.nextDialogue); // Inicia o próximo diálogo na árvore
        }
        else
        {
            // Se a opção não leva a um novo diálogo, o fluxo termina aqui
            EndDialogue();
        }
    }

    private void EndDialogue(bool invokeGlobalEndEvent = true) // Modificado para controlar o onDialogueEnd global
    {
        IsRunning = false;
        dialoguePanel?.SetActive(false);
        optionsPanel?.SetActive(false); // Garante que o painel de opções esteja escondido
        nextButton.gameObject.SetActive(true); // Garante que o botão 'Next' esteja visível para o próximo diálogo (se um for iniciado em seguida)
        ScriptLuzia.LuziaVolta();

        currentOnDialogueEndEventFromData?.Invoke(); // Evento do DialogueData atual

        if (invokeGlobalEndEvent)
        {
            onDialogueEnd?.Invoke(); // Evento global do DialogManager (somente se o fluxo de diálogo realmente terminou)
        }

        // Limpa as referências para evitar referências cruzadas indesejadas
        currentLines = null;
        currentOnDialogueStartEventFromData = null;
        currentOnDialogueEndEventFromData = null;
        currentOptions = null; // Limpa as opções
    }
}