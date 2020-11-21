using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public enum State { Waiting, ShootingAllDirectionsAlternating, HomingMissile, ShootingAllDirections, Changing, Dead}
    public State state = State.Waiting;

    public List<BossPart> bossParts = new List<BossPart>();

    public float timeBetweenStates = 1f;

    public float detectionRange = 80f;
    public GameObject detectionRangeObject;
    public GameObject colliderObject;

    public float allDirectionAlternatingFireRate = 1;
    float allDirectionAlternatingFireRateTimer = 0;
    bool shotEven = false;
    public int allDirectionAlternatingShotsToFire = 4;
    int allDirectionAlternatingShotsFired = 0;

    public int homingMissilesToFire = 2;

    public float allDirectionFireRate = 1f;
    float allDirectionFireRateTimer = 0;
    public int allDirectionShotsToFire = 3;
    int allDirectionShotsFired = 0;

    GameObject player;
    Vector3 startedPos;

    private void Start()
    {
        player = GameManager.instance.player;
        detectionRangeObject.transform.localScale = new Vector3(detectionRange * 4, 0.1f, detectionRange * 4);
        startedPos = transform.position;
    }

    private void Update()
    {
        transform.position = startedPos;
        switch (state)
        {
            case State.Waiting:
                if(Vector3.Distance(transform.position, player.transform.position) < detectionRange)
                {
                    StartBossFight();
                }
                break;
            case State.ShootingAllDirectionsAlternating:
                ShootingAllDirectionsAlternating();
                break;
            case State.HomingMissile:
                HomingMissile();
                break;
            case State.ShootingAllDirections:
                ShootingAllDirections();
                break;
            case State.Changing:
                break;
            case State.Dead:
                break;
        }
    }

    void ShootingAllDirectionsAlternating()
    {
        if (allDirectionAlternatingFireRateTimer <= 0)
        {
            for (int i = 0; i < bossParts.Count; i++)
            {
                if (i % 2 == 0 && !shotEven)
                {
                    //Debug.Log("Called in even with i of " + i + (i % 2 == 0 && !shotEven));
                    bossParts[i].Shoot();
                }
                else if(i % 2 != 0 && shotEven)
                {
                    //Debug.Log("Called in odd with i of " + i + (i % 2 == 0 && !shotEven));
                    bossParts[i].Shoot();
                }
            }
            shotEven = !shotEven;
            allDirectionAlternatingFireRateTimer = 1f/allDirectionFireRate;

            allDirectionAlternatingShotsFired++;
            if(allDirectionAlternatingShotsFired >= allDirectionAlternatingShotsToFire)
            {
                StartCoroutine(GetNewState());
                allDirectionAlternatingShotsFired = 0;
            }
        }
        allDirectionAlternatingFireRateTimer -= Time.deltaTime;
    }

    void HomingMissile()
    {
        /*
        List<BossPart> parts = new List<BossPart>(bossParts);
        for (int i = 0; i < homingMissilesToFire; i++)
        {
            BossPart part = parts[Random.Range(0, parts.Count)];
            parts.Remove(part);
            part.SpawnMissile();
        }*/

        BossPart closest = GetClosestBossPart(player.transform.position);
        Debug.Log(closest.name);
        BossPart other = null;

        int index = bossParts.IndexOf(closest);
        if(index <= 0)
        {
            other = bossParts[index + 1];
        }else if(index + 1 >= bossParts.Count)
        {
            other = bossParts[index - 1];
        }
        else
        {
            if(Random.value > 0.5f)
            {
                other = bossParts[index - 1];
            }
            else
            {
                other = bossParts[index + 1];
            }
        }

        closest.SpawnMissile();

        if(other != null)
            other.SpawnMissile();

        StartCoroutine(GetNewState());
    }

    void ShootingAllDirections()
    {
        if(allDirectionFireRateTimer <= 0)
        {
            for (int i = 0; i < bossParts.Count; i++)
            {
                bossParts[i].Shoot();
            }
            allDirectionFireRateTimer = 1f / allDirectionFireRate;
            allDirectionShotsFired++;
            if(allDirectionShotsFired >= allDirectionShotsToFire)
            {
                allDirectionShotsFired = 0;
                StartCoroutine(GetNewState());
            }
        }
        allDirectionFireRateTimer -= Time.deltaTime;
    }

    void StartBossFight()
    {
        detectionRangeObject.SetActive(false);
        colliderObject.SetActive(true);
        UIManager.instance.SetBossHealthBarActive(true);

        Enemy[] enemies = FindObjectsOfType(typeof(Enemy)) as Enemy[];
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].gameObject.SetActive(false);
        }

        StartCoroutine(GetNewState());
    }

    IEnumerator GetNewState()
    {
        State previousState = state;
        state = State.Changing;

        yield return new WaitForSeconds(timeBetweenStates);

        float random = Random.value;
        if(previousState == State.HomingMissile)
        {
            if(random < 0.5f)
            {
                state = State.ShootingAllDirections;
            }
            else
            {
                state = State.ShootingAllDirectionsAlternating;
            }
        }else if (previousState == State.ShootingAllDirections)
        {
            if (random < 0.5f)
            {
                state = State.HomingMissile;
            }
            else
            {
                state = State.ShootingAllDirectionsAlternating;
            }
        }else if (previousState == State.ShootingAllDirectionsAlternating)
        {
            if (random < 0.5f)
            {
                state = State.ShootingAllDirections;
            }
            else
            {
                state = State.HomingMissile;
            }
        }else if(previousState == State.Waiting)
        {
            if(random < 0.33f)
            {
                state = State.ShootingAllDirectionsAlternating;
            }else if(random > 0.33f && random < 0.66f)
            {
                state = State.ShootingAllDirections;
            }else if (random > 0.66f)
            {
                state = State.HomingMissile;
            }
        }
    }

    public void RemovePart(BossPart part)
    {
        bossParts.Remove(part);
        if(bossParts.Count <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        state = State.Dead;
        UIManager.instance.SetVictoryMenu(true);
    }

    BossPart GetClosestBossPart(Vector3 position)
    {
        BossPart closest = null;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < bossParts.Count; i++)
        {
            float distance = Vector3.Distance(position, bossParts[i].accuratePosition.position);
            if (distance < closestDistance)
            {
                closest = bossParts[i];
                closestDistance = distance;
            }
        }

        return closest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
