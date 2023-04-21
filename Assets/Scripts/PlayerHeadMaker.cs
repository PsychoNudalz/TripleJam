using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadMaker : MonoBehaviour
{
    [SerializeField]
    private HeadDetector headDetector;

    [SerializeField]
    private List<HeadImage> headImages = new List<HeadImage>(5);

    [SerializeField]
    private int index = 0;


    [SerializeField]
    private SpriteLerp[] spriteLerps;

    [SerializeField]
    private TheVoiceController voice;

    private int lowestFaceIndex = 0;
    private int highestFaceIndex = 0;

    public Texture BestFace => headImages[highestFaceIndex].Face;

    private void Awake()
    {
        if (!headDetector)
        {
            headDetector = GetComponent<HeadDetector>();
        }
    }

    private void Start()
    {
        // FaceInit();
    }


    private IEnumerator DelayFaceInitialise()
    {
        yield return new WaitForSeconds(Time.fixedDeltaTime);
        for (int i = 0; i < headImages.Capacity; i++)
        {
            OnCapturePlayerHead();
            Debug.Log($"Face done: {i} ");
            yield return new WaitForSeconds(Time.fixedDeltaTime);

        }

        UpdateSprites();
    }

    public void FaceInit()
    {
        StartCoroutine(DelayFaceInitialise());
    }

    public void OnAddPlayerHead()
    {
        OnCapturePlayerHead();
        UpdateSprites();

        // headImages[index] = newHead;


        // index++;
        // index %= headImages.Capacity;
    }

    public void OnCapturePlayerHead()
    {
        HeadImage newHead = headDetector.TakePlayerPicture_HeadImage();
        if (newHead.Score < 0)
        {
            Debug.LogWarning("Head generate score < 0");
            return;
        }
        // StartCoroutine(DelayUpdateTexture(index));
        Debug.Log("Head generated");
        if (index < headImages.Capacity)
        {
            headImages[index] = newHead;
            index++;
        }
        else
        {
             highestFaceIndex = 0;
            float highest = 0;
            float lowest = 100;
            headImages[lowestFaceIndex] = newHead;

            //locate the smallest and biggest
            for (var i = 0; i < headImages.Count; i++)
            {
                var headImage = headImages[i];
                if (headImage.Score > highest)
                {
                    highestFaceIndex = i;
                    highest = headImage.Score;
                }

                if (headImage.Score < lowest)
                {
                    lowestFaceIndex = i;
                    lowest = headImage.Score;
                }
            }
        }

        // if (index > 0)
        // {
        //     Debug.Log($"New image better: {headImages[index].Score > headImages[index - 1].Score}");
        // }
    }

    public void UpdateSprites()
    {
        foreach (SpriteLerp spriteLerp in spriteLerps)
        {
            spriteLerp.SetTextures(GetTextures());
        }

        voice.SetFaceTexture(GetTextures().ToArray(),BestFace);

        Debug.Log("Sprites updated");
    }

    List<Texture> GetTextures()
    {
        List<Texture> t = new List<Texture>();
        foreach (HeadImage headImage in headImages)
        {
            t.Add(headImage.Face);
        }

        return t;
    }
}