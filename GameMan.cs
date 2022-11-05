using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameMan : MonoBehaviour
{
    private List<GameObject> collectables;

    [SerializeField]
    private List<WeaponBase> availableWeapons;

    [SerializeField]
    private List<GameObject> enemiesPrefabs;

    [SerializeField]
    private float minX;

    [SerializeField]
    private float maxX;

    [SerializeField]
    private Slider healthProgressBar;

    [SerializeField]
    private GameObject inventoryPanel;

    [SerializeField]
    private GameObject avatarPanel;

    [SerializeField]
    private GameObject avatarModel;

    [SerializeField]
    private Camera avatarCam;

    [SerializeField]
    private float multiplier;

    private Vector2 inventoryPanelInitPos;
    private Vector2 avatarPanelInitPos;

    private bool isAvatarPanelActive = false;

    private int currentWeaponIndex = 0;

    Vector3 currentPos;

    public int waveIndex = 0;
    public int enemyCount = 0;
    private int waveCount = 10;

    private void ClearSpawnEnemiesSettings()
    {
        this.waveIndex = 0;
        this.enemyCount = 0;
    }

    private void SpawnEnemyWave()
    {
        var playerRef = FindObjectOfType<PlayerController>();
        if (playerRef)
        {
            for (int i = 0; i < waveCount; i++)
            {
                var enemy = Instantiate(enemiesPrefabs[UnityEngine.Random.Range(0, enemiesPrefabs.Count)]);
                enemy.transform.position = new Vector3(UnityEngine.Random.Range(playerRef.minXMovement, playerRef.maxXMovement), 0, 0);

                enemyCount++;
            }

            waveIndex++;
        }
    }

    public void DecreaseEnemiesCount()
    {
        this.enemyCount--;
    }

    public void UpdateHealthProgressBar()
    {
        if (healthProgressBar)
        {
            healthProgressBar.value = PlayerPrefs.GetFloat("player_health", 1.0f) * 100.0f;
        }
    }

    private void GetInitialPositionOfInventoryAndClose()
    {
        if (inventoryPanel)
        {
            inventoryPanelInitPos = inventoryPanel.GetComponent<RectTransform>().anchoredPosition;

            inventoryPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(Screen.currentResolution.width, 0), 0.0f);
        }
    }

    public void OpenInventoryPanel()
    {
        if (inventoryPanel)
        {
            inventoryPanel.GetComponent<RectTransform>().DOAnchorPos(inventoryPanelInitPos, 0.25f);

            //inventoryPanel.GetComponent<InventoryController>().LoadItems();
        }
    }

    public void CloseInventoryPanel()
    {
        if (inventoryPanel)
        {
            inventoryPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(Screen.currentResolution.width, 0), 0.25f);
            inventoryPanel.GetComponent<InventoryController>().StoreItems();
        }
    }

    private void GetInitialPositionOfAvatarPanelAndClose()
    {
        if (avatarPanel)
        {
            avatarPanelInitPos = avatarPanel.GetComponent<RectTransform>().anchoredPosition;

            avatarPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-Screen.currentResolution.width, 0), 0.0f);

            isAvatarPanelActive = false;
        }
    }

    public void OpenAvatarPanel()
    {
        if (avatarPanel)
        {
            avatarPanel.GetComponent<RectTransform>().DOAnchorPos(avatarPanelInitPos, 0.25f);

            isAvatarPanelActive = true;
        }
    }

    public void CloseAvatarPanel()
    {
        if (avatarPanel)
        {
            avatarPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-Screen.currentResolution.width, 0), 0.25f);

            isAvatarPanelActive = false;
        }
    }

    public void InstantiateDroppedItem(Collectable collectable)
    {
        var player = FindObjectOfType<PlayerController>();

        if (player != null)
        {
            if (collectable.type == CollectableType.PowerUp)
            {
                var prefab = Resources.Load<GameObject>("powerups");
                if (prefab)
                {
                    var collectableObject = Instantiate(prefab);
                    collectableObject.transform.position = new Vector3(
                        player.transform.position.x
                        + UnityEngine.Random.Range(0,2) == 0?
                            UnityEngine.Random.Range(1.0f, 2.0f) : UnityEngine.Random.Range(-1.0f, -2.0f),
                        collectableObject.transform.position.y,
                        collectableObject.transform.position.z);

                    collectables.Add(collectableObject);
                }
            }

            // TODO: later on we should add other collectables 
        }
    }

    void ClearCollectables()
    {
        if (collectables == null)
        {
            collectables = new List<GameObject>();
        }
        else
        {
            foreach (var collectable in collectables)
            {
                Destroy(collectable.gameObject);
            }
        }

        collectables.Clear();
    }

    private void GenerateCollectables()
    {
        var coinPrefab = Resources.Load<GameObject>("coin");
        var healthPrefab = Resources.Load<GameObject>("health");
        var powerupPrefab = Resources.Load<GameObject>("powerups");

        for (int i = 0; i < 15; i++)
        {
            int ch = UnityEngine.Random.Range(0, 3);
            if (ch == 0)
            {
                if (coinPrefab)
                {
                    var coin = Instantiate(coinPrefab);

                    coin.transform.position = new Vector3(UnityEngine.Random.Range(minX, maxX), coin.transform.position.y, coin.transform.position.z);

                    collectables.Add(coin);
                }
            }
            else if (ch == 1)
            {
                if (healthPrefab)
                {
                    var health = Instantiate(healthPrefab);

                    health.transform.position = new Vector3(UnityEngine.Random.Range(minX, maxX), health.transform.position.y, health.transform.position.z);

                    collectables.Add(health);
                }
            }
            else
            {
                if (powerupPrefab)
                {
                    var powerup = Instantiate(powerupPrefab);

                    powerup.transform.position = new Vector3(UnityEngine.Random.Range(minX, maxX), powerup.transform.position.y, powerup.transform.position.z);

                    collectables.Add(powerup);
                }
            }
        }
    }

    void LoadWeapon()
    {
        string weaponName = PlayerPrefs.GetString("current_player_weapon", "none");
        if (weaponName == "none")
        {
            weaponName = "sword";
        }

        if (availableWeapons != null)
        {
            if (availableWeapons.Count > 0)
            {
                int count = 0;
                foreach (var weapon in availableWeapons)
                {
                    if (weapon.GetName().Trim() == weaponName)
                    {
                        this.currentWeaponIndex = count;
                        var playerController = FindObjectOfType<PlayerController>();
                        if (playerController)
                        {
                            playerController.InjectWeapon(weapon);
                        }
                        var avatarObject = FindObjectOfType<AvatarObject>();
                        if (avatarObject)
                        {
                            avatarObject.PlaceWeapon(weapon);
                        }
                        if (avatarPanel)
                        {
                            avatarPanel.GetComponent<AvatarPanel>().LoadWeapon(weapon);
                        }
                        break;
                    }
                    count++;
                }
            }
        }
    }

    public void NextWeaponBtnCallback()
    {
        this.currentWeaponIndex++;
        if (this.currentWeaponIndex >= availableWeapons.Count)
        {
            this.currentWeaponIndex = 0;
        }
        var weapon = availableWeapons[this.currentWeaponIndex];
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController)
        {
            playerController.InjectWeapon(weapon);
        }
        var avatarObject = FindObjectOfType<AvatarObject>();
        if (avatarObject)
        {
            avatarObject.PlaceWeapon(weapon);
        }
        if (avatarPanel)
        {
            avatarPanel.GetComponent<AvatarPanel>().LoadWeapon(weapon);
        }
    }

    public void PreviousWeaponBtnCallback()
    {
        this.currentWeaponIndex--;
        if (this.currentWeaponIndex <= 0)
        {
            this.currentWeaponIndex = availableWeapons.Count - 1;
        }
        var weapon = availableWeapons[this.currentWeaponIndex];
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController)
        {
            playerController.InjectWeapon(weapon);
        }
        var avatarObject = FindObjectOfType<AvatarObject>();
        if (avatarObject)
        {
            avatarObject.PlaceWeapon(weapon);
        }
        if (avatarPanel)
        {
            avatarPanel.GetComponent<AvatarPanel>().LoadWeapon(weapon);
        }
    }

    public void QuitGameBtnCallback()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void OnEnable()
    {
        GetInitialPositionOfInventoryAndClose();

        GetInitialPositionOfAvatarPanelAndClose();

        UpdateHealthProgressBar();

        ClearCollectables();

        GenerateCollectables();

        LoadWeapon();

        ClearSpawnEnemiesSettings();

        //SpawnEnemyWave();
    }

    private void Update()
    {
        CheckPanRotationControlForPlayer();

        //SpawnEnemies();
    }

    void CheckPanRotationControlForPlayer()
    {
        if (isAvatarPanelActive)
        {
            if (Input.touchCount == 1)
            {
                var touch = Input.GetTouch(0);
                //targetTransform.Rotate(cameraTransform.right * touch.deltaPosition.x * multiplier, Space.World);
                avatarModel.transform.Rotate(avatarCam.transform.up * touch.deltaPosition.y * multiplier, Space.World);
            }
        }
    }
}
