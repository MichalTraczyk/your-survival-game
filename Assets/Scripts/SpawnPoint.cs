using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    GameManager gm;
    private void Start()
    {
        gm = GetComponentInParent<GameManager>();
        gm.AddSpawnPoint(transform);
    }
    private void OnBecameVisible()
    {
        gm.RemoveSpawnPoint(transform);
    }
    private void OnBecameInvisible()
    {
        gm.AddSpawnPoint(transform);
    }
}
