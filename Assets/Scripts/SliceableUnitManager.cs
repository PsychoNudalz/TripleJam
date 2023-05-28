using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceableUnitManager : MonoBehaviour
{
    [SerializeField]
    private Transform zone;
    
    [SerializeField]
    private List<SliceableUnit> sliceableUnits;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (!zone)
        {
            zone = transform;
        }

        sliceableUnits = new List<SliceableUnit>(GetComponentsInChildren<SliceableUnit>());
        foreach (SliceableUnit sliceableUnit in sliceableUnits)
        {
            sliceableUnit.MovementZone = zone;
        }
    }

    public void Remove(SliceableUnit unit)
    {
        sliceableUnits.Remove(unit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
