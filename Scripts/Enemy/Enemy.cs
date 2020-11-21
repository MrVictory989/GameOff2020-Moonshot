using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    int currentHealth;

    public float moveSpeed = 5f;

    public float rotateSpeed = 5;

    public bool shouldSpawnEnemyWhenDied = true;

    public GameObject hitParticleEffect;

    public GameObject bulletPrefab;
    public float bulletSpeed = 20;
    public int bulletDamage = 1;

    public int killedReward = 50;

    //[HideInInspector]
    public Vector3 startedPos;

    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Rigidbody rb;

    public void Init()
    {
        currentHealth = maxHealth;
        player = GameManager.instance.player;
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(int amount, Vector3 hitPos)
    {
        if (amount > 1000 && !shouldSpawnEnemyWhenDied)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        //Instantiate(hitParticleEffect, hitPos, hitParticleEffect.transform.rotation);
        if(currentHealth <= 0)
        {
            if (amount > 1000)
                Die(true);
            else
                Die(false);
        }
    }

    public void LookAtPlayer()
    {
        if (player == null)
            return;

        Vector3 dir = (player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), rotateSpeed * Time.deltaTime);
    }

    public void LookAt(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), rotateSpeed * Time.deltaTime);
    }

    public void MoveToPlayer()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        rb.velocity = dir * moveSpeed;
    }

    void Die(bool asteroid)
    {
        GameManager.instance.player.GetComponent<Player>().AddMoney(killedReward);

        if(shouldSpawnEnemyWhenDied)
            GameManager.instance.SpawnEnemy();

        AudioManager.instance.Play("ExplosionSmall");
        Destroy(gameObject);
    }

    public void SetStartedPos(Vector3 pos)
    {
        startedPos = pos;
    }
}
