using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionCanvas : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject levelInformationMenu;
    [SerializeField] private GameObject pauseMenu;

    [Header("Level Information")]
    [SerializeField] private TextMeshProUGUI levelTitleText;
    [SerializeField] private TextMeshProUGUI levelObjectiveText;
    [SerializeField] private TextMeshProUGUI levelMovementsText;
    [SerializeField] private TextMeshProUGUI levelTimeText;

    [Header("Level Sprites")]
    [SerializeField] private Image[] levels;

    [Header("External References")]
    [SerializeField] private LevelDataHolder levelDataHolderReference;

    [Header("Sprites")]
    [SerializeField] private Sprite completedLevel;
    [SerializeField] private Sprite blockedLevel;
    [SerializeField] private Sprite unlockedLevel;

    public void SelectLevel(string name)
    {
        GameManager.instance.currentLevelData = levelDataHolderReference.AccessLevelData(name);
        OpenLevelInformationScreen();
    }

    public void OpenLevelInformationScreen()
    {
        levelTitleText.text = GameManager.instance.currentLevelData.levelName;
        levelObjectiveText.text = GameManager.instance.currentLevelData.levelObjective + " Flowers";
        if (GameManager.instance.currentLevelData.levelMoves > 0)
        {
            levelMovementsText.text = GameManager.instance.currentLevelData.levelMoves + " Moves";
        }
        else
        {
            levelMovementsText.text = "N/A";
        }

        if (GameManager.instance.currentLevelData.levelTime > 0)
        {
            levelTimeText.text = GameManager.instance.currentLevelData.levelMoves + " Seconds";
        }
        else
        {
            levelTimeText.text = "N/A";
        }
        levelInformationMenu.SetActive(true);
        AudioManager.instance.PlayAudio("ButtonBubble");
    }

    public void CloseLevelInformation()
    {
        levelInformationMenu.SetActive(false);
        AudioManager.instance.PlayAudio("ButtonBubble");
    }

    public void OpenPause()
    {
        pauseMenu.SetActive(true);
        AudioManager.instance.PlayAudio("ButtonBubble");
    }

    public void ClosePause()
    {
        pauseMenu.SetActive(false);
        AudioManager.instance.PlayAudio("ButtonBubble");
    }

    public void PlayLevel()
    {
        GameManager.instance.PlayLevel();
    }
}
