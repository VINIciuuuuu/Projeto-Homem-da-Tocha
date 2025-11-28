using System.Collections;
using UnityEngine;
using DialogueEditor;

/// <summary>
/// Script para verificar se o player possui uma quantidade específica de urtigas no inventário.
/// Se tiver, inicia um diálogo alternativo. Se não tiver, finaliza o diálogo atual.
/// Este script pode ser adicionado como evento no Dialogue Editor.
/// </summary>
public class DialogueUrtigaCheck : MonoBehaviour
{
    [Header("Configuração de Urtigas")]
    [Tooltip("ItemData da urtiga que será verificado no inventário")]
    public ItemData urtigaItem;
    
    [Tooltip("Quantidade mínima de urtigas necessárias para liberar o diálogo alternativo")]
    [Min(0)]
    public int quantidadeNecessaria = 1;

    [Header("Diálogo Alternativo")]
    [Tooltip("Diálogo que será iniciado se o player tiver a quantidade necessária de urtigas")]
    public NPCConversation dialogoAlternativo;

    [Header("Referência do Inventário (Opcional)")]
    [Tooltip("Se deixado vazio, tentará encontrar automaticamente no objeto com tag 'Player'")]
    public Inventory playerInventory;

    [Header("Configuração de Timing (Opcional)")]
    [Tooltip("Delay em segundos antes de iniciar o diálogo alternativo após finalizar o atual")]
    [Min(0)]
    public float delayParaNovoDialogo = 0.1f;

    /// <summary>
    /// Método público que pode ser chamado pelo UnityEvent do Dialogue Editor.
    /// Verifica o inventário e decide se inicia o diálogo alternativo ou finaliza o atual.
    /// </summary>
    public void VerificarUrtigasEIniciarDialogo()
    {
        // Tenta encontrar o inventário se não foi atribuído
        if (playerInventory == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<Inventory>();
            }
        }

        // Verifica se o inventário foi encontrado
        if (playerInventory == null)
        {
            Debug.LogError("[DialogueUrtigaCheck] Inventário do player não encontrado! Certifique-se de que o objeto com tag 'Player' possui o componente Inventory.");
            FinalizarDialogo();
            return;
        }

        // Verifica se o ItemData foi atribuído
        if (urtigaItem == null)
        {
            Debug.LogError("[DialogueUrtigaCheck] ItemData da urtiga não foi atribuído no inspector!");
            FinalizarDialogo();
            return;
        }

        // Verifica a quantidade de urtigas no inventário
        int quantidadeAtual = playerInventory.GetCount(urtigaItem);

        if (quantidadeAtual >= quantidadeNecessaria)
        {
            // Player tem urtigas suficientes - inicia o diálogo alternativo
            if (dialogoAlternativo != null)
            {
                // Finaliza o diálogo atual e inicia o novo após um pequeno delay
                StartCoroutine(FinalizarEIniciarNovoDialogo());
            }
            else
            {
                Debug.LogWarning("[DialogueUrtigaCheck] Diálogo alternativo não foi atribuído no inspector. Finalizando diálogo atual.");
                FinalizarDialogo();
            }
        }
        else
        {
            // Player não tem urtigas suficientes - finaliza o diálogo normalmente
            FinalizarDialogo();
        }
    }

    /// <summary>
    /// Corrotina que finaliza o diálogo atual e inicia o novo após um delay
    /// </summary>
    private IEnumerator FinalizarEIniciarNovoDialogo()
    {
        // Finaliza o diálogo atual
        FinalizarDialogo();
        
        // Aguarda um pequeno delay para garantir que a transição termine
        yield return new WaitForSeconds(delayParaNovoDialogo);
        
        // Inicia o novo diálogo
        if (ConversationManager.Instance != null)
        {
            ConversationManager.Instance.StartConversation(dialogoAlternativo);
        }
        else
        {
            Debug.LogError("[DialogueUrtigaCheck] ConversationManager.Instance não encontrado!");
        }
    }

    /// <summary>
    /// Finaliza o diálogo atual
    /// </summary>
    private void FinalizarDialogo()
    {
        if (ConversationManager.Instance != null && ConversationManager.Instance.IsConversationActive)
        {
            ConversationManager.Instance.EndConversation();
        }
    }
}

