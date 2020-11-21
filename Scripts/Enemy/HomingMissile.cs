using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour, IDamageable
{
    public int maxHealth = 3;
    int currentHealth;

    public float rotateSpeed = 10;
    public float moveSpeed = 10;
    public int damage = 1;

    public GameObject explosionPrefab;

    Rigidbody rb;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 dir = (GameManager.instance.player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), rotateSpeed * Time.deltaTime);

        rb.velocity = transform.forward * moveSpeed;
    }

    public void TakeDamage(int amount, Vector3 pos)
    {
        currentHealth--;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.collider.GetComponentInParent<Player>();
        if(player != null)
        {
            player.TakeDamage(damage, collision.GetContact(0).point);
            Die();
        }
    }
}
