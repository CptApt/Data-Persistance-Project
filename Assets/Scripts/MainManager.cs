using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;
    public PlayerData currentPlayer;
    public PlayerData highScorePlayer;
    //public List<PlayerData> highScores;
    //private int highScoresLimit = 8;

    //public GameObject highScoresContainer;
    //public GameObject highScoreTemplate;
    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    public bool hasSpawned = false;
    //private int m_Points;
    
    private bool m_GameOver = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        
    }

    // Start is called before the first frame update
    void Start()
    {
        currentPlayer.playerName = "";
        

        LoadScore();

        if(highScorePlayer == null)
        {
            highScorePlayer = new PlayerData();
            highScorePlayer.playerName = "n/a";
            highScorePlayer.playerScore = 0;
            SaveScore();
        }

        /*
        if(highScores.Count < highScoresLimit)
        {
            int missingScores = highScoresLimit - highScores.Count;
            for(int i = 0; i < missingScores; i++)
            {
                PlayerData emptyScore = new PlayerData();
                emptyScore.playerName = "n/a";
                emptyScore.playerScore = 0;
                highScores.Add(emptyScore);
            }
        }
        */
        
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1 && !hasSpawned)
        {
            hasSpawned = true;
            StartMain();
        }

        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                hasSpawned = false;
                m_Started = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                hasSpawned = false;
                m_Started = false;
                SceneManager.LoadScene(0);
            }
        }
    }

    /*public void DisplayHighScores()
    {
        for(int i = 0; i < highScores.Count; i++)
        {
            GameObject go = Instantiate(highScoreTemplate, entryTransform)
        }
    }*/

    void AddPoint(int point)
    {
        //m_Points += point;
        currentPlayer.playerScore += point;
        ScoreText.text = $"{currentPlayer.playerName}'s Score : {currentPlayer.playerScore}";
        if(currentPlayer.playerScore > highScorePlayer.playerScore)
        {
            BestScoreText.text = $"Best Score: \n {currentPlayer.playerName}: {currentPlayer.playerScore}";
        }
    }

    /*
private void SortHighScores()
    {
        
        highScores.Sort((PlayerData p1, PlayerData p2) => p2.playerScore.CompareTo(p1.playerScore));
        int _extraScores = highScores.Count - highScoresLimit;
        for(int i = 0; i < _extraScores; i++)
        {
            highScores.RemoveAt(highScores.Count - 1);
        }
    }
    */

    public void GameOver()
    {
        
        if(currentPlayer.playerScore > highScorePlayer.playerScore)
        {
            highScorePlayer = currentPlayer;
            SaveScore();
        }
        


        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    public void StartMain()
    {
        currentPlayer.playerScore = 0;
        LoadScore();

        Ball = GameObject.Find("Ball").GetComponent<Rigidbody>();
        ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        BestScoreText = GameObject.Find("BestScoreText").GetComponent<Text>();
        GameOverText = GameObject.Find("GameOverText");

        ScoreText.text = $"{currentPlayer.playerName}'s Score: {currentPlayer.playerScore}";
        BestScoreText.text = $"Best Score: \n {highScorePlayer.playerName}: {highScorePlayer.playerScore}";
        GameOverText.SetActive(false);

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };

        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    void SaveScore()
    {
        string json = JsonUtility.ToJson(highScorePlayer);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);

    }

    void LoadScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            highScorePlayer = data;
        }
        
    }

    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        public int playerScore;
    }
}
