using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MainMenu : MonoBehaviour
{
    public Animator animator;
    public GameObject mainMenuGO;
    public GameObject skinsGO;
    public Slider LoadingSlider;
    public TextMeshProUGUI highscoreText;
    private void Start()
    {
        highscoreText.text = PlayerPrefs.GetInt("Highscore").ToString();
    }
    public void SkinsMenuButtonClicked()
    {
        mainMenuGO.SetActive(false);
        skinsGO.SetActive(true);
        animator.SetBool("Skins", true);
    }
    public void BackButtonClicked()
    {
        mainMenuGO.SetActive(true);
        skinsGO.SetActive(false);
        animator.SetBool("Skins", false);
    }
    public void StartButtonClicked()
    {
        StartCoroutine(loadScene(1));
    }
    public void OnExitButton()
    {
        Application.Quit(0);
    }
    IEnumerator loadScene(int scene)
    {
        GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(1);

        LoadingSlider.gameObject.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            LoadingSlider.value = asyncLoad.progress;
            yield return null;
        }
    }
}
