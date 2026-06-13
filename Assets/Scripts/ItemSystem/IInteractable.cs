using UnityEngine.Localization;

public interface IInteractable
{
    /// <summary>
    /// Crosshair'de gösterilecek metin.
    /// </summary>
    LocalizedString InteractHint { get; }
    
    LocalizedString InteractName { get; }

    bool CanInteract { get; }
    void Interact();
}