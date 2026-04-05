using UnityEngine;

public enum PrintPaperSize
{
    A3,
    A4,
    A5
}

public enum PrintPaperOrientation
{
    Portrait,
    Landscape
}

public enum PrintPaperFit
{
    ScaleToFit,
    ActualSize
}

public struct PrintSettings
{
    public NetworkDeviceSO targetPrinter;
    public PrintPaperSize paperSize;
    public PrintPaperOrientation paperOrientation;
    public PrintPaperFit paperFit;
}