using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InterfaceComponent;

[Serializable]

public class Healthy : NodeState
{
    public byte ResistanceLimit;
    [SerializeField] private byte _infectionSteps;

    public void Setup(byte resistance) {
        ResistanceLimit = resistance;
        _infectionSteps = 0;
    }


    override public void SendPackage(Encounter encounter, Package toSend)
    {
        encounter.Destination.Status.RcvPackage(toSend);
    }

    override public void RcvPackage(Package p)
    {

    }

    void Awake()
    {
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
