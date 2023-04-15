using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Awake()
    {
        if (!playerMovement)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
