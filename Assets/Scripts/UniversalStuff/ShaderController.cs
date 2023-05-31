using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderController : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    private float value;

    [SerializeField]
    private string controlField = "_Alpha";

    [SerializeField]
    private RawImage image;

    private Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = image.material;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        material.SetFloat(controlField,value);
    }
}
