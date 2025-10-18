using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro; // Certifique-se de que TMPro esteja configurado no seu projeto

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
    [SerializeField] private GameObject optionsPanel; // Painel que cont�m os bot�es de op��o
    [SerializeField] private List<Button> optionButtons; // Lista dos seus bot�es de op��o

    [Header("Comportamento")]
    [SerializeField] private bool useTypewriter = false;
    [SerializeField, Range(0.01f, 0.1f)] private float charDelay = 0.02f;

    // Eventos GLOBAIS do DialogManager (para quando QUALQUER di�logo come�a/termina)
    public UnityEvent onDialogueStartGlobal;
    public UnityEvent onDialogueEndGlobal;

    private string[] currentLines;
    // Refer�ncias aos eventos do DialogueData ATUAL
    private UnityEvent currentDialogueDataOnStartEvent;
    private UnityEvent currentDialogueDataOnEndEvent;
    private List<DialogueOption> currentOptions;

    private int index = -1;
    private Coroutine typingRoutine;
    public Luzia ScriptLuzia; // Certifique-se de que o script 'Luzia' existe e tem os m�todos Luziapara()/LuziaVolta()

    public bool IsRunning { get; private set; } = false;

    private static DialogManager instance;

    // Garante que haja apenas uma inst�ncia do DialogManager
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
        // Implementa��o de Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // Mant�m o manager entre cenas

        // Adiciona listeners para os bot�es "Pr�ximo" e de Op��o
        if (nextButton != null)
            nextButton.onClick.AddListener(Next);

        for (int i = 0; i < optionButtons.Count; i++)
        {
            int optionIndex = i; // Captura o �ndice para o closure
            optionButtons[i].onClick.AddListener(() => SelectOption(optionIndex));
        }

        // Garante que a UI de di�logo esteja escondida no in�cio
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
                Debug.LogWarning("[DialogManager] Script 'Luzia' n�o encontrado na cena. Os m�todos Luziapara/LuziaVolta n�o ser�o chamados.");
            }
        }
    }

    // Input do jogador para avan�ar o di�logo
    private void Update()
    {
        if (!IsRunning) return; // S� processa input se houver um di�logo rodando

        // Se o painel de op��es estiver ativo, n�o avan�a com Enter/E
        if (optionsPanel != null && optionsPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Return))
        {
            // Oculta o painel de op��es se estiver vis�vel e as linhas acabaram
            if (index >= currentLines.Length && currentOptions != null && currentOptions.Count > 0 && optionsPanel != null && optionsPanel.activeSelf)
            {
                // N�o faz nada, o jogador deve escolher uma op��o
                return;
            }
            Next();
        }
    }

    // Inicia um novo di�logo
    public void StartDialogue(DialogueData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[DialogManager] Nenhum DialogueData fornecido para iniciar.");
            return;
        }
        if (IsRunning)
        {
            Debug.LogWarning("[DialogManager] Tentando iniciar um di�logo enquanto outro j� est� rodando. O di�logo atual ser� encerrado.");
            EndDialogue(false); // For�a o t�rmino do di�logo anterior sem invocar onDialogueEndGlobal
        }

        currentLines = data.lines;
        currentDialogueDataOnStartEvent = data.onDialogueStartEvent;
        currentDialogueDataOnEndEvent = data.onDialogueEndEvent;
        currentOptions = data.options;

        // Chama o m�todo "parar" da Luzia se o script for encontrado
        ScriptLuzia?.Luziapara();

        if (currentLines == null || currentLines.Length == 0)
        {
            // Se n�o h� falas, mas h� op��es, tenta exibir op��es diretamente
            if (data.hasOptions && currentOptions != null && currentOptions.Count > 0)
            {
                IsRunning = true; // Precisa estar rodando para exibir op��es
                dialoguePanel?.SetActive(true); // Ativa o painel para exibir op��es, mesmo sem di�logo
                SetCharacterInfo(data); // Define as informa��es dos personagens
                DisplayOptions();
            }
            else
            {
                Debug.LogWarning($"[DialogManager] DialogueData '{data.name}' n�o possui falas configuradas e nem op��es. Encerrando.");
                EndDialogue();
            }
            return;
        }

        index = -1;
        IsRunning = true;
        dialoguePanel?.SetActive(true);
        optionsPanel?.SetActive(false); // Garante que op��es estejam escondidas ao iniciar um novo di�logo
        nextButton.gameObject.SetActive(true); // Garante que o bot�o 'Pr�ximo' esteja ativo

        SetCharacterInfo(data); // Define as informa��es dos personagens

        // Invoca os eventos de in�cio
        onDialogueStartGlobal?.Invoke();            // Evento global do Manager
        currentDialogueDataOnStartEvent?.Invoke();  // Evento espec�fico do DialogueData

        Next(); // Exibe a primeira linha
    }

    // Avan�a para a pr�xima linha ou exibe op��es
    public void Next()
    {
        if (!IsRunning) return;

        // Se o painel de op��es est� ativo, significa que o jogador deve escolher uma op��o, n�o avan�ar
        if (optionsPanel != null && optionsPanel.activeSelf) return;

        // Se est� digitando, pula para o final da frase atual
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
            // Exibe a pr�xima linha
            if (useTypewriter)
                typingRoutine = StartCoroutine(TypeText(currentLines[index]));
            else
                dialogueText.text = currentLines[index];
        }
        else
        {
            // Fim das falas sequenciais. Verificar se h� op��es.
            if (currentOptions != null && currentOptions.Count > 0)
            {
                DisplayOptions();
            }
            else
            {
                EndDialogue(); // Se n�o h� op��es, encerra o di�logo
            }
        }
    }

    // Efeito de m�quina de escrever
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

    // Exibe as op��es de di�logo
    private void DisplayOptions()
    {
        nextButton.gameObject.SetActive(false); // Esconde o bot�o "Pr�ximo"
        optionsPanel?.SetActive(true); // Mostra o painel de op��es

        // Desativa todos os bot�es primeiro
        foreach (Button btn in optionButtons)
        {
            btn.gameObject.SetActive(false);
            // Limpa listeners antigos para evitar chamadas duplas se o manager n�o foi destru�do
            btn.onClick.RemoveAllListeners();
        }

        // Ativa e configura apenas as op��es dispon�veis
        for (int i = 0; i < currentOptions.Count && i < optionButtons.Count; i++)
        {
            DialogueOption option = currentOptions[i];
            Button btn = optionButtons[i];

            btn.gameObject.SetActive(true);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = option.optionText;

            // Adiciona o listener para esta op��o espec�fica.
            // � importante adicionar o listener aqui para que ele capture a op��o correta.
            int optionIndex = i; // Captura o �ndice para o closure
            btn.onClick.AddListener(() => SelectOption(optionIndex));
        }
    }

    // Lida com a sele��o de uma op��o
    private void SelectOption(int optionIndex)
    {
        if (optionIndex < 0 || optionIndex >= currentOptions.Count) return;

        DialogueOption selectedOption = currentOptions[optionIndex];

        // Chama os eventos espec�ficos da op��o selecionada
        selectedOption.onOptionSelected?.Invoke();

        optionsPanel?.SetActive(false); // Esconde o painel de op��es
        nextButton.gameObject.SetActive(true); // Reativa o bot�o "Pr�ximo" (se for continuar com outro di�logo)

        // Se a op��o leva a um novo di�logo
        if (selectedOption.nextDialogue != null)
        {
            EndDialogue(false); // Encerra o di�logo atual sem invocar onDialogueEndGlobal
            StartDialogue(selectedOption.nextDialogue); // Inicia o pr�ximo di�logo
        }
        else
        {
            // Se a op��o n�o leva a um novo di�logo, o fluxo termina aqui
            EndDialogue();
        }
    }

    // Encerra o di�logo
    private void EndDialogue(bool invokeGlobalEndEvent = true)
    {
        IsRunning = false;
        dialoguePanel?.SetActive(false);
        optionsPanel?.SetActive(false); // Garante que o painel de op��es esteja escondido
        nextButton.gameObject.SetActive(true); // Garante que o bot�o 'Next' esteja vis�vel (para um eventual pr�ximo di�logo)

        ScriptLuzia?.LuziaVolta(); // Chama o m�todo "voltar" da Luzia

        // Invoca os eventos de fim
        currentDialogueDataOnEndEvent?.Invoke(); // Evento espec�fico do DialogueData atual

        if (invokeGlobalEndEvent)
        {
            onDialogueEndGlobal?.Invoke(); // Evento global do Manager (somente se o fluxo de di�logo realmente terminou)
        }

        // Limpa as refer�ncias para evitar refer�ncias cruzadas indesejadas
        currentLines = null;
        currentDialogueDataOnStartEvent = null;
        currentDialogueDataOnEndEvent = null;
        currentOptions = null;
        index = -1; // Reseta o �ndice
    }

    // Define as informa��es do personagem na UI
    private void SetCharacterInfo(DialogueData data)
    {
        if (nameTextA != null) nameTextA.text = data.characterName1;
        if (nameTextB != null) nameTextB.text = data.characterName2;
        if (portraitImageA != null) portraitImageA.sprite = data.characterPortrait1;
        if (portraitImageB != null) portraitImageB.sprite = data.characterPortrait2;
    }

    // M�todo de exemplo para ser chamado pelos eventos do DialogueData (para fins de teste/demonstra��o)
    public void ExampleManagerMethod()
    {
        Debug.Log("M�todo 'ExampleManagerMethod' no DialogManager foi invocado por um evento do DialogueData!");
    }

    public void LoadNewScene(string sceneName)
    {
        Debug.Log($"[DialogManager] Solicitado carregar cena: {sceneName}");
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    // Mais m�todos p�blicos que seus DialogueEvents podem chamar (Ex: dar item, tocar som, etc.)
    public void GivePlayerItem(string itemName)
    {
        Debug.Log($"[DialogManager] Jogador recebeu item: {itemName}");
        // Implemente a l�gica de invent�rio aqui
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