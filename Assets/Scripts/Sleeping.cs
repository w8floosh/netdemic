using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InterfaceComponent;

[Serializable]
public class Sleeping : NodeState
{
    public WaitForSeconds Cooldown;
    public byte CooldownTime;
    private Queue<Package> _packageQueue;
    public Queue<Package> PackageQueue
    {
        get { return _packageQueue; }
        private set { _packageQueue = value; }
    }

    public Sleeping(byte cooldownTime)
    {
        CooldownTime = cooldownTime;
        _packageQueue = new Queue<Package>();
    }
    override public void SendBundle(Encounter mEncounter, Package toSend)
    {
        throw new NotSupportedException("The node is shut down, thus it cannot send or receive messages.");
    }

    override public void RcvBundle(Package toSend)
    {
        throw new NotSupportedException("The node is shut down, thus it cannot send or receive messages. " +
                                        "Dropping package from (" + toSend.Destination + ", " + toSend.DestinationRegionID + ")...");
    }


    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
