using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArmor : Enemy
{
    enum State { Patrol, Attack};
    State state = State.Patrol;

    public float patrolRange = 50;
    public float attackRange = 10f;
    public float shootRange = 5f;

    //public GameObject bulletPrefab;
    public Transform[] shootPoints;

    public float fireRate = 1;
    float fireRateTimer;

    Vector3 target;

    Vector3 previousFramePos = Vector3.zero;
    float stuckTimer;

    private void Start()
    {
        Init();
        //startedPos = transform.position;
        target = GetPatrolPos();
    }

    private void Update()
    {
        switch (state)
        {
            case State.Patrol:
                if (Vector3.Distance(previousFramePos, transform.position) < 0.2f)
                {
                    stuckTimer += Time.deltaTime;
                    if (stuckTimer >= 1)
                    {
                        SetStartedPos(new Vector3(Random.Range(-300, 300), 0, Random.Range(-150, 150)));
                        GetPatrolPos();
                    }
                }
                else
                {
                    stuckTimer = 0;
                }

                if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
                {
                    state = State.Attack;
                }

                if (Vector3.Distance(transform.position, target) < 1f)
                {
                    target = GetPatrolPos();
                }
                LookAt(target);
                previousFramePos = transform.position;
                break;
            case State.Attack:
                target = player.transform.position;

                if(Vector3.Distance(transform.position, player.transform.position) < shootRange)
                {
                    if (fireRateTimer <= 0)
                    {
                        //Vector3 dir = (player.transform.position - shootPoint.position).normalized;
                        for (int j = 0; j < shootPoints.Length; j++)
                        {
                            for (int i = -2; i < 3; i++)
                            {
                                GameObject bullet = Instantiate(bulletPrefab, shootPoints[j].position, GameManager.instance.GetBulletSpawnRotation(transform.forward, (float)i / 30));
                                bullet.GetComponent<Bullet>().Shoot(bulletSpeed, bulletDamage, "Player");
                            }
                        }
                        fireRateTimer = 1f / fireRate;
                    }
                }
                LookAt(target);
                break;
        }
        fireRateTimer -= Time.deltaTime;
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

    void BossFightStarted()
    {
        /*state = State.Patrol;
        target = GetPatrolPos();*/
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, patrolRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
