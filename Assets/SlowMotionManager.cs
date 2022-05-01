using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SlowMotionManager : MonoBehaviour
{
    public static SlowMotionManager Instance;
    public float slowdownLenght = 2;
    public Volume slowMotionVolume;
    public float currTimescale;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    private void Update()
    {
        Time.timeScale += (1 / slowdownLenght)*Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp01(Time.timeScale);
        slowMotionVolume.weight = 1 - Time.timeScale;
        currTimescale = Time.timeScale;

    }
    public void DoSlowmotion(float slowdown, float lenght = 2)
    {
        Debug.Log("teraz");
        Time.timeScale = slowdown;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        slowdownLenght = lenght;
    }
}
