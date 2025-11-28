using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Data", fileName = "NewItemData")]

public class ItemData : ScriptableObject
{
    [Header("Identificação")]
    public string itemId;          // um ID único ("potion_small", "coin", etc.)
    public string displayName;     // nome visível

    [Header("Visual")]
    public Sprite icon;            // ícone para UI

    [Header("Empilhamento")]
    public bool stackable = true;  // pode acumular?
    [Min(1)] public int maxStack = 99;
}
    
