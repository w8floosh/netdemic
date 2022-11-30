using System;
using System.Collections;
using UnityEngine;
using static InterfaceComponent;

[Serializable]

public class Healthy : NodeState
{
    public byte InfectionSteps;
    private int _lastPacketReceived;


    override public void SendPackage(Encounter encounter, Package toSend)
    {
        StartCoroutine(TransmissionCoroutine(encounter, toSend));
       // encounter.Destination.Status.RcvPackage(toSend);
    }

    override public void RcvPackage(Package p)
    {
        if (p.FinalDestination == gameObject.name)
        {
            Debug.Log(gameObject.name + " received a packet");
            return;
        }
        if (p.ID > _lastPacketReceived)
        {
            gameObject.GetComponent<Node>().PackageEnqueue(p);
            _lastPacketReceived = p.ID;
        }
        //Debug.Log("(" + gameObject.name + ", regionID: " + gameObject.GetComponent<Node>().RegionData.RegionID + ") received package " +
            //"(ID: " + p.ID + ") from (" + p.Source + ")");
    }
    private IEnumerator TransmissionCoroutine(Encounter encounter, Package toSend, float delay = 0)
    {
        encounter.IsBusy = true;
        if (delay == 0) yield return null;
        else yield return new WaitForSeconds(delay);
        //Debug.Log(encounter.Destination.Name + " received package " + toSend.ID + " after " + (delay * 100) + "ms");
        encounter.Destination.Status.RcvPackage(toSend);
        encounter.IsBusy = false;
    }
    void Awake()
    {
        InfectionSteps = 0;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
