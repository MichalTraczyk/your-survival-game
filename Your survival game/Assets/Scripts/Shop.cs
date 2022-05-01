using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    public GameObject shopCamera;
    bool open;

    [SerializeField]
    TextMeshProUGUI currHealthText;
    [SerializeField]
    string closedMessage;
    [SerializeField]
    string openMessage;

    bool shotingEnabled;
    bool swordEnabled;
    private void Start()
    {
        GetComponent<Interactable>().Message = openMessage;
        UpdateShop();
    }
    public void ChangeShopState()
    {
        if (!open)
        {
            GetComponent<Interactable>().Message = closedMessage;
            open = true;
            OpenShop();
        }
        else
        {
            GetComponent<Interactable>().Message = openMessage;
            open = false;
            CloseShop();
        }
    }
    void OpenShop()
    {
        shopCamera.SetActive(true);
        GameManager.Instance.EnableCursor();
        Time.timeScale = 0;
        UIManager.Instance.ChangeCrosshairState(false);
        UpdateShop();
        //shotingEnabled = FindObjectOfType<ThirdPersonShooting>().enabled;
        //FindObjectOfType<ThirdPersonShooting>().enabled = false;
        FindObjectOfType<PlayerWeaponHandle>().enabled = false;
        //swordEnabled = FindObjectOfType<ThridPersonSword>().enabled;
        //FindObjectOfType<ThridPersonSword>().enabled = false;
    }
    void CloseShop()
    {
        GameManager.Instance.DisableCursor();
        UpdateShop();
        Time.timeScale = 1;
        shopCamera.SetActive(false);
        FindObjectOfType<PlayerWeaponHandle>().enabled = true;
        //FindObjectOfType<ThirdPersonShooting>().enabled = shotingEnabled;
        //FindObjectOfType<ThridPersonSword>().enabled = swordEnabled;
    }
    public void UpdateShop()
    {
        WeaponButton[] buttons = FindObjectsOfType<WeaponButton>();
        foreach(WeaponButton b in buttons)
        {
            b.UpdateDisplay();
        }

        ZombieButton[] zbuttons = FindObjectsOfType<ZombieButton>();
        foreach (ZombieButton z in zbuttons)
        {
            z.UpdateDisplay();
        }

        currHealthText.text = GameManager.Instance.GetPlayerHp().ToString();

    }
}
