using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : Enemy
{
    enum State
    {
        following,
        attacking,
    }
    State state = State.following;

    //public GameObject bulletPrefab;
    public Transform shootPoint;

    public float shootRange = 10f;

    //public float bulletSpeed = 10;
    public int damage = 50;
    public float fireRate = 2f;
    float fireRateTimer;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        LookAtPlayer();
        switch (state)
        {
            case State.following:
                MoveToPlayer();

                if(Vector3.Distance(transform.position, player.transform.position) <= shootRange)
                {
                    state = State.attacking;
                }

                break;
            case State.attacking:
                rb.velocity = Vector3.zero;
                Shoot();
                fireRateTimer -= Time.deltaTime;

                if(Vector3.Distance(transform.position, player.transform.position) > shootRange)
                {
                    state = State.following;
                }

                break;
        }
    }

    void Shoot()
    {
        if (fireRateTimer <= 0)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, GameManager.instance.GetBulletSpawnRotation(dir, 0));
            bullet.GetComponent<Bullet>().Shoot(bulletSpeed, damage, "Player");
            fireRateTimer = 1f / fireRate;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
