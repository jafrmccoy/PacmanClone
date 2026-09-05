using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScoreReader : MonoBehaviour
{
    private int playerScore;

    [Header("Playing UI")]
    [SerializeField] private GameObject[] playingNonText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject[] gameOverNonText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private String lostMessage;
    [SerializeField] private String wonMessage;

    private bool gameOver;

    private void OnEnable()
    {
        ChomperController.ScoreIncreased += UpdateScore;
        ChomperController.OnLivesChanged += UpdateLives;
        ChomperController.OnPlayerLost += PlayerLost;

        PickupsCounter.OnPickupsGone += PlayerWon;
    }
    private void OnDisable()
    {
        ChomperController.ScoreIncreased -= UpdateScore;
        ChomperController.OnLivesChanged -= UpdateLives;
        ChomperController.OnPlayerLost -= PlayerLost;

        PickupsCounter.OnPickupsGone -= PlayerWon;
    }

    private void Start()
    {
        gameOver = false;
        playerScore = 0;
        scoreText.text = playerScore.ToString();
        EnablePlayingUI();
        DisableGameOverUI();
    }

    private void UpdateScore(ChomperController chomperController, int score)
    {
        playerScore += score;

        scoreText.text = playerScore.ToString();
    }

    private void UpdateLives(ChomperController champerController, int lives)
    {
        livesText.text = lives.ToString();
    }

    private void PlayerLost(ChomperController chomperController)
    {
        gameOver = true;
        DisablePlayingUI();
        gameOverText.text = lostMessage;
        finalScoreText.text = playerScore.ToString();
        EnableGameOverUI();
    }

    private void PlayerWon(PickupsCounter pickupsCounter)
    {
        gameOver = true;
        DisablePlayingUI();
        gameOverText.text = wonMessage;
        finalScoreText.text = playerScore.ToString();
        EnableGameOverUI();
    }

    private void EnablePlayingUI()
    {
        foreach (GameObject obj in playingNonText)
        {
            obj.SetActive(true);
        }
        scoreText.gameObject.SetActive(true);
        livesText.gameObject.SetActive(true);
    }

    private void DisablePlayingUI()
    {
        foreach(GameObject obj in playingNonText)
        {
            obj.SetActive(false);
        }
        scoreText.gameObject.SetActive(false);
        livesText.gameObject.SetActive(false);
    }

    private void EnableGameOverUI()
    {
        foreach (GameObject obj in gameOverNonText)
        {
            obj.SetActive(true);
        }
        finalScoreText.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        PauseGame();
    }

    private void DisableGameOverUI()
    {
        foreach (GameObject obj in gameOverNonText)
        {
            obj.SetActive(false);
        }
        finalScoreText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void UnpauseGame()
    {
        Time.timeScale = 1f;
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if (gameOver)
        {
            UnpauseGame();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
