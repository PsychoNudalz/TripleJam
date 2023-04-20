using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TheVoiceController : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField]
    private VisualEffect vfx_Faces;

    [SerializeField]
    private Renderer mainRenderer;

    private Material faceMaterial;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFaceTexture(Texture[] faces,Texture bestFace)
    {
        for (int i = 0; i < Mathf.Min(5,faces.Length); i++)
        {
            if (faces[i])
            {
                vfx_Faces.SetTexture("Face_"+i,faces[i]);
            }
        }

        if (faceMaterial&&bestFace)
        {
            faceMaterial.SetTexture("_Face",bestFace);
        }
    }
}
