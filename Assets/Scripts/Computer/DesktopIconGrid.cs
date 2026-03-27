using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Yüklü uygulamaların ikonlarını masaüstünde gösterir.
/// Çift tıklayınca DesktopController.OpenApp() çağrılır.
///
/// SAHNE KURULUMU:
///   iconParent: Grid Layout Group olan RectTransform
///   iconPrefab: Image (ikon) + TMP (ad) + Button olan prefab
/// </summary>
public class DesktopIconGrid : MonoBehaviour
{
    [SerializeField] private DesktopController desktop;
    [SerializeField] private RectTransform iconParent;
    [SerializeField] private DesktopIcon iconPrefab;

    private void Start()
    {
        ComputerState.Instance.OnPowerOn += Refresh;
    }

    private void OnDisable()
    {
        ComputerState.Instance.OnPowerOn -= Refresh;
    }

    private void Refresh()
    {
        Clear();
        foreach (var app in desktop.InstalledApps)
        {
            var icon = Instantiate(iconPrefab, iconParent);
            icon.Setup(app, desktop);
        }
    }

    private void Clear()
    {
        foreach (Transform child in iconParent)
            Destroy(child.gameObject);
    }
}