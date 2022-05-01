using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WeaponButton : MonoBehaviour
{
    public Weapon weaponToBuy;
    public int healthPrice;

    public Button button;
    public TextMeshProUGUI price;
    private void Start()
    {
        button.onClick.AddListener(delegate {
            BuyWeapon();
        });
    }
    public void UpdateDisplay()
    {
        if(healthPrice >= GameManager.Instance.GetPlayerHp())
        {
            button.interactable = false;
            button.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            button.interactable = true;
            button.GetComponent<Image>().color = Color.white;
        }
        price.text = healthPrice.ToString();
    }
    void BuyWeapon()
    {
        GameManager.Instance.Damage(healthPrice);
        GameManager.Instance.OnWeaponBuy(weaponToBuy);
        GetComponentInParent<Shop>().UpdateShop();
    }
}
