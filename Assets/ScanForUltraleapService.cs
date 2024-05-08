using Android.BLE;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class ScanForUltraleapService : MonoBehaviour
{
    [SerializeField]
    public string TargetMACAddress = "E8:FB:1C:B9:39:F2";

    [SerializeField]
    private GameObject _serviceItem;
    [SerializeField]
    private Transform _serviceList;
    [SerializeField]
    private Image _imageBackgroundTarget;
    private bool _isScanning = false;

    private bool hasAllPermision = false;

    private void Start()
    {
        Debug.Log(Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN"));
    }

    public void RequestScanPermission()
    {
        var per = Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN");
        Debug.Log(per);
        if (!per)
        {
            Permission.RequestUserPermission("android.permission.BLUETOOTH_SCAN");
        }

    }

    public void RequestConnectPermission()
    {
        var per = Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT");
        Debug.Log(per);
        if (!per)
        {
            Permission.RequestUserPermission("android.permission.BLUETOOTH_CONNECT");
        }
    }

    public void RequestBoth()
    {
        Permission.RequestUserPermissions(new string[] { "android.permission.BLUETOOTH_SCAN", "android.permission.BLUETOOTH_CONNECT" });
    }

    private void OnDeviceFound(BleDevice device)
    {
        Debug.Log($"{device.Name}");

        //if (device.Name == "Ultraleap")
        try
        {;
            if (device.Name == "ULBLE")
            {
                Debug.Log($">>>>>>>>>>>>>> Found {device.Name} - connecting");
                device.Connect(OnDeviceConnected, OnDeviceDisconnected);
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Failed while calling device.Connect {e.Message}");
        }
    }

    public void OnDeviceConnected(BleDevice device)
    {
        Debug.Log($">>>>>>>>>>>>>> Connected to {device.Name}");

        foreach (var service in device.Services)
        {
            Debug.Log($"Discovered service {service.UUID}");

            if (service.UUID == "00000001-1e3c-fad4-74e2-97a033f1bfab")
            {
                Debug.Log($"Listing target service {service.UUID}");
                var serviceItem = Instantiate(_serviceItem, _serviceList).GetComponent<ServiceEntry>();
                serviceItem.ImageBackgroundTarget = _imageBackgroundTarget;
                serviceItem.Show(service);
            }
        }
    }

    public void OnDeviceDisconnected(BleDevice device)
    {
        Debug.Log($">>>>>>>>>>>>>> Disconnected from {device.Name}");
    }

    public void ConnectToUltraleapService()
    {
        Debug.Log($"ConnectToUltraleapService, searching for {TargetMACAddress}");

        if (!_isScanning)
        {
            if (!hasAllPermision)
            {
                if (Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN"))
                {
                    if (Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
                    {
                        hasAllPermision = true;
                        if (!_isScanning)
                        {
                            _isScanning = true;

                            try
                            {
                                Debug.Log($"Calling SearchForDevicesWithFilter");
                                //BleManager.Instance.SearchForDevices(30 * 1000, OnDeviceFound);
                                BleManager.Instance.SearchForDevicesWithFilter(60 * 1000, OnDeviceFound, TargetMACAddress);
                            }
                            catch (Exception e)
                            {
                                Debug.Log($"Failed while calling BleManager.Instance.SearchForDevicesWithFilter {e.Message}");
                            }
                        }
                    }
                    else
                    {
                        Permission.RequestUserPermission("android.permission.BLUETOOTH_CONNECT");
                        Debug.Log("Permission needed");
                        Debug.Log("Please click again to scan");
                    }
                }
                else
                {
                    Debug.Log("Permission needed");
                    Permission.RequestUserPermission("android.permission.BLUETOOTH_SCAN");
                }
            }
            else
            {
                try
                {
                    Debug.Log($"Calling SearchForDevicesWithFilter");
                    BleManager.Instance.SearchForDevices(30 * 1000, OnDeviceFound);
                    //BleManager.Instance.SearchForDevicesWithFilter(60 * 1000, OnDeviceFound, TargetMACAddress);
                }
                catch (Exception e)
                {
                    Debug.Log($"Failed while calling BleManager.Instance.SearchForDevicesWithFilter {e.Message}");
                }
            }      
        }
    }
}
