using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCamManager : MonoBehaviour
{
    
    
    private static WebCamManager current;

    private static WebCamTexture texture;

    public static WebCamTexture Texture => texture;

    // Start is called before the first frame update
    private void Awake()
    {
        current = this;
        texture = new WebCamTexture();
        texture.Play();
    }

    private void OnDestroy()
    {
        texture.Stop();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
