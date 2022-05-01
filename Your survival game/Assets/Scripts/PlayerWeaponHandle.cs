using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
[System.Serializable]
public class WeaponInEq
{
    public Weapon weapon;
    public GameObject mesh;
    public int ammoInMagazine;
    public int ammoOffMagazine;
}
public class PlayerWeaponHandle : MonoBehaviour
{
    //read variavbl
    public WeaponInEq currentWeapon;

    public List<WeaponInEq> allWeapons = new List<WeaponInEq>();
    public List<WeaponInEq> startWeapons = new List<WeaponInEq>();
    public Transform weaponPos;

    public WeaponPosition wp;

    StarterAssetsInputs input;
    ThirdPersonShooting shotScript;
    ThridPersonSword swordScript;
    Animator animator;

    int currWeaponIndex = 0;

    private void Start()
    {
        shotScript = GetComponent<ThirdPersonShooting>();
        swordScript = GetComponent<ThridPersonSword>();
        animator = GetComponent<Animator>();
        input = GetComponent<StarterAssetsInputs>();
        SetupWeaponsOnStart();
        ChangeWeapon(1);
        //AddWeapon(currentWeapon.weapon);
    }
    void SetupWeaponsOnStart()
    {
        foreach(WeaponInEq w in startWeapons)
        {
            AddWeapon(w.weapon);
        }
    }
    private void Update()
    {
        int scrollInput = 0;

        if (input.scroll < 0)
            scrollInput = -1;
        if (input.scroll > 0)
            scrollInput = 1;
        ChangeWeapon(scrollInput);
    }
    void ChangeWeapon(int i)
    {
        currWeaponIndex += i;
        if (currWeaponIndex >= allWeapons.Count)
            currWeaponIndex = 0;
        if (currWeaponIndex < 0)
            currWeaponIndex = allWeapons.Count - 1;
        if(currWeaponIndex != currWeaponIndex -i)
            SelectWeaponByIndex(currWeaponIndex);
    }
    void SelectWeaponByIndex(int index)
    {
        //if (allWeapons.Count < 1)
          //  return;
        currentWeapon.mesh.SetActive(false);

        currentWeapon = allWeapons[index];
        currentWeapon.mesh.SetActive(true);
        SetAnimatorLayers();
        UIManager.Instance.UpdateWeapon(currentWeapon.weapon.name, currentWeapon.ammoInMagazine, currentWeapon.ammoOffMagazine);
    }
    void SpawnNewWeapon()
    {
        GameObject go = Instantiate(currentWeapon.weapon.mesh, weaponPos);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        currentWeapon.mesh = go;
        currentWeapon.mesh.SetActive(true);
        //shotScript.firePos = go.GetComponent<WeaponPosition>().firePos;
    }
    void SetAnimatorLayers()
    {
        switch (currentWeapon.weapon.weaponType)
        {
            case WeaponType.Rifle:

                shotScript.enabled = true;
                swordScript.enabled = false;
                animator.SetLayerWeight(1, 1);
                animator.SetLayerWeight(2, 0);

                wp = currentWeapon.mesh.GetComponent<WeaponPosition>();
                shotScript.firePos = wp.firePos;
                break;
            case WeaponType.Melee:
                animator.SetLayerWeight(2, 1);
                animator.SetLayerWeight(1, 0);

                shotScript.enabled = false;
                swordScript.enabled = true;
                Debug.Log(shotScript.enabled);
                break;
            case WeaponType.Grenade:
                break;
            default:
                break;
        }
    }
    public void AddWeapon(Weapon w)
    {
        if(getWeaponByW(w) != null)
        {
            getWeaponByW(w).ammoOffMagazine = getWeaponByW(w).weapon.maxBullets;
        }
        else
        {
            if(currentWeapon.mesh != null)
            {
                currentWeapon.mesh.SetActive(false);
            }

            currentWeapon = new WeaponInEq();
            currentWeapon.weapon = w;
            currentWeapon.ammoInMagazine = w.bulletsInMagazine;
            currentWeapon.ammoOffMagazine = w.maxBullets - w.bulletsInMagazine;
            allWeapons.Add(currentWeapon);

            SpawnNewWeapon();
            SetAnimatorLayers();
        }
        UIManager.Instance.UpdateWeapon(currentWeapon.weapon.name, currentWeapon.ammoInMagazine, currentWeapon.ammoOffMagazine);
        ChangeWeapon(0);
    }
    WeaponInEq getWeaponByW(Weapon w)
    {
        foreach (WeaponInEq weq in allWeapons)
        {
            if (weq.weapon == w)
                return weq;
        }
        return null;
    }
}
