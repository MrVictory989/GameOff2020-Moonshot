using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamageable
{
    public float moveSpeed = 15f;
    public float rotateSpeed = 5f;

    [HideInInspector] public int speedLevel = 1;
    [HideInInspector] public int fireRateLevel = 1;
    [HideInInspector] public int damageLevel = 1;

    public int speedUpgradeCost = 50;
    public int fireRateUpgradeCost = 50;
    public int damageUpgradeCost = 50;
    public int heartContainerCost = 100;
    public int weaponCost = 50;

    public int maxHealth = 3;
    [HideInInspector] public int currentHealth;

    public float dockRange = 10f;

    public GameObject bulletPrefab;
    public GameObject bigBulletPrefab;
    public float fireRate = 2;
    float fireRateTimer;
    public int maxAmmo = 50;
    int currentAmmo;

    public Transform[] octaShootPoints;
    public LineRenderer laserLineRenderer;

    public GunType gunType;
    public List<GameObject> gunMeshes = new List<GameObject>();

    float shootButtonPressedTime;
    public float laserTravelMultiplier = 3;

    public int currentMoney = 0;

    public GunInfo normalGunInfo;
    public GunInfo shotgunInfo;
    public GunInfo octaShooterInfo;
    public GunInfo laserInfo;
    public GunInfo bigCannonInfo;

    public GameObject hitEffect;

    Camera mainCam;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;
        currentHealth = maxHealth;
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        float speed = moveSpeed + speedLevel * 2;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        rb.velocity = transform.forward * speed * vertical;
        transform.Rotate(Vector3.up, horizontal * rotateSpeed * Time.deltaTime);
        //rb.velocity = new Vector3(horizontal, 0, vertical) * moveSpeed;

        //LookAtMouse();

        fireRateTimer -= Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            Shoot();
            shootButtonPressedTime += Time.deltaTime;
        }
        else
        {
            laserLineRenderer.positionCount = 0;
            shootButtonPressedTime = 0;
        }

        GameObject planet = GameManager.instance.GetClosestPlanet(transform.position, out float distance);
        if(distance < dockRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GameManager.instance.Dock();
                Debug.Log("In dock range");
            }
        }
        LookAtMouse();
    }

    public void Dock()
    {
        rb.velocity = Vector3.zero;
        currentAmmo = maxAmmo;
        currentHealth = maxHealth;
    }

    public void Undock()
    {
        currentAmmo = maxAmmo;
        currentHealth = maxHealth;
    }

    void Shoot()
    {
        currentAmmo++;
        switch (gunType)
        {
            case GunType.Normal:
                if (fireRateTimer <= 0 && currentAmmo > 0)
                {
                    Vector3 dir = (GetMouseWorldPos() - normalGunInfo.shootPoint.position).normalized;
                    dir.y = 0;
                    GameObject bullet = Instantiate(bulletPrefab, normalGunInfo.shootPoint.position, GameManager.instance.GetBulletSpawnRotation(dir, 0));
                    bullet.GetComponent<Bullet>().Shoot(20, normalGunInfo.damage[damageLevel - 1], "Enemy");

                    currentAmmo--;
                    fireRateTimer = 1f / normalGunInfo.fireRate[fireRateLevel - 1];
                    //StartCoroutine(CameraShake.instance.Shake(0.15f, 0.2f));
                    AudioManager.instance.Play("Shoot");
                }
                laserLineRenderer.positionCount = 0;
                break;
            case GunType.Shotgun:
                if (fireRateTimer <= 0 && currentAmmo > 0)
                {
                    Vector3 dir = (GetMouseWorldPos() - shotgunInfo.shootPoint.position).normalized;
                    for (int i = -2; i < 3; i++)
                    {
                        GameObject bullet = Instantiate(bulletPrefab, shotgunInfo.shootPoint.position, GameManager.instance.GetBulletSpawnRotation(dir, (float)i/20));
                        bullet.GetComponent<Bullet>().Shoot(20, shotgunInfo.damage[damageLevel-1], "Enemy");
                    }

                    currentAmmo--;
                    fireRateTimer = 1f / shotgunInfo.fireRate[fireRateLevel -1];
                    AudioManager.instance.Play("Shoot");
                }
                laserLineRenderer.positionCount = 0;
                break;
            case GunType.Laser:
                if (currentAmmo <= 0)
                {
                    laserLineRenderer.positionCount = 0;
                    return;
                }

                Vector3 direction = (GetMouseWorldPos() - laserInfo.shootPoint.position).normalized;
                if (Physics.Linecast(laserInfo.shootPoint.position, laserInfo.shootPoint.position + direction * shootButtonPressedTime * laserTravelMultiplier, out RaycastHit hit))
                {
                    laserLineRenderer.positionCount = 2;
                    laserLineRenderer.SetPosition(0, laserInfo.shootPoint.position);
                    laserLineRenderer.SetPosition(1, hit.point);
                    Debug.DrawLine(laserInfo.shootPoint.position, laserInfo.shootPoint.position + direction * shootButtonPressedTime * laserTravelMultiplier, Color.red);
                }
                else
                {
                    laserLineRenderer.positionCount = 2;
                    laserLineRenderer.SetPosition(0, laserInfo.shootPoint.position);
                    laserLineRenderer.SetPosition(1, laserInfo.shootPoint.position + direction * shootButtonPressedTime * laserTravelMultiplier);
                }

                if (fireRateTimer <= 0)
                {
                    currentAmmo--;
                    fireRateTimer = 1f / laserInfo.fireRate[fireRateLevel - 1];
                    if(hit.collider != null)
                    {
                        IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                        if (damageable != null)
                        {
                            Instantiate(hitEffect, hit.point, hitEffect.transform.rotation);
                            damageable.TakeDamage(laserInfo.damage[damageLevel - 1], hit.point);
                        }
                    }
                }

                break;
            case GunType.OctaShooter:
                if (fireRateTimer <= 0 && currentAmmo > 0)
                {
                    for (int i = 0; i < octaShootPoints.Length; i++)
                    {
                        Vector3 dir = octaShootPoints[i].forward;
                        GameObject bullet = Instantiate(bulletPrefab, octaShootPoints[i].position, GameManager.instance.GetBulletSpawnRotation(dir, 0));
                        bullet.GetComponent<Bullet>().Shoot(20, octaShooterInfo.damage[damageLevel - 1], "Enemy");
                    }

                    currentAmmo--;
                    fireRateTimer = 1f / octaShooterInfo.fireRate[fireRateLevel - 1];
                    AudioManager.instance.Play("Shoot");
                }
                break;
            case GunType.BigCannon:
                if (fireRateTimer <= 0 && currentAmmo > 0)
                {
                    Vector3 dir = (GetMouseWorldPos() - bigCannonInfo.shootPoint.position).normalized;
                    GameObject bullet = Instantiate(bigBulletPrefab, bigCannonInfo.shootPoint.position, GameManager.instance.GetBulletSpawnRotation(dir, 0));
                    bullet.GetComponent<Bullet>().Shoot(20, bigCannonInfo.damage[damageLevel - 1], "Enemy");

                    currentAmmo--;
                    fireRateTimer = 1f / bigCannonInfo.fireRate[fireRateLevel - 1];
                    AudioManager.instance.Play("Shoot");
                }
                break;
        }
    }

    public void TakeDamage(int amount, Vector3 hitPos)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        UIManager.instance.SetGameOverMenuActive(true);
        Destroy(gameObject);
        Debug.Log("I died");
    }

    public void UpgradeSpeed()
    {
        if (currentMoney >= speedUpgradeCost && speedLevel < 5)
        {
            speedLevel++;
            speedLevel = Mathf.Clamp(speedLevel, 1, 5);
            currentMoney -= speedUpgradeCost;
        }
    }

    public void UpgradeFireRate()
    {
        if (currentMoney >= fireRateUpgradeCost && fireRateLevel < 5)
        {
            fireRateLevel++;
            fireRateLevel = Mathf.Clamp(fireRateLevel, 1, 5);
            currentMoney -= fireRateUpgradeCost;
        }
    }

    public void UpgradeDamage()
    {
        if (currentMoney >= damageUpgradeCost && damageLevel < 5)
        {
            damageLevel++;
            damageLevel = Mathf.Clamp(damageLevel, 1, 5);
            currentMoney -= damageUpgradeCost;
        }
    }

    public void BuyHeartContainer()
    {
        if(currentMoney >= heartContainerCost && maxHealth < 10)
        {
            maxHealth++;
            maxHealth = Mathf.Clamp(maxHealth, 1, 10);
            currentHealth = maxHealth;
            currentMoney -= heartContainerCost;
        }
    }

    public void BuyWeapon()
    {
        if (currentMoney >= weaponCost)
        {
            gunType = GameManager.instance.planetGunType;
            UIManager.instance.weaponShop.GetComponent<Button>().interactable = false;
            currentMoney -= weaponCost;
        }
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
    }

    void LookAtMouse()
    {
        for (int i = 0; i < gunMeshes.Count; i++)
        {
            if(gunMeshes[i].activeInHierarchy)
                gunMeshes[i].SetActive(false);
        }
        gunMeshes[(int)gunType].SetActive(true);

        if (gunType == GunType.OctaShooter)
            return;

        Vector3 dir = (GetMouseWorldPos() - transform.position).normalized;
        gunMeshes[(int)gunType].transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), rotateSpeed * Time.deltaTime);
    }

    Vector3 GetMouseWorldPos()
    {
        Vector2 screenPos = Input.mousePosition;
        Vector3 worldSpace = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, mainCam.transform.position.y));
        worldSpace.y = 0;
        return worldSpace;
        /*Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Vector3 pos = Vector3.zero;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            pos = hit.point;
        }
        return pos;*/
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, dockRange);
    }
}

public enum GunType
{
    Normal,
    Shotgun,
    Laser,
    OctaShooter,
    BigCannon,
}

[System.Serializable]
public class GunInfo
{
    public List<int> damage;
    public List<float> fireRate;

    public Transform shootPoint;
}
