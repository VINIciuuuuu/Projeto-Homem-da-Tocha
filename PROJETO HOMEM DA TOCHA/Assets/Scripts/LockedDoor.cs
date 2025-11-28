using UnityEngine;
using DialogueEditor;

/// <summary>
/// Script opcional para bloquear portas baseado na quantidade de urtigas.
/// Adicione este script APENAS nas portas que precisam de urtigas para serem abertas.
/// Portas sem este script funcionam normalmente (sem verificação).
/// </summary>
public class LockedDoor : MonoBehaviour
{
    [Header("Requisito de Urtigas")]
    [Tooltip("ItemData da urtiga necessária para abrir esta porta")]
    public ItemData urtigaItem;
    
    [Tooltip("Quantidade mínima de urtigas necessárias")]
    [Min(0)]
    public int quantidadeNecessaria = 1;

    [Header("Referência do Inventário (Opcional)")]
    [Tooltip("Se deixado vazio, tentará encontrar automaticamente no objeto com tag 'Player'")]
    public Inventory playerInventory;

    [Header("Feedback")]
    [Tooltip("Mensagem exibida quando a porta está bloqueada (apenas no console)")]
    public string mensagemBloqueio = "Você precisa de {0} urtigas para passar por aqui!";

    [Header("Diálogo de Bloqueio")]
    [Tooltip("Diálogo que será exibido quando o player tentar passar pela porta bloqueada. Deixe vazio se não quiser usar diálogo.")]
    public NPCConversation dialogoBloqueio;

    /// <summary>
    /// Verifica se o player tem urtigas suficientes para passar
    /// </summary>
    public bool PodePassar()
    {
        // Busca o inventário se não foi atribuído
        if (playerInventory == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<Inventory>();
            }
        }

        if (playerInventory == null)
        {
            Debug.LogWarning("[LockedDoor] Inventário do player não encontrado!");
            return false;
        }

        if (urtigaItem == null)
        {
            Debug.LogWarning("[LockedDoor] ItemData da urtiga não foi atribuído no inspector!");
            return false;
        }

        int quantidadeAtual = playerInventory.GetCount(urtigaItem);
        return quantidadeAtual >= quantidadeNecessaria;
    }

    /// <summary>
    /// Retorna a mensagem de bloqueio formatada
    /// </summary>
    public string GetMensagemBloqueio()
    {
        return string.Format(mensagemBloqueio, quantidadeNecessaria);
    }

    /// <summary>
    /// Exibe o diálogo de bloqueio se configurado
    /// </summary>
    public void ExibirDialogoBloqueio()
    {
        if (dialogoBloqueio != null && ConversationManager.Instance != null)
        {
            ConversationManager.Instance.StartConversation(dialogoBloqueio);
        }
        else if (dialogoBloqueio == null)
        {
            // Se não tem diálogo configurado, apenas mostra no console
            Debug.Log(GetMensagemBloqueio());
        }
        else
        {
            Debug.LogWarning("[LockedDoor] ConversationManager.Instance não encontrado!");
            Debug.Log(GetMensagemBloqueio());
        }
    }
}

