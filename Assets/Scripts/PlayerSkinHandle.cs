using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinHandle : MonoBehaviour
{
    public GameObject[] allSkins;

    private void Start()
    {
        int skin = PlayerPrefs.GetInt("Skin");
        
        foreach(GameObject g in allSkins)
        {
            g.SetActive(false);
        }
        allSkins[skin].SetActive(true);
    }
}
