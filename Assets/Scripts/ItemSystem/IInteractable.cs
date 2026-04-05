using UnityEngine.Localization;

public interface IInteractable
{
    /// <summary>
    /// Crosshair'de "E: ___" kısmında gösterilecek metin.
    /// Örn: "Aç", "Konuş", "Kullan"
    /// </summary>
    LocalizedString InteractHint { get; }

    /// <summary>Oyuncu E tuşuna bastığında çağrılır.</summary>
    void Interact();
}