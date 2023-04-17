using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SpriteLerp : MonoBehaviour
{
    [Header("Material")]
    [SerializeField]
    Texture[] textures;

    private Texture[] materialTextureQueue = new Texture[4];

    [FormerlySerializedAs("lerpValue_1")]
    [FormerlySerializedAs("lerpValue")]
    [SerializeField]
    [Range(0f, 1f)]
    float lerpValue_0 = 0f;

    [SerializeField]
    [Range(0f, 1f)]
    float lerpValue_1 = 0f;

    [FormerlySerializedAs("loopValue")]
    [SerializeField]
    private float loopValue_0 = 0;

    [SerializeField]
    private float loopValue_1 = 0;

    [Header("Settings")]
    [SerializeField]
    float lerpSpeed = 0.3f;

    [Header("Components")]
    [SerializeField]
    private Renderer renderer;

    [Header("Debug")]
    [SerializeField]
    private bool increaseLerp = true;

    private Material material;

    private void Awake()
    {
        if (!renderer)
        {
            renderer = GetComponent<Renderer>();
        }

        material = renderer.material;
    }

    void Start()
    {
        if (textures.Length <= 1)
        {
            enabled = false;
            Debug.LogError($"{gameObject} missing textures");
            return;
        }

        for (var i = 0; i < materialTextureQueue.Length; i++)
        {
            materialTextureQueue[i] = textures[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lerpValue_0 > 1f)
        {
            ShiftMaterialTexture(GetRandomTexture(),0,1);
            lerpValue_0 %= 1;
            loopValue_0++;
        }
        if (lerpValue_1 > 1f)
        {
            ShiftMaterialTexture(GetRandomTexture(),2,3);
            lerpValue_1 %= 1;
            loopValue_1++;
        }


        if (increaseLerp)
        {
            lerpValue_0 += Time.deltaTime * lerpSpeed;
            lerpValue_1 += Time.deltaTime * lerpSpeed;
        }

        material.SetFloat("_LerpValue", lerpValue_0);
        material.SetFloat("_LoopValue", loopValue_0);
        material.SetFloat("_LerpValue_1", lerpValue_1);
        material.SetFloat("_LoopValue_1", loopValue_1);
    }

    Texture GetRandomTexture()
    {
        Texture randomTexture = textures[Random.Range(0, textures.Length)];
        while (randomTexture.Equals(materialTextureQueue[0]))
        {
            randomTexture = textures[Random.Range(0, textures.Length)];
        }

        return randomTexture;
    }

    void ShiftMaterialTexture(Texture newTex, int newIndex, int oldIndex)
    {
        materialTextureQueue[oldIndex] = materialTextureQueue[newIndex];
        materialTextureQueue[newIndex] = newTex;
        material.SetTexture("_Texture_" + oldIndex, materialTextureQueue[oldIndex]);
        material.SetTexture("_Texture_" + newIndex, materialTextureQueue[newIndex]);
    }

    void ShiftMaterialTexture(Texture newTex)
    {
        materialTextureQueue[3] = materialTextureQueue[2];
        materialTextureQueue[2] = materialTextureQueue[1];
        materialTextureQueue[1] = materialTextureQueue[0];
        materialTextureQueue[0] = newTex;
        material.SetTexture("_Texture_1", materialTextureQueue[1]);
        material.SetTexture("_Texture_2", materialTextureQueue[2]);
        material.SetTexture("_Texture_3", materialTextureQueue[3]);
    }
}