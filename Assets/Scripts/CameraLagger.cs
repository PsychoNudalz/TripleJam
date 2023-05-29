using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraLagger : MonoBehaviour
{
   [SerializeField]
   private int setFPS = 15;
   RenderTexture m_SavedTexture;

   [ContextMenu("On")]
   public void OnLag_On()
   {
      Application.targetFrameRate = setFPS;
   }
   [ContextMenu("Off")]

   public void OnLag_Off()
   {
      Application.targetFrameRate = -1;
   }
}
