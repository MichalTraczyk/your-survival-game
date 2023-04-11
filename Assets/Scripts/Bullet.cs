using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10;
    int baseDamage;
    float damageLoss;
    Vector3 startPos;
    public Transform spherePos;

    private Rigidbody rb;
   // Vector3 targetPos;
    public GameObject hitWallParticles;
    public GameObject hitEnemyParticles;
    public LayerMask enemyLayers;
    public void Setup(int dmg,float dmgLoss,Vector3 startPosition)
    {
        baseDamage = dmg;
        damageLoss = dmgLoss;
        startPos = startPosition;
        rb = GetComponent<Rigidbody>();
        //targetPos = targetPosition;
    }
    private void Update()
    {
        bulletMove();
    }


    //when we collide with enemy
    private void OnTriggerEnter(Collider other)
    {
        GameObject particles = hitWallParticles;
        Damagable damagable = other.gameObject.GetComponent<Damagable>();
        EnemyCollider ec = other.gameObject.GetComponent<EnemyCollider>();
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddExplosionForce(600, transform.position, 0.3f);


        if (ec != null)
        {
            particles = hitEnemyParticles;
            ec.Damage(calculateDamage());
        }

        else if (damagable != null)
        {
            damagable.Damage(calculateDamage());
            particles = hitEnemyParticles;
        }

        Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
    void bulletMove()
    {
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        //transform.position += transform.forward * speed * Time.deltaTime;
    }
    int calculateDamage()
    {
        float dmg = baseDamage;
        float dist = Vector3.Distance(startPos,transform.position);
        dmg -= dist * damageLoss / 10;
        dmg = Mathf.Clamp(dmg, 0, 100);
        return Mathf.RoundToInt(dmg);
    }
}
