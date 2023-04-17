using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpriteLerp : MonoBehaviour
{
    [Header("Material")]
    [SerializeField]
    Texture[] textures;

    private Texture[] materialTextureQueue = new Texture[4];
    [SerializeField]
    [Range(0f,1f)]
    float lerpValue = 0f;

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

        if (lerpValue > 1f)
        {
            ShiftMaterialTexture(GetRandomTexture());
            lerpValue = 0;
        }

        if (increaseLerp)
        {
            lerpValue += Time.deltaTime * lerpSpeed;
        }

        material.SetFloat("_LerpValue",lerpValue);

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

    void ShiftMaterialTexture(Texture newTex)
    {
        materialTextureQueue[3] = materialTextureQueue[2];
        materialTextureQueue[2] = materialTextureQueue[1];
        materialTextureQueue[1] = materialTextureQueue[0];
        materialTextureQueue[0] = newTex;
        material.SetTexture("_Texture_1",materialTextureQueue[1]);
        material.SetTexture("_Texture_2",materialTextureQueue[2]);
        material.SetTexture("_Texture_3",materialTextureQueue[3]);
        material.mainTexture = materialTextureQueue[0];
    }
}
