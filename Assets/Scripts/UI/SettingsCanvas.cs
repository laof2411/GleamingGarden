using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsCanvas : MonoBehaviour
{
    //[SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundEffectsSlider;
    [SerializeField] Slider uiSlider;

    //[SerializeField] UIText languageText;

    private void OnEnable()
    {
        //AudioManager.instance.GetAudioMixer().GetFloat("Master", out float masterValue);
        //masterSlider.value = Mathf.Pow(10f,masterValue);


    }

    private void Start()
    {
        AudioManager.instance.GetAudioMixer().GetFloat("Music", out float musicValue);
        musicSlider.value = Mathf.Pow(10f, musicValue);

        AudioManager.instance.GetAudioMixer().GetFloat("SoundEffects", out float soundEffectsValue);
        soundEffectsSlider.value = Mathf.Pow(10f, soundEffectsValue);

        AudioManager.instance.GetAudioMixer().GetFloat("UI", out float uiValue);
        uiSlider.value = Mathf.Pow(10f, uiValue);

        //masterSlider.onValueChanged.AddListener(SetMaster);
        musicSlider.onValueChanged.AddListener(SetMusic);
        soundEffectsSlider.onValueChanged.AddListener(SetSoundEffects);
        uiSlider.onValueChanged.AddListener(SetUI);
    }

    public void SetMaster(float value)
    {
        AudioManager.instance.ChangeVolume(AudioType.Master, value);
        AudioManager.instance.PlayAudio("BushRustle");
    }

    public void SetMusic(float value)
    {
        AudioManager.instance.ChangeVolume(AudioType.Music, value);
        AudioManager.instance.PlayAudio("BushRustle");
    }

    public void SetSoundEffects(float value)
    {
        AudioManager.instance.ChangeVolume(AudioType.SoundEffect, value);
        AudioManager.instance.PlayAudio("BushRustle");
    }

    public void SetUI(float value)
    {
        AudioManager.instance.ChangeVolume(AudioType.UI, value);
        AudioManager.instance.PlayAudio("BushRustle");
    }

    //public void ChangeLanguage()
    //{
    //    if (TextManager.instance.currentLanguage == Language.en)
    //    {
    //        TextManager.instance.ChangeLanguage(Language.es);
    //        languageText._textID = "spanish";
    //        languageText.ForceUpdate();
    //    }
    //    else
    //    {
    //        TextManager.instance.ChangeLanguage(Language.en);
    //        languageText._textID = "english";
    //        languageText.ForceUpdate();
    //    }

    //    AudioManager.instance.PlayAudio("UIButton");
    //}

    public void PlayAudioSound()
    {
        AudioManager.instance.PlayAudio("UIButton");
    }
}
