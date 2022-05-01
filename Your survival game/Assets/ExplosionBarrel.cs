using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class ExplosionBarrel : MonoBehaviour
{
    public float radius;
    public GameObject particles;
    int zombies = 0;
    CinemachineImpulseSource source;
    private void Start()
    {
        source = GetComponent<CinemachineImpulseSource>();
    }
    public void Explode()
    {
        source.GenerateImpulse();
        SoundManager.Instance.PlaySound("Explosion");
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider c in colliders)
        {
            EnemyBehaviour eb = c.GetComponent<EnemyBehaviour>();
            if (eb != null)
            { 
                eb.DieOnBarrel(transform.position);
                zombies++;
            }
            if(zombies>=3)
            {
                SlowMotionManager.Instance.DoSlowmotion(0.1f,3);
            }
        }
        Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
