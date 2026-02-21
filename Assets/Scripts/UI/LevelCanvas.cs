using TMPro;
using UnityEngine;

public class LevelCanvas : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] LevelManager levelManager;

    [Header("Menus")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject victoryScreen;
    [SerializeField] GameObject defeatScreen;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private TextMeshProUGUI levelText;
    private void Update()
    {
        pointsText.text = levelManager.points.ToString();
        movesText.text = levelManager.moves.ToString();
        goalText.text = levelManager.goal.ToString();
    }

    public void IntroduceInitialValues()
    {
        pointsText.text = levelManager.points.ToString();
        movesText.text = levelManager.moves.ToString();
        goalText.text = levelManager.goal.ToString();
        timeText.text = levelManager.time.ToString();

        levelText.text = GameManager.instance.currentLevelData.levelName;
    }

    public void UpdatePoints() 
    { 
        pointsText.text = levelManager.points.ToString();
    }

    public void UpdateLevelMoves()
    {
        if (GameManager.instance.currentLevelData.levelMoves > 0)
        {
            movesText.text = levelManager.moves.ToString();
        }
        else
        {
            movesText.text = "N/A";
        }
    }

    public void UpdateTime()
    {
        if (GameManager.instance.currentLevelData.levelTime > 0)
        {
            timeText.text = levelManager.time.ToString();
        }
        else
        {
            timeText.text = "N/A";
        }
    }

    public void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        levelManager.isPaused = true;
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        levelManager.isPaused = false;
    }

    public void RetryLevel()
    {
        GameManager.instance.PlayLevel();
    }

    public void BackToLevelSelection()
    {
        GameManager.instance.BackToLevelSelection();
    }

    public void OpenVictoryScreen()
    {
        victoryScreen.SetActive(true);
    }

    public void OpenDefeatScreen()
    {
        defeatScreen.SetActive(true);
    }
}
