using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("BladeUnlock")]
    [SerializeField]
    private GameObject bladeUnlock;

    [SerializeField]
    private TextMeshProUGUI bladeUnlockText;

    [Header("Score")]
    [SerializeField]
    private GameObject scoreZone;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    [Header("Boss Health Bar")]
    [SerializeField]
    private GameObject healthBarZone;

    [SerializeField]
    private Image healthBar_Image;

    private Material healthBar_Material;
    
    
    public static UIManager current;
    // Start is called before the first frame update


    private void Awake()
    {
        current = this;
        if (healthBar_Image)
        {
            healthBar_Material = healthBar_Image.material;
            
        }
        healthBarZone.SetActive(false);
    }

    void Start()
    {
        bladeUnlock.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateBladeText(string s)
    {
        bladeUnlock.SetActive(true);
        bladeUnlock.transform.localScale = new Vector3(1f, 1f, 1f);
        bladeUnlockText.text = s;
        LeanTween.scale(bladeUnlock, Vector3.zero, 2f);
    }

    public void UpdateScore(int i)
    {
        scoreText.text = i.ToString();
        
    }

    public void HideScore(bool b)
    {
        scoreZone.SetActive(!b);
    } 
}
