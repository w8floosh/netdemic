using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Waveform;

[Serializable]
public class Package : MonoBehaviour
{
    public enum TypeList : byte
    {
        Secure,
        Infected
    }
    private string _source;
    public string Source
    {
        get { return _source; }
        private set { _source = value; }
    }
    private string _destination;
    public string Destination {
        get { return _destination; }
        private set { _destination = value; }
    }
    private short _destinationRegionID;
    public short DestinationRegionID {
        get { return _destinationRegionID; }
        private set { _destinationRegionID = value; }
    }
    //public byte Type                    { get; private set; }
    private byte _ttl;
    public byte TTL {
        get { return _ttl; } 
        private set { _ttl = value; }
    }

    // public string NextHop            { get; private set; }
    private Waveform _payload;
    public Waveform Payload {
        get { return _payload; } 
        private set { _payload = value; }
    }

    public Package(string source, string destination, short destinationRegionID, byte ttl, Waveform payload)
    {
        Source = source;
        Destination = destination;
        DestinationRegionID = destinationRegionID;
        TTL = ttl;
        Payload = payload;
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

