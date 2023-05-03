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
        public bool IsPlaying => sound.IsPlaying();

        public NarratorAudioSet(FlowScene scene, SoundAbstract sound, bool canInterrupt)
        {
            this.scene = scene;
            this.sound = sound;
            this.canInterrupt = canInterrupt;
        }
    }

    [SerializeField]
    private NarratorAudioSet[] sets;

    [SerializeField]
    Queue<NarratorAudioSet> currentSetQueue = new Queue<NarratorAudioSet>();

    private NarratorAudioSet currentSet => currentSetQueue.Peek();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSetQueue.Count > 0)
        {
            if (!currentSet.IsPlaying)
            {
                currentSetQueue.Dequeue();
                if (currentSetQueue.Count > 0)
                {
                    currentSet.sound.PlayF();
                }
            }
        }
    }

    public void PlayAudio(FlowScene flowScene)
    {
        if (currentSet.IsPlaying)
        {
            currentSet.sound.Stop();
        }

        currentSet.sound.PlayF();
    }

    void QueueSets(FlowScene flowScene)
    {
        foreach (NarratorAudioSet set in sets)
        {
            if (set.scene.Equals(flowScene))
            {
                currentSetQueue.Enqueue(set);
            }
        }
    }
}