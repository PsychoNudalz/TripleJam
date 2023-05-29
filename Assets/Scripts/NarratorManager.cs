using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public struct DialogueSet
{
    public FlowScene scene;

    [FormerlySerializedAs("sound")]
    public AudioClip clip;

    public bool canInterrupt;

    [TextArea(2, 1000)]
    public string text;

    public SubtitleText[] subtitleTexts;

    public bool IsActive => !scene.Equals(FlowScene.None);

    // public bool IsPlaying => IsActive && sound.IsPlaying();
    public string Name => clip.name;
    public float Length => GetLength();

    public float GetLength()
    {
        if (clip)
        {
            return clip.length;
        }

        return 0;
    }
    
    public DialogueSet(FlowScene scene = FlowScene.None, AudioClip clip = null, bool canInterrupt = false,
        string text = "", SubtitleText[] subtitleTexts = default)
    {
        this.scene = scene;
        this.clip = clip;
        this.canInterrupt = canInterrupt;
        this.text = text;
        this.subtitleTexts = subtitleTexts;
    }

    public override bool Equals(object obj)
    {
        if (obj is DialogueSet n)
        {
            if (n.scene.Equals(scene))
            {
                return true;
            }
        }

        if (obj is FlowScene o)
        {
            if (o.Equals(scene))
            {
                return true;
            }
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class NarratorManager : MonoBehaviour
{
    [SerializeField]
    private DialogueSet[] sets;

    [SerializeField]
    Queue<DialogueSet> currentSetQueue = new Queue<DialogueSet>();

    [SerializeField]
    private SoundAbstract narratorSound;

    private DialogueSet currentSet;

    public DialogueSet[] Sets => sets;


    public void SetDialogueSet(DialogueSet[] s)
    {
        Debug.Log($"Setting set with length: {s.Length}");
        sets = s;
    }

    // Start is called before the first frame update
    void Awake()
    {
        currentSet = new DialogueSet();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentSet.IsActive)
        {
            if (!narratorSound.IsPlaying())
            {
                if (currentSetQueue.Count > 0)
                {
                    PlayNextLine();
                }
            }
        }
    }

    public float PlayAudio(FlowScene flowScene)
    {
        CheckEndCurrentAudio();

        QueueSets(flowScene);
        if (currentSetQueue.Count > 1)
        {
            return currentSet.Length;
        }

        return PlayNextLine();
    }

    private float PlayNextLine()
    {
        if (currentSetQueue.Count == 0)
        {
            return 0;
        }

        currentSet = currentSetQueue.Dequeue();
        Debug.Log($"Narrator playing {currentSet.Name}");

        narratorSound.SetClip(currentSet.clip);
        narratorSound.PlayF();

        SubtitleManager.ResetAdd(currentSet.subtitleTexts);

        return currentSet.Length;
    }

    private bool CheckEndCurrentAudio()
    {
        if (narratorSound.IsPlaying())
        {
            if (currentSet.canInterrupt)
            {
                Stop();
                return false;
            }

            return true;
        }

        return false;
    }

    void QueueSets(FlowScene flowScene)
    {
        foreach (DialogueSet set in sets)
        {
            if (set.Equals(flowScene))
            {
                currentSetQueue.Enqueue(set);
            }
        }

        Debug.Log($"Narrator Queue size: {currentSetQueue.Count}");
    }

    public void Stop()
    {
        if (narratorSound.IsPlaying())
        {
            narratorSound.Stop();
            currentSetQueue = new Queue<DialogueSet>();
        }
        SubtitleManager.Reset();

    }

    [ContextMenu("ConvertDialogue")]
    public void InitialiseConvertTextToSubtitle(bool force = false)
    {
        for (var i = 0; i < sets.Length; i++)
        {
            DialogueSet dialogueSet = sets[i];
            if (force || dialogueSet.subtitleTexts.Length == 0)
            {
                sets[i] = SubtitleManager.ConvertAndAddDialogue(dialogueSet);
            }
        }
    }
}