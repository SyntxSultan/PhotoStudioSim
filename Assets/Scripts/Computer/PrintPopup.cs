using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrintPopup : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown printerSelectDropdown;
    [SerializeField] private TMP_Dropdown paperSizeSelectDropdown;
    [SerializeField] private TMP_Dropdown orientationSelectDropdown;
    [SerializeField] private TMP_Dropdown fitSelectDropdown;
    [SerializeField] private RawImage printingImagePrewiev;
    [SerializeField] private Button printButton;
    [SerializeField] private Button cancelButton;
    
    public event Action<PrintSettings> OnPrint;
    
    private List<NetworkDeviceSO> availablePrinters = new List<NetworkDeviceSO>();

    private void Awake()
    {
        PopulateDropdownItems(paperSizeSelectDropdown, typeof(PrintPaperSize));
        PopulateDropdownItems(orientationSelectDropdown, typeof(PrintPaperOrientation));
        PopulateDropdownItems(fitSelectDropdown, typeof(PrintPaperFit));
        
        printButton.onClick.AddListener(PrintButtonClicked);
        cancelButton.onClick.AddListener(ClosePrintPopup);
    }

    private void OnEnable()
    {
        RefreshPrinterList();
    }
    
    private void RefreshPrinterList()
    {
        availablePrinters = Router.Instance.GetDevicesByType(NetworkDeviceType.Printer);
        printerSelectDropdown.ClearOptions();

        if (availablePrinters.Count == 0)
        {
            printerSelectDropdown.AddOptions(new List<string> { "Yazıcı Bulunamadı" });
            printButton.interactable = false;
            return;
        }

        printButton.interactable = true;
        List<string> printerNames = availablePrinters.Select(p => p.NetworkDeviceName).ToList();
        printerSelectDropdown.AddOptions(printerNames);
    }

    public void SetPreviewImage(Texture2D texture)
    {
        printingImagePrewiev.texture = texture;
    }

    private void PrintButtonClicked()
    {
        OnPrint?.Invoke(GeneratePrintSettings());
        ClosePrintPopup();
    }

    private PrintSettings GeneratePrintSettings()
    {
        NetworkDeviceSO selectedPrinter = null;
        if (availablePrinters.Count > 0)
        {
            selectedPrinter = availablePrinters[printerSelectDropdown.value];
        }
        
        PrintSettings printSettings = new PrintSettings
        {
            targetPrinter = selectedPrinter,
            paperSize = ParseDropdownValue<PrintPaperSize>(paperSizeSelectDropdown),
            paperOrientation = ParseDropdownValue<PrintPaperOrientation>(orientationSelectDropdown),
            paperFit = ParseDropdownValue<PrintPaperFit>(fitSelectDropdown)
        };

        return printSettings;
    }

    // Helper: Dropdown değerini verilen Enum tipine çevirir
    private T ParseDropdownValue<T>(TMPro.TMP_Dropdown dropdown) where T : struct
    {
        if (Enum.TryParse(dropdown.value.ToString(), out T result))
        {
            return result;
        }
        return default;
    }
    
    private void ClosePrintPopup()
    {
        gameObject.SetActive(false);
    }

    private void PopulateDropdownItems(TMP_Dropdown dropdown, Type enumType)
    {
        dropdown.ClearOptions();
        string[] enumNames = Enum.GetNames(enumType);
        List<string> options = new List<string>(enumNames);
        dropdown.AddOptions(options);
    }
}
