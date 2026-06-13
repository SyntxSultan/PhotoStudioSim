using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewItem", menuName = "PSSGame/Item System/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Header("Genel")]
    public LocalizedString itemName;
    public Sprite icon;
    public BasePickableItem itemPrefab;
    
    [Header("Elde Tutma Pozisyonu")]
    [Tooltip("Hold point'e göre lokal pozisyon ofseti.")]
    public Vector3 holdPositionOffset = Vector3.zero;

    [Tooltip("Hold point'e göre lokal rotasyon ofseti.")]
    public Vector3 holdRotationOffset = Vector3.zero;
}