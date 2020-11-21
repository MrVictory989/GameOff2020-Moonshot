using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public GameObject bulletHit;

    string targetTag;
    float speed;
    int damage;
    Rigidbody rb;

    public void Shoot(float speed, int damage, string targetTag)
    {
        this.speed = speed;
        this.damage = damage;
        this.targetTag = targetTag;

        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 12f);
    }

    private void FixedUpdate()
    {
        if (rb != null)
            rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == targetTag)
        {
            IDamageable damageable = collision.collider.GetComponent<IDamageable>();
            if(damageable != null)
            {
                Instantiate(bulletHit, collision.GetContact(0).point, bulletHit.transform.rotation);
                damageable.TakeDamage(damage, collision.GetContact(0).point);
                Destroy(gameObject);
            }

            /*
            damageable = collision.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                Instantiate(bulletHit, collision.GetContact(0).point, bulletHit.transform.rotation);
                damageable.TakeDamage(damage, collision.GetContact(0).point);
                Destroy(gameObject);
            }*/
        }

        Asteroid asteroid = collision.collider.GetComponent<Asteroid>();
        if(asteroid != null)
        {
            Instantiate(bulletHit, collision.GetContact(0).point, bulletHit.transform.rotation);
            asteroid.TakeDamage(1, collision.GetContact(0).point);
        }

        //Debug.Log(collision.collider.name);
        Instantiate(bulletHit, collision.GetContact(0).point, bulletHit.transform.rotation);
        Destroy(gameObject);
    }
}
