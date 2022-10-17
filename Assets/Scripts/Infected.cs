using Codice.CM.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using static InterfaceComponent;

[Serializable]
public class Infected : NodeState
{
    [SerializeField] private float _infectionSpeed;

    override public void SendPackage(Encounter mEncounter, Package toSend)
    {
        for (int i = 0; i < UnityEngine.Random.Range(0, 25); i++)
        {
            mEncounter.Destination.Status.RcvPackage(toSend);
        }
    }

    override public void RcvPackage(Package toSend)
    {
        UnityEngine.Debug.Log("butto il pacchetto proveniente da (" + toSend.Destination + ", " + toSend.DestinationRegionID + ")");
    }
    public void Setup(short infectionSpeed)
    {
        _infectionSpeed = infectionSpeed;
    }
    void Awake()
    {
        _infectionSpeed = 1;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }
}
