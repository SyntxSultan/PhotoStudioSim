using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Masaüstü yöneticisi.
///
/// SORUMLULUKLAR:
///   - Yüklü uygulama listesi (install/uninstall)
///   - Pencere aç/kapat
///   - Z-order (BringToFront)
///   - Oturum dışında tüm pencereleri gizle
///
/// SAHNE KURULUMU:
///   windowParent: Canvas içinde pencerelerin instantiate edileceği RectTransform.
///   initialApps: başlangıçta yüklü AppDefinition'lar.
/// </summary>
public class DesktopController : MonoBehaviour
{
    [SerializeField] private ComputerSplashScreen splashScreen;
    [SerializeField] private float computerOpenTime = 3; 
    [SerializeField] private RectTransform windowParent;
    [SerializeField] private List<AppDefinition> initialApps = new();
    

    private readonly List<AppDefinition> installedApps = new();
    private readonly List<AppWindow> openWindows = new();
    private readonly Dictionary<AppDefinition, AppWindow> activeWindows = new();


    private void OnDisable()
    {
        ComputerState.Instance.OnPowerOn -= OnSessionStart;
        ComputerState.Instance.OnPowerOut -= OnSessionEnd;
    }

    private void Start()
    {
        ComputerState.Instance.OnPowerOn += OnSessionStart;
        ComputerState.Instance.OnPowerOut += OnSessionEnd;

        foreach (var app in initialApps.Where(a => a != null && a.installedByDefault))
            InstallApp(app);

        windowParent.gameObject.SetActive(false);
    }

    private void OnSessionStart()
    {
        StartCoroutine(SplashScreen());
    }

    private System.Collections.IEnumerator SplashScreen()
    {
        splashScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(computerOpenTime);
        DOTween.Sequence().Append(splashScreen.GetComponent<CanvasGroup>().DOFade(0, 0.25f))
            .OnComplete(() =>
            {
                splashScreen.gameObject.SetActive(false);
                windowParent.gameObject.SetActive(true);
            });
    }

    private void OnSessionEnd()
    {
        foreach (var w in openWindows.ToList())
            DestroyWindow(w);

        openWindows.Clear();
        activeWindows.Clear();

        windowParent.gameObject.SetActive(false);
    }

    public void InstallApp(AppDefinition def)
    {
        if (!installedApps.Contains(def))
            installedApps.Add(def);
    }

    public void UninstallApp(AppDefinition def)
    {
        if (activeWindows.TryGetValue(def, out var window))
            CloseWindow(window);

        installedApps.Remove(def);
    }

    public IReadOnlyList<AppDefinition> InstalledApps => installedApps;


    /// <summary>
    /// Uygulamayı açar. Zaten açıksa mevcut pencereyi öne getirir.
    /// </summary>
    public AppWindow OpenApp(AppDefinition def)
    {
        if (!installedApps.Contains(def))
        {
            Debug.LogWarning($"[Desktop] {def.appName} yüklü değil.");
            return null;
        }

        if (def.windowPrefab == null)
        {
            Debug.LogError($"[Desktop] {def.appName} does not have a window prefab assigned!");
            return null;
        }

        if (activeWindows.TryGetValue(def, out var existing))
        {
            BringToFront(existing);
            return existing;
        }

        AppWindow window = Instantiate(def.windowPrefab, windowParent);
        window.Setup(def, this);

        openWindows.Add(window);
        activeWindows[def] = window;

        BringToFront(window);
        return window;
    }

    public void CloseWindow(AppWindow window)
    {
        if (!openWindows.Contains(window)) return;
        activeWindows.Remove(window.Definition);
        openWindows.Remove(window);
        DestroyWindow(window);
    }


    public void BringToFront(AppWindow window)
    {
        window.transform.SetAsLastSibling();
    }


    private void DestroyWindow(AppWindow window)
    {
        if (window != null) Destroy(window.gameObject);
    }
}