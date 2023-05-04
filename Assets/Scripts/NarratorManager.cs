using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarratorManager : MonoBehaviour
{
    [Serializable]
    struct NarratorAudioSet
    {
        public FlowScene scene;

        public SoundAbstract sound;

        //Need to get interruption to work
        public bool canInterrupt;
        public bool IsActive => !scene.Equals(FlowScene.None);
        public bool IsPlaying => IsActive && sound.IsPlaying();
        public string Name => sound.GetClip().name;
        public float Length => sound.GetClip().length;


        public NarratorAudioSet(FlowScene scene = FlowScene.None, SoundAbstract sound = null, bool canInterrupt = false)
        {
            this.scene = scene;
            this.sound = sound;
            this.canInterrupt = canInterrupt;
        }

        public override bool Equals(object obj)
        {
            if (obj is NarratorAudioSet n)
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

    [SerializeField]
    private NarratorAudioSet[] sets;

    [SerializeField]
    Queue<NarratorAudioSet> currentSetQueue = new Queue<NarratorAudioSet>();

    private NarratorAudioSet currentSet;

    // Start is called before the first frame update
    void Awake()
    {
        currentSet = new NarratorAudioSet();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentSet.IsActive)
        {
            if (!currentSet.IsPlaying)
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

        currentSet.sound.PlayF();
        return currentSet.Length;
    }

    private bool CheckEndCurrentAudio()
    {
        if (currentSet.IsPlaying)
        {
            if (currentSet.canInterrupt)
            {
                currentSetQueue = new Queue<NarratorAudioSet>();
                currentSet.sound.Stop();
                return false;
            }

            return true;
        }

        return false;
    }

    void QueueSets(FlowScene flowScene)
    {
        foreach (NarratorAudioSet set in sets)
        {
            if (set.Equals(flowScene))
            {
                currentSetQueue.Enqueue(set);
            }
        }

        Debug.Log($"Narrator Queue size: {currentSetQueue.Count}");
    }
}