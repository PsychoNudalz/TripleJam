using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeUpgrade : MonoBehaviour
{
    [SerializeField]
    private SliceLevel sl;
    public void SetBladeLevel()
    {
        SliceBlade.SetPlayerLevel(sl);
    }
}
