using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public GameObject starPrefab;

    private void Start()
    {
        for (int x = -50; x < 50; x++)
        {
            for (int y = -25; y < 25; y++)
            {
                GameObject star = Instantiate(starPrefab, new Vector3(Random.Range(-3, 3) + x * 6, -5, Random.Range(-3, 3) + y * 6), Quaternion.identity);
                float random = Random.Range(0.2f, 0.4f);
                star.transform.localScale = new Vector3(random, random, random);
                star.transform.parent = transform;
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
