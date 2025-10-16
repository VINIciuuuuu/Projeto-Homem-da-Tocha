using System.Text;
using UnityEngine;
using UnityEngine.UI; // troque para TMPro se usar TextMeshProUGUI

public class InventoryUI : MonoBehaviour
{
    [Header("Referências")]
    public Inventory inventory; // arraste o Inventory do Player
    public Text listText;       // ou TextMeshProUGUI se preferir
    [Header("Configuração")]
    public GameObject inventoryPanel;

    private bool isOpen = false;

    private void Start()
    {
        // Garante que o inventário comece fechado
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Detecta a tecla TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    private void OnEnable()
    {
        if (inventory != null) inventory.OnInventoryChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null) inventory.OnInventoryChanged -= Refresh;
    }

    public void Refresh()
    {
        if (inventory == null || listText == null) return;

        var sb = new StringBuilder();
        sb.AppendLine("Inventário:");
        foreach (var slot in inventory.slots)
        {
            string name = slot.item != null ? slot.item.displayName : "(null)";
            sb.AppendLine($"- {name} x{slot.count}");
        }

        listText.text = sb.ToString();
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            Refresh();
            Time.timeScale = 0f;    // pausa o jogo se quiser
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}