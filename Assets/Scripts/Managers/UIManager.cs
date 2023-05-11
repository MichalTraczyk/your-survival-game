using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [Header("Main UI")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private GameObject crossHair;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI magazineAmmoText;
    [SerializeField] private TextMeshProUGUI offMagazineAmmoText;
    [Header("Waves UI")]
    [SerializeField] private GameObject wavesUI;
    [SerializeField] private TextMeshProUGUI currWave;
    [SerializeField] private TextMeshProUGUI enemiesLeft;

    [Header("DeathUI")]
    [SerializeField] private GameObject deathUI;
    [SerializeField] private TextMeshProUGUI score;

    [Header("PauseUI")]
    [SerializeField] private Slider soundEffectsSlider;
    [SerializeField] private GameObject pauseGO;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        soundEffectsSlider.value = PlayerPrefs.GetFloat("Volume");
    }
    public void UpdateINteractText(string t)
    {
        interactText.text = t;
    }
    public void ChangeCrosshairState(bool s)
    {
        crossHair.SetActive(s);
    }
    public void UpdateHp(int hp)
    {
        hpText.text = hp.ToString();
    }
    public void UpdateWeapon(string name,int currAmmo,int offMagAmmo)
    {
        weaponNameText.text = name;
        UpdateAmmo(currAmmo, offMagAmmo);
    }
    public void UpdateAmmo(int currAmmo, int offMagAmmo)
    {
        magazineAmmoText.text = currAmmo.ToString();
        offMagazineAmmoText.text = offMagAmmo.ToString();
    }
    public void UpdateWavesUI(int w, int enemies)
    {
        if (currWave.text != w.ToString())
        {
            animator.SetTrigger("WaveUpdate");
        }
        currWave.text = w.ToString();
        //enemiesLeft.text = enemies.ToString();
    }
    public void DamageUI()
    {
        animator.Play("Damage");
    }

    public void ShowEndUI(int wave)
    {
        animator.Play("GameOver");
        mainUI.SetActive(false);
        wavesUI.SetActive(false);
        deathUI.SetActive(true);
        score.text = wave.ToString();
    }

    public void ResumeButtonClicked()
    {
        GameManager.Instance.Unpause();
    }
    public void OpenPauseUI()
    {
        pauseGO.SetActive(true);
    }
    public void ClosePauseUI()
    {
        pauseGO.SetActive(false);
    }
    public void SliderValueChanged()
    {
        SoundManager.Instance.UpdateVolume(soundEffectsSlider.value);
    }
    public void MainMenuButtonClicked()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
    public void TryAgain()
    {
        GameManager.Instance.ResetScene();
    }

    /*
    public TextMeshProUGUI fpst;

    public float updateRateSeconds = 4.0F;

    int frameCount = 0;
    float dt = 0.0F;
    float fps = 0.0f;

    void Update()
    {
        frameCount++;
        dt += Time.unscaledDeltaTime;
        if (dt > 1.0 / updateRateSeconds)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0F / updateRateSeconds;
        }
        fpst.text = fps.ToString();
    }*/

}
