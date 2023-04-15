using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public void OnMove(InputValue inputValue)
    {
        Vector2 dir = inputValue.Get<Vector2>();
        if (dir.y > 0.5)
        {
            playerMovement.Move_Forward();
        }
    }
}
