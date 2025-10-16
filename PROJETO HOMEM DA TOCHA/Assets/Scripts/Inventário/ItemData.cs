using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Data", fileName = "NewItemData")]

public class ItemData : ScriptableObject
{
    [Header("Identificação")]
    public string ItemId;
    public string displayName;

    [Header("Visual")]
    public Sprite icon;

    [Header("Empilhamento")]
    public bool stackable = true;
    [Min(1)] public int maxStack = 99;
}
