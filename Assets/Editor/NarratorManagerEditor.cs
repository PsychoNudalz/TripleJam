using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NarratorManager))]
public class NarratorManagerEditor : Editor
{
    private const string SavePath = "Meta Narration/Narrator/Data/";
    private const string AudioPath = "Meta Narration/Narrator/Clips/";
    private const string Filename = "DialogueData.json";

    public override void OnInspectorGUI()
    {
        NarratorManager myScript = (NarratorManager) target;


        if (GUILayout.Button("Initialise Text to Subtitles"))
        {
            myScript.InitialiseConvertTextToSubtitle();
            SetNarratorDirty();
        }

        if (GUILayout.Button("Reset Text to Subtitles"))
        {
            myScript.InitialiseConvertTextToSubtitle(true);
            SetNarratorDirty();
        }

        if (GUILayout.Button("Save JSON"))
        {
            SaveJSON(myScript);
        }

        if (GUILayout.Button("Load JSON"))
        {
            LoadJson(myScript);
            SetNarratorDirty();
        }

        DrawDefaultInspector();
    }

    public void SetNarratorDirty()
    {
        NarratorManager myScript = (NarratorManager) target;

        EditorUtility.SetDirty(myScript.gameObject);
    }

    public void SaveJSON(NarratorManager manager)
    {
        DialogueSet[] dialogueSets = manager.Sets;
        List<DialogueSet_JSON> dialogueSetJsons = new List<DialogueSet_JSON>();
        foreach (DialogueSet dialogueSet in dialogueSets)
        {
            dialogueSetJsons.Add(new DialogueSet_JSON(dialogueSet));
        }

        string json = JsonHelper.ToJson(dialogueSetJsons.ToArray(), true);
        Debug.Log($"Final JSON:\n{json}");
        FileLoader.CreateFileToResources(SavePath, Filename, json);
    }

    public void LoadJson(NarratorManager manager)
    {
        List<DialogueSet> sets = new List<DialogueSet>();
        DialogueSet_JSON[] jsons = FileLoader.LoadJSONArrayFromResources<DialogueSet_JSON>(SavePath + Filename);
        foreach (DialogueSet_JSON json in jsons)
        {
            FlowScene scene = (FlowScene) Enum.Parse(typeof(FlowScene), json.flowScene);
            AudioClip audioClip = GetAudioClip(json.audioClip);
            DialogueSet current = new DialogueSet(scene,
                audioClip, json.canInterrupt, json.text,
                JsonHelper.FromJson<SubtitleText>(json.subtitleTexts));
            sets.Add(current);
        }

        manager.SetDialogueSet(sets.ToArray());
    }

    AudioClip GetAudioClip(string name)
    {
        return FileLoader.LoadAudioClipFromResources(AudioPath + name);
    }
}

[System.Serializable]
public struct DialogueSet_JSON
{
    public string flowScene;
    public string audioClip;
    public bool canInterrupt;
    public string text;
    public string subtitleTexts;

    public DialogueSet_JSON(DialogueSet dialogueSet)
    {
        flowScene = dialogueSet.scene.ToString();
        audioClip = dialogueSet.Name;
        canInterrupt = dialogueSet.canInterrupt;
        text = dialogueSet.text;
        subtitleTexts = SubtitleManager.ConvertSubtitlesToJSON(dialogueSet.subtitleTexts);
    }
}

public struct Dialogue_JSON
{
    public DialogueSet_JSON[] dialogue;

    public Dialogue_JSON(DialogueSet_JSON[] dialogue)
    {
        this.dialogue = dialogue;
    }
}