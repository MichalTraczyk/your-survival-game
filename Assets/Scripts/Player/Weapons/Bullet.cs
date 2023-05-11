using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private Transform spherePos;

    private int baseDamage;
    private float damageLoss;
    private Vector3 startPos;

    private Rigidbody rb;

    // Vector3 targetPos;
    [SerializeField] private GameObject hitWallParticles;
    [SerializeField] public GameObject hitEnemyParticles;
    [SerializeField] private LayerMask enemyLayers;
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
        IDamagable damagable = other.gameObject.GetComponent<IDamagable>();
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddExplosionForce(600, transform.position, 0.3f);

        if (damagable != null)
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
