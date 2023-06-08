using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class SubtitleText
{
    public string text;
    public float time;

    public SubtitleText(string text, float time)
    {
        this.text = text;
        this.time = time;
    }
}

public class SubtitleManager : MonoBehaviour
{
    [SerializeField]
    private Queue<SubtitleText> subtitleQueue;


    [Header("UI")]
    [SerializeField]
    private GameObject subtitleGameObject;

    [SerializeField]
    private TextMeshProUGUI textBox;

    private Coroutine subtitleCoroutine;
    public static SubtitleManager current;

    public const int MAX_WORD = 125;

    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Subtitle_Start();
    }

    private void Subtitle_Start()
    {
        Subtitle_Stop();
        subtitleQueue = new Queue<SubtitleText>();
        subtitleCoroutine = StartCoroutine(SubtitleCoroutine());
    }

    private void Subtitle_Stop()
    {
        if (subtitleCoroutine != null)
        {
            StopCoroutine(subtitleCoroutine);
            subtitleCoroutine = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddQueue(SubtitleText subtitleText)
    {
        subtitleQueue.Enqueue(subtitleText);
    }

    private void OnEnable()
    {
        Subtitle_Start();
    }

    private void OnDisable()
    {
        Subtitle_Stop();
    }

    private void OnDestroy()
    {
        Subtitle_Stop();
    }


    IEnumerator SubtitleCoroutine()
    {
        SubtitleText current;
        while (true)
        {
            subtitleGameObject.SetActive(false);
            yield return new WaitUntil(() => subtitleQueue.Count > 0);
            subtitleGameObject.SetActive(true);

            current = subtitleQueue.Dequeue();
            Debug.Log($"Display: {current.text}\nTime: {current.time}");
            textBox.text = current.text;
            yield return new WaitForSeconds(current.time);
            Debug.Log($"Subtitle End");
        }
    }

    [ContextMenu("Test Add Subtitle")]
    public void Debug_AddSubtitle()
    {
        float t = Random.Range(2f, 3f);
        AddQueue(new SubtitleText("THIS IS A TEST TEXT for " + t.ToString(), t));
    }

    public static void Add(SubtitleText t)
    {
        current.AddQueue(t);
    }

    public static void Add(SubtitleText[] t)
    {
        foreach (SubtitleText subtitleText in t)
        {
            Add(subtitleText);
        }
    }

    public static void Reset()
    {
        current.Subtitle_Start();
    }

    public static void ResetAdd(SubtitleText[] t)
    {
        current.Subtitle_Start();
        Add(t);
    }

    public static DialogueSet ConvertAndAddDialogue(DialogueSet dialogueSet)
    {
        SubtitleText[] temp = ConvertTextToSubtitles(dialogueSet.text, dialogueSet.Length);
        Debug.Log($"Set subtitle on Dialogue");
        dialogueSet.subtitleTexts = temp;
        return dialogueSet;
    }

    public static SubtitleText[] ConvertTextToSubtitles(string originalText, float duration)
    {
        List<SubtitleText> splitSubtitle = new List<SubtitleText>();
        string line = "";
        float durationPerSection;
        if (originalText.Length == 0)
        {
            durationPerSection = duration;
        }
        else
        {
            durationPerSection = duration * Mathf.Min((float) MAX_WORD / (float) originalText.Length, 1f);
        }

        originalText = originalText.Replace("\n", " ");
        originalText = originalText.Replace("\r", "");

        string[] splitText = originalText.Split(" ");
        int lineCount = 0;

        
         // durationPerSection = Mathf.FloatToHalf(durationPerSection);


        foreach (string s in splitText)
        {
            if (lineCount + s.Length < MAX_WORD)
            {
                line += s + " ";
                lineCount += s.Length + 1;
            }
            else
            {
                splitSubtitle.Add(new SubtitleText(line, durationPerSection));
                line = s+ " ";
                lineCount = 0;
            }
        }


        // for (int i = 0; i < originalText.Length; i++)
        // {
        //     line += originalText[i];
        //     if (i % MAX_WORD == MAX_WORD - 1)
        //     {
        //         splitSubtitle.Add(new SubtitleText(line, durationPerSection));
        //         line = "";
        //     }
        // }

        if (splitSubtitle.Count > 0)
        {
            splitSubtitle.Add(new SubtitleText(line, Mathf.Max(0.5f, duration % durationPerSection)));
        }
        else
        {
            splitSubtitle.Add(new SubtitleText(line, duration));
        }

        return splitSubtitle.ToArray();
    }


    public static string ConvertSubtitlesToJSON(SubtitleText[] splitSubtitle)
    {
        string json = JsonHelper.ToJson(splitSubtitle);
        Debug.Log(json);
        return json;
    }
}