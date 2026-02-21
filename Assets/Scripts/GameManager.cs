using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public LevelData currentLevelData;

    public bool[] levelsCompleted;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        FindAnyObjectByType<BlackCanvas>().MakeMaskSmall();
    }

    public void PlayLevel()
    {
        AudioManager.instance.PlayAudio("Scene Change");
        
        FindAnyObjectByType<BlackCanvas>().MakeMaskBig();
        StartCoroutine(SceneChange(1));
    }

    public void BackToLevelSelection()
    {
        AudioManager.instance.PlayAudio("Scene Change");

        FindAnyObjectByType<BlackCanvas>().MakeMaskBig();
        StartCoroutine(SceneChange(0));
    }

    private IEnumerator SceneChange(int _sceneNumber)
    {
        yield return new WaitForSeconds(1f);

        AudioManager.instance.StopAudio("LevelSelection");
        AudioManager.instance.StopAudio("Gameplay");
        SceneManager.LoadScene(_sceneNumber);
    }
}
