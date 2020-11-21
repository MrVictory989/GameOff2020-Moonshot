using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Enemy
{
    public float patrolRange = 50;

    public Transform[] enemySpawnPoints;

    Vector3 target;

    private void Start()
    {
        Init();
        //GameManager.instance.spawners.Add(this);
        startedPos = transform.position;
        target = GetPatrolPos();
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, target) < 1f)
        {
            target = GetPatrolPos();
        }
        LookAt(target);
        Move();
    }

    void Move()
    {
        rb.velocity = transform.forward * moveSpeed;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    Vector3 GetPatrolPos()
    {
        Vector2 random = Random.insideUnitCircle * patrolRange;
        return new Vector3(startedPos.x + random.x, 0, startedPos.z + random.y);
    }

    public Vector3 GetSpawnPoint()
    {
        return enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)].position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }
}
