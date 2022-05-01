using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    //Asignables
    [SerializeField]
    public EnemyBehaviourSO behaviour;
    NavMeshAgent controller;
    Animator animator;

    Transform player;
    Transform target;
    bool trigered;
    bool attacking;
    bool dead;
    bool canAttack = true;
    bool onRagdoll;
    float targetUpdateTime;
    float targetUpdateCurrentTime;
    bool visible;

    public ParticleSystem bloodPartcieles;
    public ParticleSystem ExplosionParticles;
    [SerializeField]
    Transform forcePos;
    public Transform root;


    public AudioClip[] ConstAudioClips;
    public AudioSource constantAudio;
    public AudioSource attackAudio;
    // Start is called before the first frame update
    void Start()
    {

        //int a = Random.Range(0, ConstAudioClips.Length);
        //constantAudio.clip = ConstAudioClips[a];
        //float pitch = Random.Range(0.7f, 1.3f);
        //constantAudio.pitch = pitch;
        //constantAudio.Play();

        float vol = SoundManager.Instance.currentVolume;
        constantAudio.volume = vol;
        attackAudio.volume = vol;

        animator = GetComponent<Animator>();
        controller = GetComponent<NavMeshAgent>();
        DisableRagdoll();
        //Start settings
        player = GameObject.FindGameObjectWithTag("Player").transform;
        controller.speed = behaviour.speed;

        behaviour = GameManager.Instance.getBehaviour();
        GetComponent<Damagable>().Setup(GameManager.Instance.getHp(behaviour.baseHp));
        //GetComponent<Damagable>().startHp = GameManager.Instance.getHp(behaviour.baseHp);
       // GetComponent<Damagable>().currHp = GetComponent<Damagable>().startHp;


        float t = 3;
        if (behaviour.enemyBehaviour == Behaviour.Aggresive)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            Trigger();
        }
        else if(behaviour.enemyBehaviour == Behaviour.Neutral)
        {
            InvokeRepeating("RandomChangeTarget",1,5f);
        }
        else
        {
            animator.SetBool("Agonal", true);
            t = 5;
        }
        StartCoroutine(disableMovement(t));
        if (behaviour.speedType == speedType.Runner)
            animator.SetBool("Runner", true);
        StartCoroutine(updateTarget());

        RandomizeAnimations();
    }
    void RandomizeAnimations()
    {
        //IDLE animation randomize
        int r = Random.Range(0, 2);
        animator.SetInteger("IDLERand",r);

        //Walk animation randomize
        r = Random.Range(0, 3);
        animator.SetInteger("WalkRand", r);

        r = Random.Range(0, 2);
        animator.SetInteger("RunRand", r);

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
        if (target == null)
            return;
        float distance = Vector3.Distance(transform.position, player.position);
        if (!trigered && distance < behaviour.distanceToTrigger)
            Trigger();

        if(!visible)
        {
            targetUpdateTime = 2f;
            return;
        }
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
        //animator.SetTrigger("Trigger");
        target = player;
        trigered = true;
        CancelInvoke();
    }

    IEnumerator disableAttack(float secs)
    {
        canAttack = false;
        yield return new WaitForSeconds(secs);
        canAttack = true;
    }
    IEnumerator damagePlayer()
    {
        //Play animation
        int rat = Random.Range(0, 3);
        animator.SetInteger("AttackType", rat);

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
    // Update is called once per frame
    void Update()
    {
        if (dead)
            return;
        CheckPlayerDistance();
        AnimatorUpdate();
        targetUpdateCurrentTime += Time.deltaTime;
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
        animator.SetBool("SkipStart", true);
    }
    IEnumerator updateTarget()
    {
        while(true)
        {
            if(target!= null && controller.enabled &&  trigered && controller.isOnNavMesh && targetUpdateTime < targetUpdateCurrentTime)
            {
                controller.SetDestination(target.position);
                targetUpdateCurrentTime = 0;
            }
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(canAttack && other.CompareTag("Player") && !attacking)
        {
            StartCoroutine(damagePlayer());
        }
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
    public void OnSwordHit()
    {
        disableAttack(3);
        StartCoroutine(RagdollOnHit());
    }
    IEnumerator RagdollOnHit()
    {
        EnableRagdoll(forcePos.position,true);

        yield return new WaitForSeconds(2);

        DisableRagdoll();
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
    public void DieOnBarrel(Vector3 barrelPos)
    {
        if (dead)
            return;
        Die(false);
        EnableRagdoll(barrelPos, true, 50000,true);
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
        Destroy(GetComponent<Damagable>());
        EnemyCollider[] ec = GetComponentsInChildren<EnemyCollider>();
        foreach(EnemyCollider e in ec)
        {
            Destroy(e);
        }
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
