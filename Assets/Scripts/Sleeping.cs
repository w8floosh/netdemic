using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InterfaceComponent;

[Serializable]
public class Sleeping : NodeState
{
    public void Setup(NodeState lastStatus, float cooldown)
    {
        Destroy(this, cooldown);
        gameObject.GetComponent<Node>().Status = lastStatus;
        lastStatus.enabled = true;
    }

    override public void SendPackage(Encounter mEncounter, Package toSend)
    {
        throw new NotSupportedException("The node is shut down, thus it cannot send or receive messages.");
    }

    override public void RcvPackage(Package toSend)
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
