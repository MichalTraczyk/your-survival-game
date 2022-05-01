using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalkToPlayer : MonoBehaviour
{
    NavMeshAgent controller;
    Transform target;

    public int distanceToTrigger;

    bool trigered;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<NavMeshAgent>();
        SetPlayerAsTarget();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < distanceToTrigger)
            Trigger();
        if (trigered)
        {
            controller.SetDestination(target.position);
        }

    }
    public void SetPlayerAsTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void AgonalChangeTarget()
    {
        //
    }
    public void Trigger()
    {
        trigered = true;
    }
}
