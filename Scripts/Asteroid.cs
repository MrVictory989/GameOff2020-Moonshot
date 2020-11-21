using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Asteroid : MonoBehaviour, IDamageable
{
    public GameObject destroyedEffect;

    float speed;
    Rigidbody rb;
    int hitTimes = 0;

    public void Shoot(float speed)
    {
        this.speed = speed;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(rb != null)
            rb.velocity = transform.forward * speed;
    }

    public void TakeDamage(int amount, Vector3 pos)
    {
        hitTimes++;
        if(hitTimes >= 3)
        {
            Debug.Log("Died because I have suffered 3 hits");
            Die();
        }
    }

    void Die()
    {
        Instantiate(destroyedEffect, transform.position, destroyedEffect.transform.rotation);
        GameManager.instance.SpawnAsteroid();
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag != "Bullet")
        {
            IDamageable damageable = collision.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(10000, collision.GetContact(0).point);
                Die();
                return;
            }

            damageable = collision.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(100000, collision.GetContact(0).point);
                Die();
                return;
            }

            Debug.Log($"Died because I collided with {collision.collider.name}");
            Die();
        }
    }
}
