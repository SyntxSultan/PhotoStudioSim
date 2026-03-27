using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Oyun dünyasındaki kamera objesi.
///
/// SAHNE KURULUMU:
///   1) Boş bir GameObject yap, adı "Camera"
///   2) Child olarak bir Camera ekle → bu script'in lensCamera alanına sürükle
///   3) lensCamera'nın targetTexture'ına bir RenderTexture asset'i ata
///      (aynı RT'yi viewfinderTexture alanına da ver)
///   4) Bu objeyi IInteractable layer'a al (oyuncu kamerayı alıp taşıyabilirse)
///   5) Bilgisayarda bir UI butonu veya Computer.cs'de F tuşu → TransferToComputer()
///
/// FOTOĞRAF ÇEKME:
///   Kamerayı "tutan" sistemden her frame shootKey (default: Mouse0) ile çekim yapılır.
///   Viewfinder için lensCamera'nın RT'sini bir RawImage'a ata.
/// </summary>
 
public class ItemCamera : BasePickableItem, IUsable
{
    [Header("Lens")]
    [SerializeField] private Camera lensCamera;
    [SerializeField] private Canvas screenCanvas;
    [SerializeField] private GameObject flash;

    [Tooltip("lensCamera.targetTexture ile aynı RenderTexture olmalı.")]
    [SerializeField] private RenderTexture viewfinderTexture;

    [Header("Fotoğraf Ayarları")]
    [Tooltip("Kaydedilen fotoğrafların çözünürlüğü.")]
    [SerializeField] private Vector2Int photoResolution = new(256, 256);

    [Tooltip("Maksimum kaç fotoğraf saklanır (aktarılmamış).")]
    [SerializeField] private int maxLocalPhotos = 10;

    [Header("Ses / Efekt (opsiyonel)")]
    [SerializeField] private AudioSource shutterSound;

    private readonly List<Texture2D> localPhotos = new();

    private bool isHeld;

    protected override void OnPickedUp()
    {
        isHeld = true;
        lensCamera.enabled = true;
        screenCanvas.gameObject.SetActive(true);
    }

    protected override void OnDropped()
    {
        isHeld = false;
        lensCamera.enabled = false;
        screenCanvas.gameObject.SetActive(false);
    }


    /// <summary>
    /// Tek fotoğraf çek. Viewfinder RT'sini Texture2D'ye kopyalar.
    /// Çağrı: oyuncu input'u veya harici sistem.
    /// </summary>
    private bool TakePhoto()
    {
        if (localPhotos.Count >= maxLocalPhotos)
        {
            Debug.Log("[GameCamera] Hafıza dolu, önce bilgisayara aktar.");
            return false;
        }

        if (viewfinderTexture == null)
        {
            Debug.LogWarning("[GameCamera] viewfinderTexture atanmamış!");
            return false;
        }

        Texture2D snap = new Texture2D(photoResolution.x, photoResolution.y, TextureFormat.RGB24, false);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = viewfinderTexture;

        snap.ReadPixels(new Rect(0, 0, viewfinderTexture.width, viewfinderTexture.height), 0, 0);
        snap.Apply();
        
        RenderTexture.active = prev;
        
        SaveAsPNG(snap);

        localPhotos.Add(snap);

        if (shutterSound) shutterSound.Play();
        if (flash) StartCoroutine(FlashBang());

        return true;
    }

    private IEnumerator FlashBang()
    {
        flash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        flash.SetActive(false);
    }

    private void SaveAsPNG(Texture2D textureToSave)
    {
        byte[] bytes = textureToSave.EncodeToPNG();

        string folderPath = Path.Combine(Application.persistentDataPath, "SavedPhotos");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, $"Photo_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        try
        {
            File.WriteAllBytes(filePath, bytes);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }

    /// <summary>
    /// Kameradaki tüm fotoğrafları CameraStorage'a aktar.
    /// Bilgisayar başında F tuşuna basınca çağrılır.
    /// </summary>
    public void TransferToComputer()
    {
        if (localPhotos.Count == 0)
        {
            Debug.Log("[GameCamera] Aktarılacak fotoğraf yok.");
            return;
        }

        CameraStorage.Instance.Upload(localPhotos);
        int count = localPhotos.Count;
        localPhotos.Clear();

        Debug.Log($"[GameCamera] {count} fotoğraf bilgisayara aktarıldı.");
    }

    /// <summary>Kamerada bekleyen fotoğraf sayısı.</summary>
    public int PendingCount => localPhotos.Count;

    public string UseHint => "Fotoğraf Çek";

    private void Update()
    {
        if (!isHeld) return;

        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            TransferToComputer();
        }
    }

    public void OnUseStart()
    {
        if (!isHeld) return;
        Debug.Log("Sol tık basıldı");
        TakePhoto();
    }

    public void OnUseStop()
    {
        
    }
}