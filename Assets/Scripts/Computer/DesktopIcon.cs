using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]

/// <summary>
/// Masaüstü ikon prefabı'na eklenir.
/// Çift tıklama: uygulamayı aç.
/// </summary>
public class DesktopIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button button;

    private AppDefinition def;
    private DesktopController desktop;

    // Çift tıklama takibi
    private float lastClickTime;
    private const float DoubleClickThreshold = 0.3f;

    public void Setup(AppDefinition appDef, DesktopController desktopCtrl)
    {
        def = appDef;
        desktop = desktopCtrl;

        if (iconImage && def.icon) iconImage.sprite = def.icon;
        if (label) label.text = def.appName;
        if (button) button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        float now = Time.unscaledTime;
        if (now - lastClickTime <= DoubleClickThreshold)
            desktop.OpenApp(def);

        lastClickTime = now;
    }
}