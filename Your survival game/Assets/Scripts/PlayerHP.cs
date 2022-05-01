using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class PlayerHP : MonoBehaviour
{
    Animator animator;
    public Transform root;
    public Transform lookAtCam;
    public MonoBehaviour[] scriptsToDisable;
    public void Damage(int dmg)
    {
        GameManager.Instance.Damage(dmg);
    }
    private void Start()
    {
        animator = GetComponent<Animator>();

        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
        }
    }
    public void EnableRagdoll()
    {
        foreach(MonoBehaviour b in scriptsToDisable)
        {
            Destroy(b);
        }
        lookAtCam.parent = root;
        GetComponent<ThirdPersonController>().canMove = false;
        animator.enabled = false;
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
        }
    }
}
