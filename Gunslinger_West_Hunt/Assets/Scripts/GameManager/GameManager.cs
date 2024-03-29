using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsPlayerAlive { get => isPlayerAlive; set => isPlayerAlive = value; }
    public GameObject Player { get => player; }

    public int playerHealth;
    public int playerArmor;
    public HealthBar playerHealthBar;
    public ShieldBar shieldBar;

    private Vector2 playerPosition;
    private bool isPlayerAlive;
    [SerializeField] private GameObject player;

    public LoadoutSO playerLoadout;
    public int remainingTargets;
    public LevelMetaSO levelMeta;
    public bool levelWon;

    public LevelEndManager levelEndManager;

    public WeaponGaugeManager gaugeManager;
    public SoundManager soundManager;

    public GameObject deadPlayer;

    private void Awake()
    {
        Debug.Log($"Entered the level {levelMeta.levelName}");
        Instance = this;
        IsPlayerAlive = true;
        levelWon = false;
        remainingTargets = levelMeta.targets;
    }

    private void Start()
    {
        soundManager.PlaySound(levelMeta.levelTheme);
    }

    public void UpdatePlayerPosition(Vector2 newPosition)
    {
        playerPosition = newPosition;
    }

    public Vector2 GetPlayerPosition()
    {
        return playerPosition;
    }

    public void KillPlayer()
    {
        Instantiate(deadPlayer, player.transform);
        Inventory.GetInstance().GetInventoryItems().Clear();
        levelEndManager.InitiateEndScreen(levelMeta.levelName, "You Died!", "$0", levelMeta.sceneName);
        player.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void CheckWinCondition()
    {
        if(remainingTargets == 0)
        {
            levelWon = true;
            levelEndManager.ToggleLevelClear(true);
            Debug.Log("Level Cleared");
        }
    }

    public void OnLevelExit()
    {
        soundManager.StopSound(levelMeta.levelTheme);
        Inventory inventory = Inventory.GetInstance();
        int totalReward = levelMeta.levelReward;
        foreach(KeyValuePair<ItemDetailSO, int> entry in inventory.GetInventoryItems())
        {
            if(entry.Key.type == ItemType.bounty)
            {
                totalReward += entry.Key.value * entry.Value;
            }
        }
        playerLoadout.playerMoney += totalReward;
        Debug.Log($"Player Gained {totalReward}");
        levelEndManager.InitiateEndScreen(levelMeta.levelName, "Level Won!", $"${totalReward}", levelMeta.sceneName);
        isPlayerAlive = false;
        levelMeta.RecordPassedLevel(levelMeta.levelName);
        levelMeta.isNextLevelUnlocked = true;
    }

    public void UpdateWeaponUI(WeaponStatus status)
    {
        if (status.isReloading)
        {
            gaugeManager.UpdateAmmoIndicator("Reload");
        }
        else
        {
            string indicator = $"{status.currentAmmo}/{status.maxAmmo}";
            gaugeManager.UpdateAmmoIndicator(indicator);
        }
    }

}
