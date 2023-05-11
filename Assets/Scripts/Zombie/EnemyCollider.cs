using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollider : MonoBehaviour, IDamagable
{
    public float multiplier = 1;
    EnemyBehaviour parentScript;
    private void Awake()
    {
        parentScript = GetComponentInParent<EnemyBehaviour>();
    }
    public void Damage(int dmg, WeaponType weaponType = WeaponType.Rifle)
    {
        parentScript.Damage(Mathf.RoundToInt(dmg * multiplier),weaponType);
    }
}
