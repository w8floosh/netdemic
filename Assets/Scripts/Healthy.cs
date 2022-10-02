using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InterfaceComponent;

[Serializable]

public class Healthy : NodeState
{
    public readonly byte ResistanceLimit;
    public byte InfectionSteps;
    private Queue<Package> _incomingPackageQueue;
    public Queue<Package> IncomingPackageQueue
    {
        get { return _incomingPackageQueue; }
        private set { _incomingPackageQueue = value; }
    }
    private short _queueSize;
    public short QueueSize
    {
        get { return _queueSize; }
        private set
        {
            _queueSize = value;
        }
    }


    public Healthy(byte resistance, short queueSize)
    {
        ResistanceLimit = resistance;
        InfectionSteps = 0;
        QueueSize = queueSize;
        IncomingPackageQueue = new Queue<Package>(QueueSize);
    }

    override public void SendBundle(Encounter encounter, Package toSend = null)
    {
        Package NextBundle = _incomingPackageQueue.Dequeue();
        encounter.Destination.Status.RcvBundle(NextBundle);
    }

    override public void RcvBundle(Package bundle)
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
