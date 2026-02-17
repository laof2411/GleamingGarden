using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject backgroundPanel;
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    public int goal;
    public int moves;
    public int points;
    public int time;

    public bool isGameEnded;

    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI movesText;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        pointsText.text = "Points: " + points.ToString();
        movesText.text = "Moves: " + moves.ToString();

        goalText.text = "Goal: " + goal.ToString();
    }

    public void Initialize(int _moves, int _goal)
    {
        moves = _moves;
        goal = _goal;
    }

    public void ProcessTurn(int _pointsToGain, bool _substractMoves)
    {
        points += _pointsToGain;
        if (_substractMoves)
        {
            moves--;
        }

        if(points >= goal)
        {
            isGameEnded = true;
            backgroundPanel.SetActive(true);
            victoryPanel.SetActive(true);
            return;
        }
        else if(moves == 0)
        {
            isGameEnded = true;
            backgroundPanel.SetActive(true);
            defeatPanel.SetActive(true);
            return;
        }
    }

    public void WinGame()
    {
        SceneManager.LoadScene(0);
    }

    public void LoseGame()
    {
        SceneManager.LoadScene(0);
    }
}
