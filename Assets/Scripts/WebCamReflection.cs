using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCamReflection : MonoBehaviour
{
    [SerializeField]
    private Renderer renderer;

    private Material material;
    private WebCamTexture webCamTexture;

    private void Awake()
    {
        
        if (!GetComponent<Renderer>())
        {
            renderer = GetComponent<Renderer>();
        }

        material = GetComponent<Renderer>().material;
    
    }

    // Start is called before the first frame update
    void Start()
    {
        webCamTexture = WebCamManager.Texture;
        material.SetTexture("_Reflection",webCamTexture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
