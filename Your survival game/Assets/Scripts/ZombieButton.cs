using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ZombieButton : MonoBehaviour
{
    public int healthGive;

    public Button button;
    public TextMeshProUGUI price;
    public EnemyDifficulity difficulity;
    public int count;
    private void Start()
    {
        button.onClick.AddListener(delegate {
            Buy();
         });
    }
    public void UpdateDisplay()
    {
        //button.onClick.AddListener(delegate {
        //    Buy();
       // });
        price.text = healthGive.ToString();

    }
    void Buy()
    {
        Debug.Log("clicked");
        GameManager.Instance.AddHp(healthGive);
        GameManager.Instance.AddZombiesToQueue(count, difficulity);
        GetComponentInParent<Shop>().UpdateShop();
    }

}
