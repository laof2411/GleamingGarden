using UnityEngine;

public class AudioOnStart : MonoBehaviour
{
    [SerializeField] private string audioName;

    public void Start()
    {
        AudioManager.instance.PlayAudio(audioName);
    }
}
