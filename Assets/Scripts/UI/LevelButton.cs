using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Level Information")]
    [SerializeField] private int levelID;

    [Header("Level Reference")]
    [SerializeField] private Image levelImage;

    [Header("Sprites")]
    [SerializeField] private Sprite blockedLevel;
    [SerializeField] private Sprite unlockedLevel;
    [SerializeField] private Sprite completedLevel;

    private void Start()
    {
        LevelSetup();
    }

    private void LevelSetup()
    {
        if (GameManager.instance.levelsCompleted[levelID])
        {
            GetComponent<Button>().interactable = true;
            levelImage.sprite = completedLevel;
        }
        else if (levelID != 0)
        {
            if (GameManager.instance.levelsCompleted[levelID - 1])
            {
                GetComponent<Button>().interactable = true;
                levelImage.sprite = unlockedLevel;
            }
            else
            {
                GetComponent<Button>().interactable = false;
                levelImage.sprite = blockedLevel;
            }
        }
        else if (levelID == 0)
        {
            GetComponent<Button>().interactable = true;
            levelImage.sprite = unlockedLevel;
        }
        else
        {
            GetComponent<Button>().interactable = false;
            levelImage.sprite = blockedLevel;
        }
    }
}
