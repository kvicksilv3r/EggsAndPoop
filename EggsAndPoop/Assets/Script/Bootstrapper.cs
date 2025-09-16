using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bootstrapper : MonoBehaviour
{
    public UnityEvent bootstrapEvents;

    private void Start()
    {
        bootstrapEvents.Invoke();
    }
    //TODO REBUILD ENTIRE BOOT SEQUENCE
    //TODO REBUILD ANIMAL DATA SAVE DATA
}
