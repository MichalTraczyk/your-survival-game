using UnityEngine;

public enum WeaponType
{
    Rifle,
    Melee,
    Grenade
}
public enum ShotType
{
    Auto,
    Semi
}
[CreateAssetMenu(fileName = "weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
public class Weapon : ScriptableObject
{
    public WeaponType weaponType;
    public int baseDamage;
    public GameObject mesh;
    public GameObject hitEnemyParticles;

    [Header("Rifle type settings")]
    public ShotType shotType;
    public float RPM;
    public float damageLossPer10Meters;
    public float recoil = 0.02f;
    public GameObject bulletPrefab;
    public bool isShoutgun;
    public int maxBullets;
    public int bulletsInMagazine;

    public float reloadTime = 2;
    public AudioClip shotClip;
    public AudioClip[] swordClips;
    public AudioClip[] swordClips_hit;
}