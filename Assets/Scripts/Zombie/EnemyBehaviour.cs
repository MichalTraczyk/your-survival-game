using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public enum ZombieState
{
    Agonizing,
    Wandering,
    Running,
    Attacking
}
public class EnemyBehaviour : MonoBehaviour, IDamagable
{
    //Asignables
    [SerializeField] EnemyBehaviourSO behaviour;
    private NavMeshAgent controller;
    private Animator animator;


    private Transform player;
    private Transform target;
    private bool trigered;
    private bool attacking;
    private bool dead;
    private bool canAttack = true;
    private bool onRagdoll;
    private float targetUpdateTime;
    private float targetUpdateCurrentTime;
    private bool visible;
    private int currentHp;
    //distance to player
    private float distance;


    [SerializeField] Slider hpSlider;
    //Current zombie state - what is he doing
    public ZombieState state { get; private set;}

    [Header("Particle systems")]
    [SerializeField] private ParticleSystem bloodPartcieles;
    [SerializeField] private ParticleSystem ExplosionParticles;
    [Header("Position from where ragdoll force is applied")]
    [SerializeField] private Transform forcePos;
    [SerializeField] private Transform root;

    [Header("Audio clips")]
    [SerializeField] AudioSource attackAudio;


    private void Awake()
    {
        //Get components 
        animator = GetComponent<Animator>();
        controller = GetComponent<NavMeshAgent>();

        //Setup base stats
        behaviour = GameManager.Instance.getBehaviour();
        currentHp = GameManager.Instance.getHp(behaviour.baseHp);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        controller.speed = behaviour.speed;
    }
    // Start is called before the first frame update
    void Start()
    {

        if (hpSlider != null)
        {
            hpSlider.maxValue = currentHp;
            hpSlider.value = currentHp;
        }
        float vol = SoundManager.Instance.currentVolume;
        attackAudio.volume = vol;
        DisableRagdoll();

        animator.runtimeAnimatorController = behaviour.animator;
        if (behaviour.enemyBehaviour == Behaviour.Aggresive)
        {
            target = player;
            Trigger();
        }
        else if (behaviour.enemyBehaviour == Behaviour.Neutral)
        {
            state = ZombieState.Wandering;
        }
        else
        {
            state = ZombieState.Agonizing;
        }
    }
    void RandomChangeTarget()
    {
        int x = Random.Range(-15, 15);
        //int y = Random.Range(-15, 15);
        int z = Random.Range(-15, 15);
        if(controller.isOnNavMesh)
            controller.SetDestination(transform.position + new Vector3(x, 0, z));
    }    
    void CheckPlayerDistance()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        
        if (!trigered && distance < behaviour.distanceToTrigger)
            Trigger();
        
        if (target == null)
            return;

        //if invicible dont update often
        if(!visible)
        {
            targetUpdateTime = 2f;
            return;
        }


        //The closer zombie is more often update path to player
        if (distance < 10)
        {
            targetUpdateTime = 0.1f;
        }
        else if(distance < 20)
        {
            targetUpdateTime = 1f;
        }
        else
        {
            targetUpdateTime = 3f;
        }
    }

    public void Trigger()
    {
        state = ZombieState.Running;
        target = player;
        trigered = true;
    }
    #region attacking
    IEnumerator disableAttack(float secs)
    {
        canAttack = false;
        yield return new WaitForSeconds(secs);
        canAttack = true;
    }
    IEnumerator damagePlayer()
    {

        animator.SetTrigger("Attack");
        //Send sphere in forward
        attacking = true;
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(disableMovement(1.7f));
        //SpherePlayerDamage();
        float r = Random.Range(1f, 1.4f);
        yield return new WaitForSeconds(r);
        attacking = false;
    }

    //Invoke from animation
    public void SpherePlayerDamage()
    {
        attackAudio.Play();
        Collider[] coll = Physics.OverlapSphere(forcePos.position, 1.5f);
        foreach(Collider c in coll)
        {
            PlayerHP hp = c.GetComponent<PlayerHP>();
            if(hp!=null)
            {
                hp.Damage(behaviour.damage);
            }
        }
    }

    #endregion
    // Update is called once per frame
    void Update()
    {
        if (dead)
            return;

        CheckPlayerDistance();
        if(state == ZombieState.Running)
        {
            
            if (target != null && controller.enabled && trigered && controller.isOnNavMesh && targetUpdateTime < targetUpdateCurrentTime)
            {
                controller.SetDestination(target.position);
                targetUpdateCurrentTime = 0;
            }
        }
        else if(state == ZombieState.Wandering)
        {
            if(targetUpdateCurrentTime > 5)
            {
                RandomChangeTarget();
                targetUpdateCurrentTime = 0;
            }
        }
        targetUpdateCurrentTime += Time.deltaTime;
        AnimatorUpdate();
    }


    private void AnimatorUpdate()
    {
        animator.SetFloat("Speed", controller.velocity.magnitude);
    }
    IEnumerator disableMovement(float time)
    {
        controller.enabled = false;
        yield return new WaitForSeconds(time);
        controller.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(canAttack && other.CompareTag("Player") && !attacking)
        {
            StartCoroutine(damagePlayer());
        }
    }
    public void OnSwordHit()
    {
        disableAttack(3);
        StartCoroutine(RagdollOnHit());
    }
    public void DieOnBarrel(Vector3 barrelPos)
    {
        if (dead)
            return;

        Die(false);
        EnableRagdoll(barrelPos, true, 50000,true);
    }





    IEnumerator RagdollOnHit()
    {
        EnableRagdoll(forcePos.position,true);

        yield return new WaitForSeconds(2);

        DisableRagdoll();
    }
    void DisableRagdoll()
    {
        if (dead)
            return;

        Vector3 pos = root.position;
        root.localPosition = new Vector3(0.002f, 0.9f, -0.05f);
        root.localEulerAngles = new Vector3(0, -90, -90);
        transform.position = pos;

        animator.enabled = true;
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
        }
        controller.enabled = true;

    }
    public void EnableRagdoll(Vector3 forcePosP,bool addForce = false,float force = 20000,bool allRbsForce = false)
    {
        if (onRagdoll)
            return;
        onRagdoll = true;
        animator.enabled = false;
        controller.enabled = false;
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
            if(addForce && allRbsForce)
            {
                rb.AddExplosionForce(force, forcePos.position, 5);
            }
        }
        if(addForce && !allRbsForce)
        {
            rbs[0].AddExplosionForce(force, forcePos.position, 5);
            rbs[1].AddExplosionForce(force, forcePos.position, 5);
        }
    }



    IEnumerator DestroyOnDie()
    {
        yield return new WaitForSeconds(4);

        int r = Random.Range(0, 1);
        if (r == 0)
        {
            ExplosionParticles.transform.parent = null;
            ExplosionParticles.transform.rotation = Quaternion.identity;
            ExplosionParticles.Play();
        }
        Destroy(this.gameObject);
    }
    public void Die(bool playAnimation = true)
    {
        GameManager.Instance.OnZombieKilled();
        bloodPartcieles.Play();
        dead = true;
        StartCoroutine(DestroyOnDie());
        controller.enabled = false;

        if (playAnimation)
        {
            int r = Random.Range(0, 3);
            if(r <2)
            {
                EnableRagdoll(forcePos.position,true);
            }
            else
            {
                animator.SetTrigger("Die");
            }
        }
        EnemyCollider[] ec = GetComponentsInChildren<EnemyCollider>();
        foreach(EnemyCollider e in ec)
        {
            Destroy(e);
        }
    }
    public void Damage(int dmg, WeaponType weaponType = WeaponType.Rifle)
    {
        if (weaponType == WeaponType.Melee)
        {
            OnSwordHit();
        }
        if (!trigered)
            Trigger();

        currentHp -= dmg;

        if (currentHp <= 0)
        {
            Die();
        }
        if (hpSlider != null)
            hpSlider.value = currentHp;

    }

    private void OnBecameVisible()
    {
        visible = true;
        if(!dead)
        {
            animator.enabled = true;
        }
    }
    private void OnBecameInvisible()
    {
        visible = false;
        if (!dead)
        {
            animator.enabled = false;
        }
    }




}
