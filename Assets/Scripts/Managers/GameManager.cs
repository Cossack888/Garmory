using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{


    [SerializeField] private GameObject inventoryManager;
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] GameObject menu;
    CameraTransition cameraTransition;
    private EquipmentPanelManager equipmentPanelManager;
    private EnemySpawner enemySpawner;
    private ItemEquipper equipper;
    private PlayerMovement movement;
    private FightControlls fightControlls;
    private Health health;
    private Statistics statistics;
    private bool menuMode;
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (InputManager.Instance != null)
        {
            Debug.Log("Input Setup");
            InputManager.Instance.OnToggleMenu += ToggleMenu;
        }
    }
    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnToggleMenu -= ToggleMenu;
        }
        if (health != null)
        {
            health.OnDeath -= ActivateDeathScreen;
        }
    }
    private void Start()
    {
        equipmentPanelManager = inventoryManager.GetComponent<EquipmentPanelManager>();
        movement = FindFirstObjectByType<PlayerMovement>();
        equipper = movement.GetComponent<ItemEquipper>();
        fightControlls = movement.GetComponent<FightControlls>();
        health = movement.GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath += ActivateDeathScreen;
        }
        statistics = movement.GetComponent<Statistics>();
        movement.enabled = false;
        cameraTransition = FindFirstObjectByType<CameraTransition>();
        enemySpawner = FindFirstObjectByType<EnemySpawner>();

    }
    public void ChangeCamera(CameraPoint cameraPoint)
    {
        cameraTransition.ChangeCamera(cameraPoint);
    }
    public void StartGame()
    {
        movement.enabled = true;
        ChangeCamera(CameraPoint.Game);
        equipper.DestroyUnequippedItems();
        enemySpawner.StartSpawning();
        ToggleInventory(false);
        TogglePauseGame(false);
        fightControlls.SetWeapon();
        health.InitialiseWeapons();
        health.InitialiseHealthPoints(statistics.HealthPoints + 1);
        LockCursor();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        System.GC.Collect();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void ToggleMenu()
    {
        if (inventoryCanvas.activeSelf || deathScreen.activeSelf || loadingScreen.activeSelf)
        {
            return;
        }
        UnLockCursor();
        menuMode = !menuMode;
        menu.SetActive(menuMode);
        TogglePauseGame(menuMode);
    }
    public void ActivateDeathScreen()
    {
        deathScreen.SetActive(true);
    }
    public void TogglePauseGame(bool state)
    {
        foreach (EnemyAI Ai in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            Ai.TogglePause(state);
        }
        movement.enabled = !state;
        fightControlls.enabled = !state;
        if (state)
        {
            enemySpawner.StopSpawning();
        }
        else
        {
            enemySpawner.StartSpawning();
        }

    }
    public void ToggleInventory(bool state)
    {
        if (deathScreen.activeSelf) return;
        UnLockCursor();
        TogglePauseGame(state);
        fightControlls.enabled = !state;
        if (equipmentPanelManager != null) equipmentPanelManager.enabled = state;
        inventoryCanvas.SetActive(state);
        if (state)
        {
            cameraTransition.ChangeCamera(CameraPoint.Overview);
        }
        else
        {
            cameraTransition.ChangeCamera(CameraPoint.Game);
        }
    }

    public void InitialiseStartButton()
    {
        startButton.SetActive(true);
    }
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void UnLockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
