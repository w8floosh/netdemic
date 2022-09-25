using System;
using System.Collections.Generic;
using UnityEngine;
using static InterfaceComponent;

[Serializable]
public class Infected : MonoBehaviour, INodeState
{
    public readonly short InfectionSpeed;
    private Queue<Package> _packageQueue;

    public Infected(short speed)
    {
        InfectionSpeed = speed;
        _packageQueue = new Queue<Package>();
    }

    public void SendBundle(Encounter mEncounter, Package toSend)
    {
        for (int i = 0; i < UnityEngine.Random.Range(0, 25); i++)
        {
            mEncounter.Destination.Status.RcvBundle(toSend);
        }
    }

    public void RcvBundle(Package toSend)
    {
        UnityEngine.Debug.Log("butto il pacchetto proveniente da (" + toSend.Destination + ", " + toSend.DestinationRegionID + ")");
    }

}
