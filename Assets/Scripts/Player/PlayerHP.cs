using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class PlayerHP : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Transform root;
    [SerializeField] private Transform lookAtCam;
    [SerializeField] private MonoBehaviour[] scriptsToDisableOnRagdoll;
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
        foreach(MonoBehaviour b in scriptsToDisableOnRagdoll)
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
