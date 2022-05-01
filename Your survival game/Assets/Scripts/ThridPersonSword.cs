using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class ThridPersonSword : MonoBehaviour
{
    private PlayerWeaponHandle weaponHandle;
    private StarterAssetsInputs input;
    private Animator animator;
    private ThirdPersonController controller;
    public PlayerRotateMesh rotator;

    public Transform attackSpherePos;
    public float sphereRadius;
    bool damaged;

    bool attack;
    public AudioSource attackAudio;
    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        weaponHandle = GetComponent<PlayerWeaponHandle>();
        animator = GetComponent<Animator>();
        controller = GetComponent<ThirdPersonController>();
        rotator = GetComponent<PlayerRotateMesh>();
    }
    void PlayAttackAudio(bool hit = false)
    {
        if(!hit)
        {
            int r = Random.Range(0, weaponHandle.currentWeapon.weapon.swordClips.Length);
            attackAudio.clip = weaponHandle.currentWeapon.weapon.swordClips[r];
        }
        else
        {
            int r = Random.Range(0, weaponHandle.currentWeapon.weapon.swordClips_hit.Length);
            attackAudio.clip = weaponHandle.currentWeapon.weapon.swordClips_hit[r];
        }
        //int r = Random.Range(0, weaponHandle.currentWeapon.weapon.swordClips.Length);
        attackAudio.pitch = Random.Range(0.7f, 1f);
        attackAudio.Play();
    }
    private void Update()
    {
        if (!weaponHandle.enabled)
            return;

        if (input.shot && !attack)
        {
            //PlayAttackAudio();

            StartCoroutine(MoveForwardABit());
            controller.canMove = false;
            animator.SetBool("Attack", true);
            input.shot = false;
            attack = true;
        }

        if(!attack)
        {
            rotator.speed = 20;
            if (input.sprint)
            {
                rotator.rotation = 60;
            }
            else if(input.move != Vector2.zero)
            {
                rotator.rotation = 30;
            }
            else
            {
                rotator.rotation = 0;
            }    
        }
    }
    private void OnDisable()
    {
        EnableMove();
    }
    public void ResetAttack()
    {
        animator.SetBool("Attack", false);
        attack = false;
    }
    public void EnableMove()
    {
        //controller.hardMove = true;
        controller.canMove = true;
    }
    public void SetRotation(int rot)
    {
        rotator.speed = 70;
        rotator.rotation = rot;
    }
    public void AttackFromAnimation()
    {
        int count = 0;
        bool hit = false;
        Collider[] colliders = Physics.OverlapSphere(attackSpherePos.position, sphereRadius);
        foreach (Collider c in colliders)
        {
            Damagable d = c.GetComponent<Damagable>();
            if (d != null)
            {
                count++;
                d.Damage(20, WeaponType.Melee);
                hit = true;
            }
        }
        Debug.Log("hit: " + hit);
        PlayAttackAudio(hit);
        if (count > 3)
            SlowMotionManager.Instance.DoSlowmotion(0.2f, 3);
    }
    public IEnumerator MoveForwardABit()
    {
        yield return new WaitForSeconds(0.1f);
        controller.hardMove = true;
        yield return new WaitForSeconds(0.1f);
        controller.hardMove = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackSpherePos.position, sphereRadius);
    }
}
