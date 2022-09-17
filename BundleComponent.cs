using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Waveform;

public class BundleComponent : MonoBehaviour
{
    public class Bundle
    {
        public string Source                { get; private set; }
        public string Destination           { get; private set; }
        public short DestinationRegionID    { get; private set; }
        public byte Type                    { get; private set; }
        public byte TTL                     { get; private set; }

        // public string NextHop            { get; private set; }
        public Waveform Payload             { get; private set; }

        public Bundle(string source, string destination, short destinationRegionID, byte type, byte ttl, Waveform payload)
        {
            Source              = source;
            Destination         = destination;
            DestinationRegionID = destinationRegionID;
            Type                = type;
            TTL                 = ttl;
            Payload             = payload;
        }

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
