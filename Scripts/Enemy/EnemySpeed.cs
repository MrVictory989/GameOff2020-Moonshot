using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpeed : Enemy
{
    enum State { Patrol, Attacking, Waiting,};
    State state = State.Patrol;

    public float patrolRange = 5;
    public float attackRange = 10f;

    //public GameObject bulletPrefab;
    public Transform[] shootPoints;

    public float fireRate = 02f;
    float fireRateTimer = 0;

    //public float bulletSpeed = 20f;
    //public int bulletDamage = 1;

    public int burstAmount = 8;
    int bulletsShot;

    public float attackCooldown = 3;
    float attackCooldownTimer;

    Vector3 target;
    bool hasStartedAttacking = false;

    float stuckTimer;
    Vector3 previousFramePos;

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
                if(Vector3.Distance(previousFramePos, transform.position) < 0.2f)
                {
                    stuckTimer += Time.deltaTime;
                    if(stuckTimer >= 1)
                    {
                        SetStartedPos(new Vector3(Random.Range(-300, 300), 0, Random.Range(-150, 150)));
                        GetPatrolPos();
                    }
                }
                else
                {
                    stuckTimer = 0;
                }

                if(Vector3.Distance(transform.position, player.transform.position) < attackRange)
                {
                    state = State.Attacking;
                }

                if(Vector3.Distance(transform.position, target) < 1f)
                {
                    target = GetPatrolPos();
                }
                LookAt(target);
                previousFramePos = transform.position;
                break;
            case State.Attacking:
                target = player.transform.position;
                LookAt(target);

                if (Vector3.Distance(transform.position, player.transform.position) < attackRange || hasStartedAttacking)
                {
                    if (fireRateTimer <= 0)
                    {
                        for (int i = 0; i < shootPoints.Length; i++)
                        {
                            Vector3 dir = (player.transform.position - shootPoints[i].position).normalized;
                            GameObject bullet = Instantiate(bulletPrefab, shootPoints[i].position, GameManager.instance.GetBulletSpawnRotation(transform.forward, 0));
                            bullet.GetComponent<Bullet>().Shoot(bulletSpeed, bulletDamage, "Player");
                        }
                        fireRateTimer = 1f / fireRate;
                        bulletsShot++;
                        hasStartedAttacking = true;
                        if (bulletsShot >= burstAmount)
                        {
                            Vector2 random = Random.insideUnitCircle * patrolRange;
                            target = new Vector3(transform.position.x + random.x, 0, transform.position.z + random.y);
                            attackCooldownTimer = attackCooldown;
                            bulletsShot = 0;
                            hasStartedAttacking = false;
                            state = State.Waiting;
                        }
                    }
                }
                break;
            case State.Waiting:
                LookAt(target);
                attackCooldownTimer -= Time.deltaTime;
                if(attackCooldownTimer <= 0)
                {
                    state = State.Attacking;
                }
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
