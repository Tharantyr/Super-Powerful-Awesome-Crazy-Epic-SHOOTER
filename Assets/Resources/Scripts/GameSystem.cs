﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    public GameObject player, shop;
    public GameObject UI;
    public AudioSource soundManager, musicManager, explosionManager;
    public static GameSystem instance;
    public enum GameState { MainMenu, PauseMenu, Wave, WaveStart, WaveOver, GameOver, Shop, Highscores };
    public GameState gameState;
    public int rocketCapacity, shieldCapacity;
    public float waveTime, shieldTime;
    public List<GameObject> enemyList;
    public AudioClip music;
    public AudioClip[] PlayerSounds, EnemySounds, UISounds;
    public bool freezeTimer;
    public GameObject enemySpawner;

    int orbCount, killCount, rocketCount, lifeCount, health, shield, score, waveCount;
    GameObject orbCounter, killCounter, rocketCounter, lifeCounter, healthCounter, shieldCounter, scoreCounter, timeCounter, highscores, mainMenu, waveText;
    float timer;
    Coroutine c;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        // Initialize stuff
        orbCounter = UI.transform.Find("orbCounter").gameObject;
        killCounter = UI.transform.Find("killCounter").gameObject;
        rocketCounter = UI.transform.Find("rocketCounter").gameObject;
        lifeCounter = UI.transform.Find("lifeCounter").gameObject;
        healthCounter = UI.transform.Find("healthCounter").gameObject;
        shieldCounter = UI.transform.Find("shieldCounter").gameObject;
        scoreCounter = UI.transform.Find("TopUI").Find("Score").gameObject;
        timeCounter = UI.transform.Find("TopUI").Find("Timer").gameObject;
        waveText = UI.transform.Find("WaveText").gameObject;
    }

    private void Start()
    {
        // Initialize music
        musicManager.priority = 0;
        musicManager.clip = music;
        musicManager.volume = 0.1f;
        musicManager.loop = true;
        musicManager.Play();

        // Initialize sound effects
        soundManager.volume = 0.5f;

        enemySpawner = ObjectPool.instance.GetPooledObject("EnemySpawner", false); // Handles logic for spawning enemy waves
        gameState = GameState.MainMenu;
        waveTime = 60;
        mainMenu = ObjectPool.instance.GetPooledObject("MainMenu", false);
        shop = ObjectPool.instance.GetPooledObject("Shop", false);
        highscores = ObjectPool.instance.GetPooledObject("Highscores", false);
        c = null;
        freezeTimer = false;
        player.SetActive(false);
        enemyList = new List<GameObject>(); // Currently active enemies
    }

    public void InitializePlayer() // Initialize values for player
    {
        Orbs = 0;
        Kills = 0;
        Rockets = 3;
        Lives = 3;
        Health = 100;
        Shield = 40;
        Score = 0;
        rocketCapacity = 3;
        shieldCapacity = 40;
        shieldTime = 10;
        Wave = 0;
        timer = waveTime;
    }

    private void Update() // Transition between different phases of the game
    {
        if (gameState == GameState.MainMenu)
            mainMenu.SetActive(true);

        else if (gameState == GameState.PauseMenu)
            freezeTimer = true;

        else if (gameState == GameState.WaveStart)
        {
            player.SetActive(true);

            if (c == null)
                c = StartCoroutine(WaveCountdown());
        }

        else if (gameState == GameState.Wave)
        {
            freezeTimer = false;
            c = null;
            enemySpawner.SetActive(true);
        }

        else if (gameState == GameState.WaveOver)
        {
            if (c == null)
            {
                timer = waveTime;
                c = StartCoroutine(WaveEnded());
            }
        }

        else if (gameState == GameState.Shop)
        {
            freezeTimer = false;
            c = null;

            if (!shop.activeInHierarchy)
            {
                waveText.SetActive(false); // Deactivate this UI component in shop so it doesn't interfere with mouse hovering over items
                shop.SetActive(true);
            }
        }

        else if (gameState == GameState.GameOver)
        {
            if (c == null)
            {
                Score += 10 * Orbs;
                c = StartCoroutine(GameOver());
            }
        }

        else if (gameState == GameState.Highscores)
        {
            c = null;

            if (!highscores.activeInHierarchy)
            {
                highscores.SetActive(true);
            }
        }
    }

    public void StartACoroutine(IEnumerator c) // Used to start coroutines from objects that are about to be deactivated so the coroutine continues playing
    {
        StartCoroutine(c);
    }

    public IEnumerator GameOver() // Used to transition to highscores list
    {
        Text waveTextText = waveText.GetComponent<Text>();
        Text waveCounter = waveText.transform.GetChild(0).GetComponent<Text>();
        waveText.GetComponent<CanvasGroup>().alpha = 1;
        waveCounter.GetComponent<CanvasGroup>().alpha = 1;
        WaitForSeconds wait = new WaitForSeconds(2);

        waveCounter.text = "Game";
        waveTextText.text = "Over";
        yield return wait;
        waveCounter.GetComponent<CanvasGroup>().alpha = 0;
        waveTextText.fontSize = 180;
        waveTextText.text = "Loading Highscores";
        yield return wait;
        gameState = GameState.Highscores;

        waveTextText.GetComponent<CanvasGroup>().alpha = 0;
        waveTextText.fontSize = 300;
    }

    public IEnumerator DeathCountdown() // Countdown for respawn
    {
        freezeTimer = true; // Stop wave timer from running while respawning
        Text waveTextText = waveText.GetComponent<Text>();
        Text waveCounter = waveText.transform.GetChild(0).GetComponent<Text>();
        waveText.GetComponent<CanvasGroup>().alpha = 1;
        waveCounter.GetComponent<CanvasGroup>().alpha = 1;
        WaitForSeconds wait = new WaitForSeconds(1);

        waveCounter.text = "Oops";
        waveTextText.text = "3";
        yield return wait;
        waveTextText.text = "2";
        yield return wait;
        waveTextText.text = "1";
        yield return wait;
        waveCounter.GetComponent<CanvasGroup>().alpha = 0;
        waveTextText.text = "GO";
        freezeTimer = false;
        player.transform.position = new Vector2(0, -2);
        Health = 100;
        Shield = shieldCapacity;
        Rockets = rocketCapacity;
        player.SetActive(true);
        yield return wait;

        waveText.GetComponent<CanvasGroup>().alpha = 0;
        waveCounter.GetComponent<CanvasGroup>().alpha = 1;
    }

    private IEnumerator WaveCountdown() // Countdown before every wave
    {
        Wave++;
        Text waveTextText = waveText.GetComponent<Text>();
        Text waveCounter = waveText.transform.GetChild(0).GetComponent<Text>();
        waveText.GetComponent<CanvasGroup>().alpha = 1;
        waveCounter.GetComponent<CanvasGroup>().alpha = 1;
        WaitForSeconds wait = new WaitForSeconds(1);

        waveCounter.text = "Wave " + Wave;
        waveTextText.text = "3";
        yield return wait;
        waveTextText.text = "2";
        yield return wait;
        waveTextText.text = "1";
        yield return wait;
        waveCounter.GetComponent<CanvasGroup>().alpha = 0;
        waveTextText.text = "GO";
        gameState = GameState.Wave;
        yield return wait;

        waveText.GetComponent<CanvasGroup>().alpha = 0;
        waveCounter.GetComponent<CanvasGroup>().alpha = 1;
    }

    private IEnumerator WaveEnded() // Transition from wave to shop
    {
        Text waveTextText = waveText.GetComponent<Text>();
        Text waveCounter = waveText.transform.GetChild(0).GetComponent<Text>();
        waveText.GetComponent<CanvasGroup>().alpha = 1;
        WaitForSeconds wait = new WaitForSeconds(2);

        waveCounter.text = "Wave " + Wave;
        waveTextText.text = "Complete";
        yield return wait;
        waveCounter.GetComponent<CanvasGroup>().alpha = 0;
        waveTextText.fontSize = 240;
        waveTextText.text = "Entering Shop";
        yield return wait;

        waveText.GetComponent<CanvasGroup>().alpha = 0;
        waveTextText.fontSize = 300;
        gameState = GameState.Shop;
    }

    public bool InView(GameObject o) // Check if object in camera view
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        if (GeometryUtility.TestPlanesAABB(planes, o.GetComponent<Collider2D>().bounds))
            return true;
        else
            return false;
    }

    private string FloatToSeconds(float f) // Convert float to 0:00 format to display timer in top right corner
    {
        string result;

        if (timer >= 10)
            result = "0:" + Mathf.FloorToInt(f).ToString();
        else if (timer > 1)
            result = "0:0" + Mathf.FloorToInt(f).ToString();
        else
            result = "0:00";

        return result;
    }

    // Bunch of properties that I made just to make it easy to update the UI
    public int Orbs
    {
        get { return orbCount; }

        set
        {
            orbCount = value;
            orbCounter.GetComponent<Text>().text = "* " + orbCount;
        }
    }

    public int Kills
    {
        get { return killCount; }

        set
        {
            killCount = value;
            killCounter.GetComponent<Text>().text = "* " + killCount;
        }
    }

    public int Rockets
    {
        get { return rocketCount; }

        set
        {
            rocketCount = value;
            rocketCounter.GetComponent<Text>().text = "* " + rocketCount;
        }
    }

    public int Lives
    {
        get { return lifeCount; }

        set
        {
            lifeCount = value;
            lifeCounter.GetComponent<Text>().text = "* " + lifeCount;
        }
    }

    public int Health
    {
        get { return health; }

        set
        {
            health = value;

            if (health < 0)
                health = 0;

            healthCounter.GetComponent<Text>().text = "* " + health;
            player.GetComponent<Player>().hp = health;
        }
    }

    public int Shield
    {
        get { return shield; }

        set
        {
            shield = value;

            if (shield < 0)
                shield = 0;

            shieldCounter.GetComponent<Text>().text = "* " + shield;
        }
    }

    public int Score
    {
        get { return score; }

        set
        {
            score = value;

            scoreCounter.GetComponent<Text>().text = score.ToString();
        }
    }

    public int Wave
    {
        get { return waveCount; }

        set
        {
            waveCount = value;

            timeCounter.GetComponent<Text>().text = "Wave " + waveCount + " * 0:59";
        }
    }

    public float Timer
    {
        get { return timer; }

        set
        {
            timer = value;

            timeCounter.GetComponent<Text>().text = "Wave " + waveCount + " * " + FloatToSeconds(timer);
        }
    }
}