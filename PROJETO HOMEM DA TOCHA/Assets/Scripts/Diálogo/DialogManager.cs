using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro; // Certifique-se de que TMPro esteja configurado no seu projeto

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
    [SerializeField] private GameObject optionsPanel; // Painel que contém os botões de opção
    [SerializeField] private List<Button> optionButtons; // Lista dos seus botões de opção

    [Header("Comportamento")]
    [SerializeField] private bool useTypewriter = false;
    [SerializeField, Range(0.01f, 0.1f)] private float charDelay = 0.02f;

    // Eventos GLOBAIS do DialogManager (para quando QUALQUER diálogo começa/termina)
    public UnityEvent onDialogueStartGlobal;
    public UnityEvent onDialogueEndGlobal;

    private string[] currentLines;
    // Referências aos eventos do DialogueData ATUAL
    private UnityEvent currentDialogueDataOnStartEvent;
    private UnityEvent currentDialogueDataOnEndEvent;
    private List<DialogueOption> currentOptions;

    private int index = -1;
    private Coroutine typingRoutine;
    public Luzia ScriptLuzia; // Certifique-se de que o script 'Luzia' existe e tem os métodos Luziapara()/LuziaVolta()

    public bool IsRunning { get; private set; } = false;

    private static DialogManager instance;

    // Garante que haja apenas uma instância do DialogManager
    public static DialogManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DialogManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("DialogManager");
                    instance = obj.AddComponent<DialogManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        // Implementação de Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // Mantém o manager entre cenas

        // Adiciona listeners para os botões "Próximo" e de Opção
        if (nextButton != null)
            nextButton.onClick.AddListener(Next);

        for (int i = 0; i < optionButtons.Count; i++)
        {
            int optionIndex = i; // Captura o índice para o closure
            optionButtons[i].onClick.AddListener(() => SelectOption(optionIndex));
        }

        // Garante que a UI de diálogo esteja escondida no início
        dialoguePanel?.SetActive(false);
        optionsPanel?.SetActive(false);
    }

    private void Start()
    {
        // Tenta encontrar o script Luzia no Awake ou Start
        if (ScriptLuzia == null)
        {
            ScriptLuzia = FindAnyObjectByType<Luzia>();
            if (ScriptLuzia == null)
            {
                Debug.LogWarning("[DialogManager] Script 'Luzia' não encontrado na cena. Os métodos Luziapara/LuziaVolta não serão chamados.");
            }
        }
    }

    // Input do jogador para avançar o diálogo
    private void Update()
    {
        if (!IsRunning) return; // Só processa input se houver um diálogo rodando

        // Se o painel de opções estiver ativo, não avança com Enter/E
        if (optionsPanel != null && optionsPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Return))
        {
            // Oculta o painel de opções se estiver visível e as linhas acabaram
            if (index >= currentLines.Length && currentOptions != null && currentOptions.Count > 0 && optionsPanel != null && optionsPanel.activeSelf)
            {
                // Não faz nada, o jogador deve escolher uma opção
                return;
            }
            Next();
        }
    }

    // Inicia um novo diálogo
    public void StartDialogue(DialogueData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[DialogManager] Nenhum DialogueData fornecido para iniciar.");
            return;
        }
        if (IsRunning)
        {
            Debug.LogWarning("[DialogManager] Tentando iniciar um diálogo enquanto outro já está rodando. O diálogo atual será encerrado.");
            EndDialogue(false); // Força o término do diálogo anterior sem invocar onDialogueEndGlobal
        }

        currentLines = data.lines;
        currentDialogueDataOnStartEvent = data.onDialogueStartEvent;
        currentDialogueDataOnEndEvent = data.onDialogueEndEvent;
        currentOptions = data.options;

        // Chama o método "parar" da Luzia se o script for encontrado
        ScriptLuzia?.Luziapara();

        if (currentLines == null || currentLines.Length == 0)
        {
            // Se não há falas, mas há opções, tenta exibir opções diretamente
            if (data.hasOptions && currentOptions != null && currentOptions.Count > 0)
            {
                IsRunning = true; // Precisa estar rodando para exibir opções
                dialoguePanel?.SetActive(true); // Ativa o painel para exibir opções, mesmo sem diálogo
                SetCharacterInfo(data); // Define as informações dos personagens
                DisplayOptions();
            }
            else
            {
                Debug.LogWarning($"[DialogManager] DialogueData '{data.name}' não possui falas configuradas e nem opções. Encerrando.");
                EndDialogue();
            }
            return;
        }

        index = -1;
        IsRunning = true;
        dialoguePanel?.SetActive(true);
        optionsPanel?.SetActive(false); // Garante que opções estejam escondidas ao iniciar um novo diálogo
        nextButton.gameObject.SetActive(true); // Garante que o botão 'Próximo' esteja ativo

        SetCharacterInfo(data); // Define as informações dos personagens

        // Invoca os eventos de início
        onDialogueStartGlobal?.Invoke();            // Evento global do Manager
        currentDialogueDataOnStartEvent?.Invoke();  // Evento específico do DialogueData

        Next(); // Exibe a primeira linha
    }

    // Avança para a próxima linha ou exibe opções
    public void Next()
    {
        if (!IsRunning) return;

        // Se o painel de opções está ativo, significa que o jogador deve escolher uma opção, não avançar
        if (optionsPanel != null && optionsPanel.activeSelf) return;

        // Se está digitando, pula para o final da frase atual
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            dialogueText.text = currentLines[index];
            typingRoutine = null;
            return;
        }

        index++;
        if (index < currentLines.Length)
        {
            // Exibe a próxima linha
            if (useTypewriter)
                typingRoutine = StartCoroutine(TypeText(currentLines[index]));
            else
                dialogueText.text = currentLines[index];
        }
        else
        {
            // Fim das falas sequenciais. Verificar se há opções.
            if (currentOptions != null && currentOptions.Count > 0)
            {
                DisplayOptions();
            }
            else
            {
                EndDialogue(); // Se não há opções, encerra o diálogo
            }
        }
    }

    // Efeito de máquina de escrever
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

    // Exibe as opções de diálogo
    private void DisplayOptions()
    {
        nextButton.gameObject.SetActive(false); // Esconde o botão "Próximo"
        optionsPanel?.SetActive(true); // Mostra o painel de opções

        // Desativa todos os botões primeiro
        foreach (Button btn in optionButtons)
        {
            btn.gameObject.SetActive(false);
            // Limpa listeners antigos para evitar chamadas duplas se o manager não foi destruído
            btn.onClick.RemoveAllListeners();
        }

        // Ativa e configura apenas as opções disponíveis
        for (int i = 0; i < currentOptions.Count && i < optionButtons.Count; i++)
        {
            DialogueOption option = currentOptions[i];
            Button btn = optionButtons[i];

            btn.gameObject.SetActive(true);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = option.optionText;

            // Adiciona o listener para esta opção específica.
            // É importante adicionar o listener aqui para que ele capture a opção correta.
            int optionIndex = i; // Captura o índice para o closure
            btn.onClick.AddListener(() => SelectOption(optionIndex));
        }
    }

    // Lida com a seleção de uma opção
    private void SelectOption(int optionIndex)
    {
        if (optionIndex < 0 || optionIndex >= currentOptions.Count) return;

        DialogueOption selectedOption = currentOptions[optionIndex];

        // Chama os eventos específicos da opção selecionada
        selectedOption.onOptionSelected?.Invoke();

        optionsPanel?.SetActive(false); // Esconde o painel de opções
        nextButton.gameObject.SetActive(true); // Reativa o botão "Próximo" (se for continuar com outro diálogo)

        // Se a opção leva a um novo diálogo
        if (selectedOption.nextDialogue != null)
        {
            EndDialogue(false); // Encerra o diálogo atual sem invocar onDialogueEndGlobal
            StartDialogue(selectedOption.nextDialogue); // Inicia o próximo diálogo
        }
        else
        {
            // Se a opção não leva a um novo diálogo, o fluxo termina aqui
            EndDialogue();
        }
    }

    // Encerra o diálogo
    private void EndDialogue(bool invokeGlobalEndEvent = true)
    {
        IsRunning = false;
        dialoguePanel?.SetActive(false);
        optionsPanel?.SetActive(false); // Garante que o painel de opções esteja escondido
        nextButton.gameObject.SetActive(true); // Garante que o botão 'Next' esteja visível (para um eventual próximo diálogo)

        ScriptLuzia?.LuziaVolta(); // Chama o método "voltar" da Luzia

        // Invoca os eventos de fim
        currentDialogueDataOnEndEvent?.Invoke(); // Evento específico do DialogueData atual

        if (invokeGlobalEndEvent)
        {
            onDialogueEndGlobal?.Invoke(); // Evento global do Manager (somente se o fluxo de diálogo realmente terminou)
        }

        // Limpa as referências para evitar referências cruzadas indesejadas
        currentLines = null;
        currentDialogueDataOnStartEvent = null;
        currentDialogueDataOnEndEvent = null;
        currentOptions = null;
        index = -1; // Reseta o índice
    }

    // Define as informações do personagem na UI
    private void SetCharacterInfo(DialogueData data)
    {
        if (nameTextA != null) nameTextA.text = data.characterName1;
        if (nameTextB != null) nameTextB.text = data.characterName2;
        if (portraitImageA != null) portraitImageA.sprite = data.characterPortrait1;
        if (portraitImageB != null) portraitImageB.sprite = data.characterPortrait2;
    }

    // Método de exemplo para ser chamado pelos eventos do DialogueData (para fins de teste/demonstração)
    public void ExampleManagerMethod()
    {
        Debug.Log("Método 'ExampleManagerMethod' no DialogManager foi invocado por um evento do DialogueData!");
    }

    public void LoadNewScene(string sceneName)
    {
        Debug.Log($"[DialogManager] Solicitado carregar cena: {sceneName}");
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    // Mais métodos públicos que seus DialogueEvents podem chamar (Ex: dar item, tocar som, etc.)
    public void GivePlayerItem(string itemName)
    {
        Debug.Log($"[DialogManager] Jogador recebeu item: {itemName}");
        // Implemente a lógica de inventário aqui
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null)
        {
            // Ex: AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
            Debug.Log($"[DialogManager] Tocando som: {clip.name}");
        }
    }
}