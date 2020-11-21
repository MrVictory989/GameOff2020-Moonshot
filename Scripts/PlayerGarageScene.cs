using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGarageScene : MonoBehaviour
{
    public List<GameObject> gunMeshes;
    Player player;

    private void Start()
    {
        player = GameManager.instance.player.GetComponent<Player>();
    }

    private void Update()
    {
        for (int i = 0; i < gunMeshes.Count; i++)
        {
            if(i == (int)player.gunType)
            {
                if(!gunMeshes[i].activeInHierarchy)
                    gunMeshes[i].SetActive(true);
            }
            else
            {
                if (gunMeshes[i].activeInHierarchy)
                    gunMeshes[i].SetActive(false);
            }
        }
    }
}
