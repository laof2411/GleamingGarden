using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public TextAsset textJSON;
    public TextReferenceWrapper wrapper;
    public Dictionary<string, string> textDictionary = new Dictionary<string, string>();

    public HashSet<ITranslatable> translatables = new HashSet<ITranslatable>();
    public static TextManager instance;
    public Language currentLanguage;


    public void Awake()
    {
        if(instance == null)
        {
            transform.SetParent(null);
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            ChangeLanguage(Language.es);
        }
        else
        {
            this.enabled = false;
        }     
    }

    public void ChangeLanguage(Language languageID)
    {
        currentLanguage = languageID;
        wrapper = JsonUtility.FromJson<TextReferenceWrapper>(textJSON.text);

        switch (languageID)
        {
            case Language.en:
                {
                    textDictionary = wrapper.texts.DistinctBy(item => item.text_id).ToDictionary(item => item.text_id, item => item.en);
                    break;
                }
            case Language.es:
                {
                    textDictionary = wrapper.texts.DistinctBy(item => item.text_id).ToDictionary(item => item.text_id, item => item.es);
                    break;
                }
        }
        TranslateTranslatables();
    }

    public void TranslateTranslatables()
    {       
        foreach (ITranslatable translatable in translatables) 
        {
            translatable.translatableText.text = textDictionary[translatable.textID];
        }
    }
}

[System.Serializable]
public class TextReferenceWrapper
{
    public TextReference[] texts;
}

[System.Serializable]
public class TextReference
{
    public string text_id;

    public string en;
    public string es;
}

[System.Serializable]
public class DialoguesList
{
    public Dialogue[] lines;
}
[System.Serializable]
public class Dialogue
{
    public string text_id;
    public string clue_key;
}

public enum Language
{
    en,
    es,
}
