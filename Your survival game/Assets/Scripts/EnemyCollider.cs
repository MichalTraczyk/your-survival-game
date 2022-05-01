using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    public float multiplier = 1;
    Damagable parentScript;
    private void Awake()
    {
        parentScript = GetComponentInParent<Damagable>();
    }
    public void Damage(int dmg)
    {
        parentScript.Damage(Mathf.RoundToInt(dmg * multiplier));
    }
}
