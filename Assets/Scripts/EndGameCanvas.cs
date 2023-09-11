using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highestScoreText;

    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float timeToDisplay;

    private float timer = 0;

    private bool gameEndedDisplay = false;

    private void OnEnable()
    {
        MyEvents.OnPlayerDeath += GameEnded;
        MyEvents.OnGetScore += GetScore;
    }
    private void OnDisable()
    {
        MyEvents.OnPlayerDeath -= GameEnded;
        MyEvents.OnGetScore -= GetScore;
    }

    private void Update()
    {
        if (!gameEndedDisplay || canvasGroup.alpha >= 1)
            return;

        timer += Time.deltaTime;

        canvasGroup.alpha = Mathf.InverseLerp(canvasGroup.alpha, timeToDisplay, timer);
    }
    private void GetScore(float score)
    {
        highestScoreText.text = "Highest Score: " + PlayerPrefs.GetFloat("HighestScore");
        scoreText.text = "Score: " + score;
    }
    private void GameEnded()
    {
        gameEndedDisplay = true;
        Cursor.visible = transform;
        Cursor.lockState = CursorLockMode.None;
    }
    public void PlayAgain() => SceneManager.LoadScene("PlayScene");
    public void QuitGame() => Application.Quit();

}
