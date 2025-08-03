using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public GameObject gameOverPanel;
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI timeSurvivedText;
    public bool Paused { get; private set; }
    private bool gameOver;

    private int enemiesKilled;
    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(Instance);
        
        gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        Button fightButton = gameOverPanel.GetComponentInChildren<Button>();
        fightButton?.onClick.AddListener(RestartScene);
    }

    private void Update()
    {
        if(!gameOver) return;
        
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            RestartScene();
    }

    [ContextMenu("Game Over")]
    public void GameOver()
    {
        Paused = true;
        gameOver = true;
        
        enemiesKilledText.text = enemiesKilled.ToString();
        timeSurvivedText.text = GetFormattedTime();
        gameOverPanel.SetActive(true);
        
        //enable cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private string GetFormattedTime()
    {
        float time = Time.time;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void EnemyDied()
    {
        enemiesKilled++;
    }
}
