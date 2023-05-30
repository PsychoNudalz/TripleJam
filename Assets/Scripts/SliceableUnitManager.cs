using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceableUnitManager : MonoBehaviour
{
    [SerializeField]
    private Transform zone;
    
    [SerializeField]
    private List<SliceableUnit> sliceableUnits;

    [SerializeField]
    private GameFlowManager flowManager;
    
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
        if (sliceableUnits.Contains(unit))
        {
            sliceableUnits.Remove(unit);
            if (sliceableUnits.Count == 0)
            {
                flowManager?.AllBugsCleared();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
