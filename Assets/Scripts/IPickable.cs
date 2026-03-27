using UnityEngine;

/// <summary>
/// Yerden alınabilen her item bu arayüzü implement eder.
/// Sadece interact edilebilen objeler bu arayüzü implement ETMEZ.
/// </summary>
public interface IPickable
{
    /// <summary>Item'ın ekranda görünecek adı.</summary>
    string ItemName { get; }

    /// <summary>Crosshair'de "E: ___" kısmında gösterilecek metin. Örn: "Al", "Topla"</summary>
    string PickupHint { get; }

    /// <summary>Oyuncu itemi eline aldığında çağrılır.</summary>
    void OnPickup(Transform holdPoint);

    /// <summary>Oyuncu itemi bıraktığında/fırlattığında çağrılır.</summary>
    void OnDrop(Vector3 throwDirection, float throwForce);
}