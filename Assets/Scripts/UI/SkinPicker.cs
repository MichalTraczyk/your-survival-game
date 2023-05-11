using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkinPicker : MonoBehaviour
{
    public Skin[] allSkins;
    int currSkinIndex;
    public GameObject lockedPanel;
    public TextMeshProUGUI TargetWavesText;
    // Start is called before the first frame update
    void Start()
    {
        currSkinIndex = PlayerPrefs.GetInt("Skin");
        EnableSkin(currSkinIndex);
    }

    public void AddIndex()
    {
        currSkinIndex++;
        if(currSkinIndex >= allSkins.Length)
        {
            currSkinIndex = 0;
        }
        UpdateSkin();
    }
    public void RemoveIndex()
    {
        currSkinIndex--;
        if (currSkinIndex <0)
        {
            currSkinIndex = allSkins.Length - 1;
        }
        UpdateSkin();
    }
    void UpdateSkin()
    {
        EnableSkin(currSkinIndex);
        Skin s = allSkins[currSkinIndex];
        if(s.targetWaves <= PlayerPrefs.GetInt("Highscore"))
        {
            PlayerPrefs.SetInt("Skin",currSkinIndex);
            lockedPanel.SetActive(false);
        }
        else
        {
            lockedPanel.SetActive(true);
            TargetWavesText.text = "Survive " + s.targetWaves.ToString() + " waves to unlock!";
        }
    }
    void EnableSkin(int i)
    {
        foreach (Skin g in allSkins)
        {
            g.gameObject.SetActive(false);
        }
        allSkins[i].gameObject.SetActive(true);
    }

}
