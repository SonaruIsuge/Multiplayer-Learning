using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    public Text ammoText;
    public PlayerEquipment localPlayer;
    public IWeaponAction currentWeapon;
    void Awake()
    {
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
            out var networkedClient))
        {
            if (!localPlayer)
            {
                localPlayer = networkedClient.PlayerObject.GetComponent<PlayerEquipment>();
                localPlayer.OnCurrentWeaponChange += changeCurrentWeapon;
            }
        }

        if (currentWeapon != null) ammoText.text = currentWeapon.GetAmmo().ToString();
        else ammoText.text = "";
    }

    void changeCurrentWeapon(IWeaponAction current)
    {
        currentWeapon = current;
    }
}
