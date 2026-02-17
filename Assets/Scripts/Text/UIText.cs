using TMPro;
using UnityEngine;

public class UIText : MonoBehaviour, ITranslatable
{
    [SerializeField] public string _textID;
    [SerializeField] private TextMeshProUGUI _translatableText;

    public string textID
    {
        get { return _textID; }
        set
        {
            textID = value;
        }
    }

    public TextMeshProUGUI translatableText
    {
        get { return _translatableText; }
        set
        {
            translatableText = value;
        }
    }

    public void OnEnable()
    {
        FindAnyObjectByType<TextManager>().translatables.Add(this);
        translatableText.text = FindAnyObjectByType<TextManager>().textDictionary[textID];
    }

    public void OnDisable()
    {
        if(TextManager.instance != null)
        {
            TextManager.instance.translatables.Remove(this);
        }
    }

    public void ForceUpdate()
    {
        translatableText.text = TextManager.instance.textDictionary[textID];
    }
}
