using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using StarterAssets;
public class ThirdPersonShooting : MonoBehaviour
{
    [Header("Assignables")]
    public CinemachineVirtualCamera normalCamera;
    public CinemachineVirtualCamera aimCamera;

    public Transform mesh;
    private Transform cam;

    public LayerMask mouseLayers;


    [Header("Options")]
    public float normalSensitivity = 1;
    public float aimSensitivity = 0.6f;


    private float shotTime;

    //Access from weapon handle script to set current weapon
    [HideInInspector]
    public Transform firePos;
    [HideInInspector]
    public Weapon w;

    bool aiming;
    bool reloading;
    Vector3 mouseWorldPos;


    [Header("IK")]
    public float lerpSpeed = 20;

    public Transform SpineTargetPos;
    public Transform RightHandPos;
    public Transform LeftHandTargetPos;

    public Rig aimRig;
    public Rig LeftHandRig;


    //references
    private StarterAssetsInputs input;
    private Animator animator;
    private ThirdPersonController controller;
    private PlayerWeaponHandle weaponHandle;
    private PlayerRotateMesh rotatePlayer;
    private CinemachineImpulseSource impulse;

    [Header("Audio")]
    public AudioSource shotSource;
    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        weaponHandle = GetComponent<PlayerWeaponHandle>();
        rotatePlayer = GetComponent<PlayerRotateMesh>();
        impulse = GetComponent<CinemachineImpulseSource>();
    }
    private void Start()
    {
        cam = Camera.main.transform;
        UIManager.Instance.ChangeCrosshairState(input.aim);
    }
    void Update()
    {
        if (!weaponHandle.enabled)
            return;

        Input();
        IKHandle();
        Aiming();

        shotTime += Time.deltaTime;
    }
    void Input()
    {
        aiming = input.aim;
        if (input.reload)
        {
            input.reload = false;
            StartCoroutine(Reload());
        }
        if (input.shot)
            Fire();
    }
    void IKHandle()
    {
        Vector2 shotPos = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(shotPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, mouseLayers))
        {
            mouseWorldPos = hit.point;
            SpineTargetPos.position = Vector3.Lerp(SpineTargetPos.position, cam.position + cam.forward * 10, Time.deltaTime * lerpSpeed / 3);
            RightHandPos.position = SpineTargetPos.position;
        }

        LeftHandTargetPos.position = weaponHandle.currentWeapon.mesh.GetComponent<WeaponPosition>().IKPos.position;
    }
    void Aiming()
    {
        if (aiming != input.aim)
        {
            UIManager.Instance.ChangeCrosshairState(input.aim);
        }

        if (aiming && !reloading)
        {
            
            input.sprint = false;

            aimCamera.gameObject.SetActive(true);
            normalCamera.gameObject.SetActive(false);
            controller.mouseSensitivity = aimSensitivity;

            //Handle player rotation
            controller.canRotate = false;
            Vector3 worldAimTarget = mouseWorldPos;
            worldAimTarget.y = transform.position.y;

            Vector3 aimTarget = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimTarget, Time.deltaTime * lerpSpeed);

            rotatePlayer.rotation = 50;


            //Enable proper animator layer
            float targetWeight = Mathf.Lerp(animator.GetLayerWeight(1), 1, Time.deltaTime * lerpSpeed);
            animator.SetLayerWeight(1, targetWeight);
            aimRig.weight = Mathf.Lerp(aimRig.weight, 1, Time.deltaTime * lerpSpeed);

        }
        else
        {
            aimCamera.gameObject.SetActive(false);
            normalCamera.gameObject.SetActive(true);
            controller.mouseSensitivity = normalSensitivity;
            controller.canRotate = true;
            rotatePlayer.rotation = 0;

            float targetWeight = Mathf.Lerp(animator.GetLayerWeight(1), 0, Time.deltaTime * lerpSpeed);
            animator.SetLayerWeight(1, targetWeight);
            aimRig.weight = Mathf.Lerp(aimRig.weight, 0, Time.deltaTime * lerpSpeed);
        }
    }
    void GenerateShot()
    {
        //StartCoroutine(raycastsShot(hit));

        Vector3 aimDir = (mouseWorldPos - firePos.position).normalized + GenerateRecoil()/7;
    
        GameObject bullet = Instantiate(weaponHandle.currentWeapon.weapon.bulletPrefab, firePos.position, Quaternion.LookRotation(aimDir));
        Bullet b = bullet.GetComponent<Bullet>();

        b.Setup(weaponHandle.currentWeapon.weapon.baseDamage, weaponHandle.currentWeapon.weapon.damageLossPer10Meters, transform.position);
        b.hitEnemyParticles = weaponHandle.currentWeapon.weapon.hitEnemyParticles;
    }
    void Fire()
    {
        if (input.shot && !aiming)
            aiming = true;
        if (reloading)
            return;

        if (weaponHandle.currentWeapon.ammoInMagazine <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (shotTime > 60 / weaponHandle.currentWeapon.weapon.RPM)
        {
            //SoundManager.Instance.PlaySound("PlayerDie");
            PlayShotSound();
            weaponHandle.currentWeapon.ammoInMagazine--;
            GenerateShot();
            

            
            weaponHandle.wp.muzzleFlash.Play();
            impulse.GenerateImpulse();
            shotTime = 0;
            switch (weaponHandle.currentWeapon.weapon.shotType)
            {
                case ShotType.Auto:
                    break;
                case ShotType.Semi:
                    input.shot = false;
                    break;
                default:
                    break;
            }
            UpdateAmmoUI();
        }
    }
    void PlayShotSound()
    {
        float rand = Random.Range(0.7f, 1.3f);
        shotSource.pitch = rand;
        shotSource.PlayOneShot(weaponHandle.currentWeapon.weapon.shotClip);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(SpineTargetPos.position,0.2f);
    }
    IEnumerator Reload()
    {
        //if already reloading
        if (reloading)
            yield break;
        if (weaponHandle.currentWeapon.ammoOffMagazine == 0)
            yield break;
        //play animation
        LeftHandRig.weight = 0;
        SoundManager.Instance.PlaySound("Reload");
        animator.SetTrigger("Reload");
        animator.SetLayerWeight(1, 0);
        aiming = false;
        reloading = true;

        yield return new WaitForSeconds(weaponHandle.currentWeapon.weapon.reloadTime);

        reloading = false;
        int missingAmmo = weaponHandle.currentWeapon.weapon.bulletsInMagazine - weaponHandle.currentWeapon.ammoInMagazine;
        if (missingAmmo <= weaponHandle.currentWeapon.ammoOffMagazine)
        {
            weaponHandle.currentWeapon.ammoInMagazine += missingAmmo;
            weaponHandle.currentWeapon.ammoOffMagazine -= missingAmmo;
        }
        else
        {
            weaponHandle.currentWeapon.ammoInMagazine += weaponHandle.currentWeapon.ammoOffMagazine;
            weaponHandle.currentWeapon.ammoOffMagazine = 0;
        }
        UpdateAmmoUI();
        LeftHandRig.weight = 1;
    }    
    void UpdateAmmoUI()
    {
        UIManager.Instance.UpdateAmmo(weaponHandle.currentWeapon.ammoInMagazine, weaponHandle.currentWeapon.ammoOffMagazine);
    }
    void OnWeaponSwap()
    {
        StopAllCoroutines();
        reloading = false;
    }
    private void OnDisable()
    {
        if(aimCamera != null)
            aimCamera.gameObject.SetActive(false);
        if(normalCamera != null)
            normalCamera.gameObject.SetActive(true);


        controller.mouseSensitivity = normalSensitivity;
        controller.canRotate = true;
        rotatePlayer.rotation = 0;
        animator.SetLayerWeight(1, 0);
        aimRig.weight = 0;
        LeftHandRig.weight = 0;

        OnWeaponSwap();
    }
    private void OnEnable()
    {
        Debug.Log("teraz!");
        rotatePlayer.speed = 20;
        LeftHandRig.weight = 1;
    }
    Vector3 GenerateRecoil()
    {
        Vector3 recoil = Vector3.zero;
        float r = weaponHandle.currentWeapon.weapon.recoil;
        recoil.x = Random.Range(-r, r);
        recoil.y = Random.Range(-r, r);

        return recoil;

    }

}
