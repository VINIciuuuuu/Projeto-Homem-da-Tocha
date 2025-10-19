using UnityEngine;
using UnityEngine.Events; // Necessário para UnityEvent
using System.Collections.Generic;

// Esta classe auxiliar representa uma única opção de diálogo
[System.Serializable]
public class DialogueOption
{
    [TextArea(1, 3)]
    public string optionText; // O texto que o jogador verá para esta opção
    public DialogueData nextDialogue; // O DialogueData que será carregado se esta opção for escolhida
    public UnityEvent onOptionSelected; // Eventos que acontecem ao selecionar esta opção
                                        // Adicione aqui condições para a opção ser visível/selecionável (ex: item no inventário, quest completa)
                                        // public bool IsAvailable() { /* lógica para verificar disponibilidade */ return true;
}


[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Conteúdo do Diálogo")]
    [TextArea(3, 10)]
    public string[] lines; // As falas sequenciais antes das opções (se houver)
    public Sprite characterPortrait1; // Imagem do personagem falando
    public Sprite characterPortrait2; // Imagem do personagem falando
    public string characterName1; // Nome do personagem
    public string characterName2; // Nome do personagemaaa

    [Header("Opções de Diálogo (se houver)")]
    public bool hasOptions = false; // Flag para indicar se este diálogo termina com opções
    public List<DialogueOption> options; // Lista de opções para o jogador escolher

    [Header("Eventos")]
    public UnityEvent onDialogueStartEvent; // Eventos que acontecem QUANDO ESTE diálogo específico começa
    public UnityEvent onDialogueEndEvent;   // Eventos que acontecem QUANDO ESTE diálogo específico termina (após todas as falas ou após uma opção ser selecionada

    [Header("Configuração de Herança")]
    public DialogueData dialogoPai; // Referência ao DialogueData "pai"

    private void OnValidate()
    {
        PuxarDadosDoPai();
    }

    // OnEnable é chamado quando o ScriptableObject é carregado.
    // Garante que os dados sejam puxados também em tempo de execução se for necessário.
    private void OnEnable()
    {
        PuxarDadosDoPai();
    }

    public void PuxarDadosDoPai()
    {
        if (dialogoPai == null)
        {
            // Não há pai para puxar, este é um diálogo "raiz" ou um que não herda
            return;
        }

        if (string.IsNullOrEmpty(characterName1))
        {
            characterName1 = dialogoPai.characterName1;
        }

        if (string.IsNullOrEmpty(characterName2))
        {
            characterName2 = dialogoPai.characterName2;
        }

        if (characterPortrait1 == null)
        {
            characterPortrait1 = dialogoPai.characterPortrait1;
        }

        if (characterPortrait2 == null)
        {
            characterPortrait2 = dialogoPai.characterPortrait2;
        }
    }
}