using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public string planetName;

    private void Start()
    {
        GameManager.instance.planets.Add(gameObject);
    }
}
