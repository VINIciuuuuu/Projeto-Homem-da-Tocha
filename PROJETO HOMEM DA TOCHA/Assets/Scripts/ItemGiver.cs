using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    [Header("Referência do Inventário")]
    public Inventory playerInventory;

    [Header("Item Padrão (opcional)")]
    public ItemData defaultItem;
    public int defaultAmount = 1;

    void Start()
    {
        // Se não foi atribuído manualmente, tenta encontrar o inventário do player
        if (playerInventory == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<Inventory>();
            }
        }
    }

    /// <summary>
    /// Adiciona um item ao inventário do player
    /// </summary>
    /// <param name="item">O ItemData do item a ser adicionado</param>
    /// <param name="amount">A quantidade a ser adicionada</param>
    public void GiveItem(ItemData item, int amount)
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("[ItemGiver] Inventário do player não encontrado!");
            return;
        }

        if (item == null)
        {
            Debug.LogWarning("[ItemGiver] ItemData é null!");
            return;
        }

        int remaining = playerInventory.AddItem(item, amount);

        if (remaining == 0)
        {
            Debug.Log($"[ItemGiver] {amount}x {item.displayName} adicionado ao inventário!");
        }
        else
        {
            Debug.LogWarning($"[ItemGiver] Inventário cheio! {remaining} itens não puderam ser adicionados.");
        }
    }

    /// <summary>
    /// Adiciona o item padrão configurado no Inspector
    /// </summary>
    public void GiveDefaultItem()
    {
        if (defaultItem != null)
        {
            GiveItem(defaultItem, defaultAmount);
        }
        else
        {
            Debug.LogWarning("[ItemGiver] Nenhum item padrão configurado!");
        }
    }

    /// <summary>
    /// Adiciona uma urtiga ao inventário (método específico para facilitar)
    /// </summary>
    /// <param name="amount">Quantidade de urtigas (padrão: 1)</param>
    public void GiveUrtiga(int amount = 1)
    {
        // Procura o asset da Urtiga em Resources
        ItemData urtiga = Resources.Load<ItemData>("Urtiga");

        if (urtiga != null)
        {
            GiveItem(urtiga, amount);
        }
        else
        {
            Debug.LogError("[ItemGiver] Asset da Urtiga não encontrado em Resources! Configure o item manualmente no método GiveItem ou use GiveDefaultItem.");
        }
    }
}
