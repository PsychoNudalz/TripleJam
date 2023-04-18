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

    private Material material;

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
        StartCoroutine(DelayUpdateTexture(index));
        headImages[index] = newHead;
        if (index > 0)
        {
            Debug.Log($"New image better: {headImages[index].Score > headImages[index - 1].Score}");
        }

        index++;
        index %= headImages.Count;
    }

    IEnumerator DelayUpdateTexture(int i)
    {
        yield return new WaitForFixedUpdate();
        material.SetTexture("_MainTex", headImages[i].Face);
        material.SetTexture("_Texture_0", headImages[i].Face);
        material.SetTexture("_Texture_1", headImages[i].Face);
        material.SetTexture("_Texture_2", headImages[i].Face);
        material.SetTexture("_Texture_3", headImages[i].Face);
    }
}