using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Spawning;
using MLAPI.Transports.UNET;

public class ConnectionManager : MonoBehaviour
{
    public GameObject ConnectionButtonPanel = null;
    public GameObject IPInputPanel = null;
    public Camera LobbyCamera = null;

    public float PlaneMax = 10f;
    public float PlaneMin = -10f;

    private string IPAddress = "127.0.0.1";
    private UNetTransport transport;

    private void Awake()
    {
        ConnectionButtonPanel.SetActive(true);
        IPInputPanel.SetActive(false);
    }

    // Happen on server
    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost(GetRandomPosition(), Quaternion.identity);

        ConnectionButtonPanel.SetActive(false);
        LobbyCamera.gameObject.SetActive(false);
    }   

    public void Join()
    {
        IPInputPanel.SetActive(true);
    }
    
    public void IPAddressChanged(string newAddress)
    {
        this.IPAddress = newAddress;
    }

    public void SubmitAddress()
    {
        Debug.Log(IPAddress);

        transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        transport.ConnectAddress = IPAddress;

        IPInputPanel.SetActive(false);
        LobbyCamera.gameObject.SetActive(false);
        ConnectionButtonPanel.SetActive(false);
        
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("Password1234");
        NetworkManager.Singleton.StartClient();
    }

    public void CancelJoin()
    {
        IPInputPanel.SetActive(false);
    }

    // Happen on server
    private void ApprovalCheck(byte[] connectionData, ulong clientID, NetworkManager.ConnectionApprovedDelegate callback)
    {
        //Check the incoming data
        bool approve = System.Text.Encoding.ASCII.GetString(connectionData) == "Password1234";
        callback(true, null, approve, GetRandomPosition(), Quaternion.identity);
    } 

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(PlaneMin, PlaneMax);
        float z = Random.Range(PlaneMin, PlaneMax);
        return new Vector3(x, 1f, z);
    }

}
