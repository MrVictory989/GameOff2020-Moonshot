using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossPart : MonoBehaviour, IDamageable
{
    public Boss boss;
    public GameObject tentacle;
    public Transform accuratePosition;

    public List<Transform> shootPoints = new List<Transform>();
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public int damage;

    public GameObject missilePrefab;

    public int maxHealth;
    int currentHealth;
    public Image healthBarFill;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        healthBarFill.transform.localScale = new Vector3((float)currentHealth / maxHealth, 1, 1);
    }

    public void Shoot()
    {
        for (int i = 0; i < shootPoints.Count; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoints[i].position, GameManager.instance.GetBulletSpawnRotation(shootPoints[i].forward, 0));
            bullet.GetComponent<Bullet>().Shoot(bulletSpeed, damage, "Player");
        }
    }

    public void SpawnMissile()
    {
        Instantiate(missilePrefab, shootPoints[Random.Range(0, shootPoints.Count)].position, Quaternion.identity);
    }

    public void TakeDamage(int amount, Vector3 hitPos)
    {
        if (amount > 1000)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBarFill.transform.localScale = new Vector3((float)currentHealth / maxHealth, 1, 1);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        AudioManager.instance.Play("Explosion");
        boss.RemovePart(this);
        Destroy(tentacle);
        Destroy(gameObject);
    }
}
