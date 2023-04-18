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
    private Renderer renderer;

    [SerializeField]
    private SpriteLerp spriteLerp;
    private Material material;
    private int lowestFaceIndex = 0;

    private void Awake()
    {
        if (!headDetector)
        {
            headDetector = GetComponent<HeadDetector>();
        }

        if (!renderer)
        {
            renderer = GetComponent<Renderer>();
        }

        material = renderer.material;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnAddPlayerHead()
    {
        HeadImage newHead = headDetector.TakePlayerPicture_HeadImage();
        // StartCoroutine(DelayUpdateTexture(index));

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

        spriteLerp.SetTextures(GetTextures());
        
        // headImages[index] = newHead;
        if (index > 0)
        {
            Debug.Log($"New image better: {headImages[index].Score > headImages[index - 1].Score}");
        }

        // index++;
        // index %= headImages.Capacity;
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