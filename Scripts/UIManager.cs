using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Transform crosshair;
    public GameObject dockedMenu;
    public GameObject bossHealthBar;
    public GameObject gameOverMenu;
    public GameObject victoryMenu;

    public GameObject mapMenu;
    public GameObject mapCam;

    public List<GameObject> healthBarHearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public TextMeshProUGUI moneyText;

    public Sprite normalGunShop;
    public Sprite shotgunShop;
    public Sprite octoShooterShop;
    public Sprite bigCannonShop;
    public Sprite laserShop;
    public Image weaponShop;

    public UpgradeBarUI speedBar;
    public UpgradeBarUI fireRateBar;
    public UpgradeBarUI damageBar;

    Player player;

    private void Awake()
    {
        if (instance == null)
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
        player = GameManager.instance.player.GetComponent<Player>();
    }

    private void Update()
    {
        crosshair.transform.position = Input.mousePosition;

        for (int i = 0; i < healthBarHearts.Count; i++)
        {
            if(i < player.maxHealth)
            {
                healthBarHearts[i].SetActive(true);
                if(i < player.currentHealth)
                {
                    healthBarHearts[i].GetComponent<Image>().sprite = fullHeart;
                }
                else
                {
                    healthBarHearts[i].GetComponent<Image>().sprite = emptyHeart;
                }
            }
            else
            {
                healthBarHearts[i].SetActive(false);
            }
        }

        moneyText.text = $"${player.currentMoney}";

        speedBar.SetImageSprite(player.speedLevel);
        fireRateBar.SetImageSprite(player.fireRateLevel);
        damageBar.SetImageSprite(player.damageLevel);

        if (Input.GetKey(KeyCode.Tab))
        {
            mapMenu.SetActive(true);
            mapCam.SetActive(true);
        }
        else
        {
            mapMenu.SetActive(false);
            mapCam.SetActive(false);
        }
    }

    public void SetDockedMenuActive(bool active, GunType gunType)
    {
        dockedMenu.SetActive(active);
        weaponShop.GetComponent<Button>().interactable = true;

        switch (gunType)
        {
            case GunType.Normal:
                weaponShop.sprite = normalGunShop;
                break;
            case GunType.Shotgun:
                weaponShop.sprite = shotgunShop;
                break;
            case GunType.OctaShooter:
                weaponShop.sprite = octoShooterShop;
                break;
            case GunType.Laser:
                weaponShop.sprite = laserShop;
                break;
            case GunType.BigCannon:
                weaponShop.sprite = bigCannonShop;
                break;
        }
    }

    public void SetBossHealthBarActive(bool active)
    {
        bossHealthBar.SetActive(active);
    }

    public void SetGameOverMenuActive(bool active)
    {
        gameOverMenu.SetActive(active);
        bossHealthBar.SetActive(false);
    }

    public void SetVictoryMenu(bool active)
    {
        victoryMenu.SetActive(active);
        bossHealthBar.SetActive(false);
    }
}

[System.Serializable]
public class UpgradeBarUI
{
    public Sprite levelOne;
    public Sprite levelTwo;
    public Sprite levelThree;
    public Sprite levelFour;
    public Sprite levelFive;

    public Image image;

    public void SetImageSprite(int level)
    {
        if(level == 1)
        {
            image.sprite = levelOne;
        }else if (level == 2)
        {
            image.sprite = levelTwo;
        }
        else if (level == 3)
        {
            image.sprite = levelThree;
        }
        else if (level == 4)
        {
            image.sprite = levelFour;
        }
        else if (level == 5)
        {
            image.sprite = levelFive;
        }
        else
        {
            Debug.LogWarning("No sprite for the level given!!!");
        }
    }
}
