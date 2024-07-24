using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    private Vector2 screenBounds;
    static float width = 3.7f;
    static float height = 7.0f;
    public float snakeSpeed = 1;

    public BodyPart bodyPrefab = null;
    public GameObject rockPrefab = null;
    public GameObject eggPrefab = null;
    public GameObject goldEggPrefab = null;
    public GameObject spikePrefab = null;

    public Sprite tailSprite = null;
    public Sprite bodySprite = null;
    public SnakeHead snakeHead = null;

    public bool alive = true;
    public bool waitingToPlay = true;

    List<Egg> eggs = new List<Egg>();
    List<Spike> spikes = new List<Spike>();

    int level = 0;
    int noOfEggsForNextLevel = 0;
    int noOfSpikesInLevel = 0;
    Vector3 lastEggPosition;

    public int score = 0;
    public int hiScore = 0;
    public Text scoreText = null;
    public Text hiScoreText = null;
    public Text tapToPlayText = null;
    public Text gameOverText = null;
    public GameObject grass = null;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        width = screenBounds.x;
        height = screenBounds.y;
        if ((width/height) > 0.0f)
        {
            grass.transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        Debug.Log("Starting Snake Game");
        CreateWalls();
        alive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingToPlay)
        {
            foreach(Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    StartGamePlay();
                }
            }

            if (Input.GetMouseButtonUp(0))
                StartGamePlay();
        }
    }

    public void GameOver()
    {
        alive = false;
        waitingToPlay = true;
        gameOverText.gameObject.SetActive(true);
        tapToPlayText.gameObject.SetActive(true);
        noOfSpikesInLevel = 0;
    }

    void StartGamePlay()
    {
        score = 0;
        level = 0;
        noOfSpikesInLevel = 0;
        scoreText.text = "Score: " + score;
        hiScoreText.text = "High Score: " + hiScore;
        gameOverText.gameObject.SetActive(false);
        tapToPlayText.gameObject.SetActive(false);

        waitingToPlay = false;
        alive = true;
        KillOldEggs();
        KillOldSpikes();
        snakeHead.ResetSnake();
        LevelUp();
    }

    void LevelUp()
    {
        level++;
        noOfEggsForNextLevel = 4 + (level * 2);
        noOfSpikesInLevel++;
        snakeSpeed = 1f + (level / 4f);
        if (snakeSpeed > 6) snakeSpeed = 6;

        snakeHead.ResetSnake();
        CreateEgg();

        KillOldSpikes();
        for (int i = 0; i < noOfSpikesInLevel; i++) CreateSpike();
    }

    public void EggEaten(Egg egg)
    {
        score++;

        noOfEggsForNextLevel--;
        if (noOfEggsForNextLevel == 0)
        {
            score += 10;
            LevelUp();
        }
        else if (noOfEggsForNextLevel == 1)
            CreateEgg(true);
        else
            CreateEgg(false);

        if (score > hiScore)
        {
            hiScore = score;
            hiScoreText.text = "High Score: " + hiScore;
        }
        scoreText.text = "Score: " + score;

        eggs.Remove(egg);
        Destroy(egg.gameObject);
    }

    void CreateWalls()
    {
        Vector3 start  = new Vector3(-width, -height, -1);
        Vector3 finish = new Vector3(-width, +height, -1);
        CreateWall(start, finish);

        start = new Vector3(+width, -height, -1);
        finish = new Vector3(+width, +height, -1);
        CreateWall(start, finish);

        start = new Vector3(-width, -height, -1);
        finish = new Vector3(+width, -height, -1);
        CreateWall(start, finish);

        start = new Vector3(-width, +height, -1);
        finish = new Vector3(+width, +height, -1);
        CreateWall(start, finish);
    }

    void CreateWall(Vector3 start, Vector3 finish)
    {
        float distance = Vector3.Distance(start, finish);
        int noOfRocks = (int)(distance * 3);
        Vector3 delta = (finish - start) / noOfRocks;

        Vector3 position = start;
        for(int rock = 0; rock <= noOfRocks; rock++)
        {
            float rotation = Random.Range(0, 360);
            float scale    = Random.Range(1.5f, 2f);
            CreateRock(position, scale, rotation);
            position = position + delta;
        }
    }

    void CreateRock(Vector3 position, float scale, float rotation)
    {
        GameObject rock = Instantiate(rockPrefab, position, Quaternion.Euler(0,0,rotation));
        rock.transform.localScale = new Vector3(scale, scale, 1);
    }

    void CreateEgg(bool golden = false)
    {
        Vector3 position;
        position.x = -width + Random.Range(1f, (width * 2) - 2f);
        position.y = -height + Random.Range(1f, (height * 2) - 2f);
        position.z = -1f;
        Egg egg = null;
        if (golden)
            egg = Instantiate(goldEggPrefab, position, Quaternion.identity).GetComponent<Egg>();
        else
            egg = Instantiate(eggPrefab, position, Quaternion.identity).GetComponent<Egg>();
        eggs.Add(egg);
        lastEggPosition = position;
    }

    void CreateSpike()
    {
        Vector3 position;
        Vector3 center = new Vector3(0, 0, -1f);
        do
        {
            position.x = -width + Random.Range(1f, (width * 2) - 2f);
            position.y = -height + Random.Range(1f, (height * 2) - 2f);
            position.z = -1f;
        } while ((Vector3.Distance(position, lastEggPosition)<2.0f) || (Vector3.Distance(position,center)<2.0f));
        
        Spike spike = null;
        spike = Instantiate(spikePrefab, position, Quaternion.identity).GetComponent<Spike>();
        spikes.Add(spike);
    }

    void KillOldEggs()
    {
        foreach (Egg egg in eggs)
        {
            Destroy(egg.gameObject);
        }
        eggs.Clear();
    }

    void KillOldSpikes()
    {
        foreach (Spike spike in spikes)
        {
            Destroy(spike.gameObject);
        }
        spikes.Clear();
    }
}
