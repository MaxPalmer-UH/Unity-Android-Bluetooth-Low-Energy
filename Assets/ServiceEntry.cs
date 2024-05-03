using Android.BLE;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ServiceEntry : MonoBehaviour
{
    public Image ImageBackgroundTarget;

    private string _serviceUuid = string.Empty;
    private string _serviceName = string.Empty;

    [SerializeField]
    private Text _serviceUuidText;


    [SerializeField]
    private Text _serviceButtonText;

    [SerializeField]
    private Color _onConnectedColorA;
    [SerializeField]
    private Color _onConnectedColorB;

    private bool _isConnected = false;

    private BleGattService _bleservice;

    private float _rssiTimer = 0f;

    public void Show(BleGattService service)
    {
        _serviceButtonText.text = "Subscribe";

        _serviceUuid = service.UUID;
        _serviceUuidText.text = service.UUID;

        _bleservice = service;
    }

    public void Update()
    {
    }

    public void Subscribe()
    {
        _isConnected = true;

        foreach (var characteristic in _bleservice.Characteristics)
        {
            Debug.Log($"Service has characteristics {characteristic.UUID}");

            if (characteristic.UUID == "00000002-1e3c-fad4-74e2-97a033f1bfab")
            {
                Debug.Log($"Discovered and subscribed to target characteristic {characteristic.UUID} on service {_bleservice.UUID}");
                characteristic.Subscribe(OnSubscribedValue);
            }
        }

        //device.GetCharacteristic("180C", "2A56").Subscribe((value) =>
        //{
        //    Debug.Log(Encoding.UTF8.GetString(value));
        //});
    }

    public void OnSubscribedValue(byte[] data)
    {
        Debug.Log($"Service characteristic data changed {data}");

        if (ImageBackgroundTarget.color != _onConnectedColorA)
        {
            ImageBackgroundTarget.color = _onConnectedColorA;    
        }
        else
        {
            ImageBackgroundTarget.color = _onConnectedColorB;
        }
    }
}
