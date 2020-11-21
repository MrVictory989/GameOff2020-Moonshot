using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

    [HideInInspector]public List<GameObject> planets = new List<GameObject>();
    public List<GameObject> planetPresets = new List<GameObject>();

    public GameObject starPrefab;

    public GameObject[] asteroidPrefabs;
    public float asteroidSpeed = 10f;
    [Min(1)] public int asteroidCount;

    public List<EnemySpawner> spawners = new List<EnemySpawner>();
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public int enemyCount;

    public GameObject pauseMenu;

    public GameObject mainCam;
    public GameObject garageCam;

    public GunType planetGunType;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        planetPresets[Random.Range(0, planetPresets.Count)].SetActive(true);
        for (int x = -60; x < 60; x++)
        {
            for (int y = -35; y < 35; y++)
            {
                GameObject star = Instantiate(starPrefab, new Vector3(Random.Range(-3, 3) + x * 6, -5, Random.Range(-3, 3) + y * 6), Quaternion.identity);
                float random = Random.Range(0.2f, 0.4f);
                star.transform.localScale = new Vector3(random, random, random);
                star.transform.parent = transform;
            }
        }

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }

        for (int i = 0; i < asteroidCount; i++)
        {
            SpawnAsteroid();
        }

        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].enabled = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Dock()
    {
        List<GunType> gunTypes = new List<GunType>();
        foreach(GunType gt in System.Enum.GetValues(typeof(GunType)))
        {
            if (gt == player.GetComponent<Player>().gunType)
                continue;

            gunTypes.Add(gt);
        }
        planetGunType = gunTypes[Random.Range(0, gunTypes.Count)];

        player.GetComponent<Player>().Dock();
        player.GetComponent<Player>().enabled = false;
        UIManager.instance.SetDockedMenuActive(true, planetGunType);
        mainCam.SetActive(false);
        garageCam.SetActive(true);
        Time.timeScale = 0;
    }

    public void UnDock()
    {
        player.GetComponent<Player>().enabled = true;
        player.GetComponent<Player>().Undock();
        UIManager.instance.SetDockedMenuActive(false, planetGunType);
        mainCam.SetActive(true);
        garageCam.SetActive(false);
        Time.timeScale = 1;
    }

    public void SpawnAsteroid()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-300, 300), 0, Random.Range(-150, 150));
        Vector2 random = Random.insideUnitCircle;
        Vector3 dir = (spawnPos + new Vector3(random.x, 0, random.y) - spawnPos).normalized;
        GameObject asteroid = Instantiate(asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)], spawnPos, GetBulletSpawnRotation(dir, 0));
        //asteroid.transform.Rotate(Vector3.right, Random.Range(0, 360), Space.Self);
        asteroid.GetComponent<Asteroid>().Shoot(asteroidSpeed + Random.Range(-1.5f, 1.5f));
    }

    public void SpawnEnemy()
    {
        EnemySpawner spawner = spawners[Random.Range(0, spawners.Count)];
        GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], spawner.GetSpawnPoint(), Quaternion.identity);
        enemy.GetComponent<Enemy>().SetStartedPos(new Vector3(Random.Range(-300, 300), 0, Random.Range(-150, 150)));
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void UnPause()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public GameObject GetClosestPlanet(Vector3 position, out float closestPlanetDistance)
    {
        if(planets.Count <= 0)
        {
            closestPlanetDistance = float.MaxValue;
            return null;
        }

        GameObject closest = null;
        float closestDistance = float.MaxValue;
        for(int i = 0; i < planets.Count; i++)
        {
            float distance = Vector3.Distance(position, planets[i].transform.position);
            if (distance < closestDistance)
            {
                closest = planets[i];
                closestDistance = distance;
            }
        }

        closestPlanetDistance = closestDistance;
        return closest;
    }

    public Quaternion GetBulletSpawnRotation(Vector3 direction, float offset)
    {
        Quaternion bulletRotation = Quaternion.LookRotation(direction, Vector3.up);
        bulletRotation.y += offset;
        return bulletRotation;
    }
}
