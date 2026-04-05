using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkDevicesChangedEventArgs : EventArgs
{
    public readonly List<NetworkDeviceSO> connectedDevices = new List<NetworkDeviceSO>();
    public NetworkDeviceSO changedDevice;

    public NetworkDevicesChangedEventArgs(List<NetworkDeviceSO> connectedDevices,  NetworkDeviceSO changedDevice)
    {
        this.connectedDevices = connectedDevices;
        this.changedDevice = changedDevice;
    }
} 

public class Router : MonoBehaviour
{
    public static Router Instance { get; private set; }
    
    public event EventHandler<NetworkDevicesChangedEventArgs> OnNetworkDevicesChanged;
    
    private HashSet<NetworkDeviceSO> connectedDevices =  new HashSet<NetworkDeviceSO>();

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
    }

    public void Connect(NetworkDeviceSO device)
    {
        connectedDevices.Add(device);
        OnNetworkDevicesChanged?.Invoke(this, new NetworkDevicesChangedEventArgs(connectedDevices.ToList(), device));
    }

    public void Disconnect(NetworkDeviceSO device)
    {
        if (!connectedDevices.Contains(device))
        {
            Debug.LogError($"{device.name} is not connected");
            return;
        }
        connectedDevices.Remove(device);
        OnNetworkDevicesChanged?.Invoke(this, new NetworkDevicesChangedEventArgs(connectedDevices.ToList(), device));
    }

    public List<NetworkDeviceSO> GetDevicesByType(NetworkDeviceType deviceType)
    {
        return connectedDevices.Where(device => device.NetworkDeviceType == deviceType).ToList();
    }
}
