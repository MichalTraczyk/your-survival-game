using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using StarterAssets;
public enum EnemyDifficulity
{
    Easy,
    Normal,
    Hard
}
public class GameManager : MonoBehaviour
{
    public Volume damageVolume;
    public Volume onDamageVolume;

    public GameObject enemyPrefab;
    public EnemyBehaviourSO[] easyEnemies;
    public EnemyBehaviourSO[] normalEnemies;
    public EnemyBehaviourSO[] hardEnemies;


    List<EnemyBehaviourSO> allEnemies = new List<EnemyBehaviourSO>();
    int playerHp = 5;
    int currentWave = 1;
    public static GameManager Instance;
    GameObject PlayerGO;
    PlayerWeaponHandle weaponHandle;

    public List<Transform> validSpawnpoints = new List<Transform>();

    [SerializeField]
    List<EnemyBehaviourSO> enemiesQueue = new List<EnemyBehaviourSO>();

    float time = 0;
    int spawnedEnemies = 0;
    bool lost;
    bool paused;
    public StarterAssetsInputs input;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        UIManager.Instance.UpdateHp(playerHp);

        foreach (EnemyBehaviourSO so in easyEnemies)
        {
            allEnemies.Add(so);
        }
        foreach (EnemyBehaviourSO so in normalEnemies)
        {
            allEnemies.Add(so);
        }
        foreach (EnemyBehaviourSO so in hardEnemies)
        {
            allEnemies.Add(so);
        }

        PlayerGO = GameObject.FindGameObjectWithTag("Player");
        weaponHandle = PlayerGO.GetComponent<PlayerWeaponHandle>();
        GenerateEnemiesList(1);
        UpdateZombiesCount();
        InvokeRepeating("CheckForStuckZombies", 30, 30);
        Time.timeScale = 1;
        DisableCursor();
    }
    public int GetPlayerHp()
    {
        return playerHp;
    }
    void UpdateHp()
    {
        float w = Mathf.Clamp01(1 - playerHp / 3);
        damageVolume.weight = w;
        UIManager.Instance.UpdateHp(playerHp);
    }
    void Die()
    {
        SoundManager.Instance.PlaySound("PlayerDie");
        PlayerGO.GetComponent<PlayerHP>().EnableRagdoll();
        if(PlayerPrefs.GetInt("Highscore")<currentWave)
        {
            PlayerPrefs.SetInt("Highscore", currentWave);
        }
        lost = true;
        EnemyBehaviour[] zombies = FindObjectsOfType<EnemyBehaviour>();
        foreach (EnemyBehaviour eb in zombies)
        {
            if(eb.GetComponent<Damagable>() != null)
                eb.GetComponent<Damagable>().Damage(420);

            AudioSource[] s = eb.GetComponents<AudioSource>();
            foreach (AudioSource a in s)
            {
                Destroy(a);
            }
        }
        StartCoroutine(die());
    }
    IEnumerator die()
    {
        yield return new WaitForSeconds(2);
        EnableCursor();
        Time.timeScale = 0;
        Destroy(SlowMotionManager.Instance);
        UIManager.Instance.ShowEndUI(currentWave);
    }
    public void Damage(int dmg)
    {
        StartCoroutine(damageAnimation());
        playerHp -= dmg;
        UpdateHp();
        if (playerHp <= 0)
            Die();
    }
    IEnumerator damageAnimation()
    {
        float w = 0;
        while(w < 0.9)
        {
            w = Mathf.Lerp(w, 1, Time.deltaTime * 30);
            onDamageVolume.weight = w;
            yield return new WaitForEndOfFrame();
        }
        w = 1;
        while (w > 0)
        {
            w = Mathf.Lerp(w, 0, Time.deltaTime * 30);
            onDamageVolume.weight = w;
            yield return new WaitForEndOfFrame();
        }
    }
    public void PauseClicked()
    {
        if(paused)
        {
            Unpause();
        }
        else
        {
            Pause();
        }
    }
    public void Pause()
    {
        EnableCursor();
        paused = true;
        Time.timeScale = 0;
        SlowMotionManager.Instance.enabled = false;
        UIManager.Instance.OpenPauseUI();
    }
    public void Unpause()
    {
        DisableCursor();
        paused = false;
        Time.timeScale = 1;
        SlowMotionManager.Instance.enabled = true;
        UIManager.Instance.ClosePauseUI();
    }
    private void Update()
    {
        time += Time.deltaTime;
        if(time> 5)
        {
            if(enemiesQueue.Count> 0)
            {
                if(enemiesQueue.Count >= 5)
                {
                    SpawnZombies(5);
                }
                else
                {
                    SpawnZombies(enemiesQueue.Count);
                }
            }
            time = 0;
            UpdateZombiesCount();
            CheckForNextWave();
        }
    }
    void CheckForStuckZombies()
    {
        if(spawnedEnemies < 3 && enemiesQueue.Count == 0)
        {
            EnemyBehaviour[] zombies = FindObjectsOfType<EnemyBehaviour>();
            foreach(EnemyBehaviour z in zombies)
            {
                z.GetComponent<Damagable>().Damage(2137);
            }
        }
    }
    void UpdateZombiesCount()
    {
        UIManager.Instance.UpdateWavesUI(currentWave, spawnedEnemies);
        UpdateZombiesSound();
    }
    void UpdateZombiesSound()
    {
        int zombiesSounds = spawnedEnemies / 5;
        SoundManager.Instance.UpdateZombiesAudio(zombiesSounds);
    }
    public void AddZombiesToQueue(int number,EnemyDifficulity diff)
    {
        for(int i = 0;i<number;i++)
        {
            int r = 0;
            switch (diff)
            {
                case EnemyDifficulity.Easy:
                    r = Random.Range(0, easyEnemies.Length);
                    enemiesQueue.Add(easyEnemies[r]);
                    break;
                case EnemyDifficulity.Normal:
                    r = Random.Range(0, normalEnemies.Length);
                    enemiesQueue.Add(normalEnemies[r]);

                    break;
                case EnemyDifficulity.Hard:
                    r = Random.Range(0, hardEnemies.Length);
                    enemiesQueue.Add(hardEnemies[r]);

                    break;;
            }
        }
    }
    void SpawnZombies(int number)
    {
        for(int i = 0;i < number;i++)
        {
            if(enemiesQueue.Count >0 && enemiesQueue[0] != null)
            {
                int spawnPointIndex = Random.Range(0, validSpawnpoints.Count);
                Vector3 pos = validSpawnpoints[spawnPointIndex].position;
                pos += new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f));
                GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
                spawnedEnemies++;
            }
        }
    }
    public EnemyBehaviourSO getBehaviour()
    {
        EnemyBehaviourSO so = enemiesQueue[0];
        enemiesQueue.RemoveAt(0);
       
        return so;
    }
    public void AddSpawnPoint(Transform p)
    {
        if(!validSpawnpoints.Contains(p))
        {
            validSpawnpoints.Add(p);
        }
    }
    public void RemoveSpawnPoint(Transform p)
    {
        if (validSpawnpoints.Contains(p))
        {
            validSpawnpoints.Remove(p);
        }
    }

    void GenerateEnemiesList(int wave)
    {
        float zombiesCount = 0.000058f * wave * wave * wave + 0.074032f * wave * wave + 0.718119f * wave + 14.738699f;

        int c = Mathf.RoundToInt(zombiesCount);
        for(int i = 0;i < c;i++)
        {
            int r = Random.Range(0, allEnemies.Count);
            enemiesQueue.Add(allEnemies[r]);
        }
    }
    public void AddHp(int amount)
    {
        playerHp += amount;
        UpdateHp();
    }
    public void OnZombieKilled()
    {
        spawnedEnemies--;
        CheckForNextWave();
        Invoke("UpdateZombiesCount", 0.1f);
    }
    void CheckForNextWave()
    {
        if (spawnedEnemies <=1 && enemiesQueue.Count == 0)
        {
            NextWave();
        }
    }
    void NextWave()
    {
        if (lost)
            return;

        playerHp++;
        UpdateHp();
        currentWave++;
        GenerateEnemiesList(currentWave);
    }
    public void OnWeaponBuy(Weapon w)
    {
        weaponHandle.AddWeapon(w);
    }

    public int getHp(int baseHp)
    {
        int hp = baseHp;

        hp += currentWave * 100;

        if(currentWave > 9)
        {
            for(int i = 9; i<=currentWave;i++)
            {
                hp =  Mathf.RoundToInt(hp * 1.1f);
            }
        }


        return hp;
    }
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    public void ResetScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
}
