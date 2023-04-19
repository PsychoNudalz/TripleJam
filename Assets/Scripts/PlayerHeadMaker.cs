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

    private int lowestFaceIndex = 0;

    private void Awake()
    {
        if (!headDetector)
        {
            headDetector = GetComponent<HeadDetector>();
        }
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

    private void OnCapturePlayerHead()
    {
        HeadImage newHead = headDetector.TakePlayerPicture_HeadImage();
        // StartCoroutine(DelayUpdateTexture(index));
        Debug.Log("Head generated");

        int highestIndex = 0;
        float highest = 0;
        float lowest = 100;

        headImages[lowestFaceIndex] = newHead;


        //locate the smallest and biggest
        for (var i = 0; i < headImages.Count; i++)
        {
            var headImage = headImages[i];
            if (headImage.Score > highest)
            {
                highestIndex = i;
                highest = headImage.Score;
            }

            if (headImage.Score < lowest)
            {
                lowestFaceIndex = i;
                lowest = headImage.Score;
            }
        }

        if (index > 0)
        {
            Debug.Log($"New image better: {headImages[index].Score > headImages[index - 1].Score}");
        }
    }

    private void UpdateSprites()
    {
        foreach (SpriteLerp spriteLerp in spriteLerps)
        {
            spriteLerp.SetTextures(GetTextures());
        }

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