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

    public const int MAX_WORD = 98;

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


    public static string ConvertDialogue(DialogueSet dialogueSet)
    {
        return ConvertText(dialogueSet.text, dialogueSet.Length);
    }

    public static string ConvertText(string originalText,float duration)
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
             durationPerSection = duration * Mathf.Max(MAX_WORD / originalText.Length,1f);
        }

        for (int i = 0; i < originalText.Length; i++)
        {
            line += originalText[i];
            if (i % MAX_WORD == MAX_WORD - 1)
            {
                splitSubtitle.Add(new SubtitleText(line,durationPerSection));
                line = "";
            }
        }
        splitSubtitle.Add(new SubtitleText(line,durationPerSection));

        string json = JsonHelper.ToJson(splitSubtitle.ToArray(),true);
        Debug.Log(json);
        return json;
    }
}