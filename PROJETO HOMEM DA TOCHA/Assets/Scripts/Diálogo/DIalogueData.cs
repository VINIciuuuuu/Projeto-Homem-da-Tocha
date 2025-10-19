using UnityEngine;
using UnityEngine.Events; // Necess�rio para UnityEvent
using System.Collections.Generic;

// Esta classe auxiliar representa uma �nica op��o de di�logo
[System.Serializable]
public class DialogueOption
{
    [TextArea(1, 3)]
    public string optionText; // O texto que o jogador ver� para esta op��o
    public DialogueData nextDialogue; // O DialogueData que ser� carregado se esta op��o for escolhida
    public UnityEvent onOptionSelected; // Eventos que acontecem ao selecionar esta op��o
                                        // Adicione aqui condi��es para a op��o ser vis�vel/selecion�vel (ex: item no invent�rio, quest completa)
                                        // public bool IsAvailable() { /* l�gica para verificar disponibilidade */ return true;
}


[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Conte�do do Di�logo")]
    [TextArea(3, 10)]
    public string[] lines; // As falas sequenciais antes das op��es (se houver)
    public Sprite characterPortrait1; // Imagem do personagem falando
    public Sprite characterPortrait2; // Imagem do personagem falando
    public string characterName1; // Nome do personagem
    public string characterName2; // Nome do personagemaaa

    [Header("Op��es de Di�logo (se houver)")]
    public bool hasOptions = false; // Flag para indicar se este di�logo termina com op��es
    public List<DialogueOption> options; // Lista de op��es para o jogador escolher

    [Header("Eventos")]
    public UnityEvent onDialogueStartEvent; // Eventos que acontecem QUANDO ESTE di�logo espec�fico come�a
    public UnityEvent onDialogueEndEvent;   // Eventos que acontecem QUANDO ESTE di�logo espec�fico termina (ap�s todas as falas ou ap�s uma op��o ser selecionada

    [Header("Configura��o de Heran�a")]
    public DialogueData dialogoPai; // Refer�ncia ao DialogueData "pai"

    private void OnValidate()
    {
        PuxarDadosDoPai();
    }

    // OnEnable � chamado quando o ScriptableObject � carregado.
    // Garante que os dados sejam puxados tamb�m em tempo de execu��o se for necess�rio.
    private void OnEnable()
    {
        PuxarDadosDoPai();
    }

    public void PuxarDadosDoPai()
    {
        if (dialogoPai == null)
        {
            // N�o h� pai para puxar, este � um di�logo "raiz" ou um que n�o herda
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