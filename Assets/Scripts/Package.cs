using System;

[Serializable]
public class Package
{
    public enum TypeList : byte
    {
        Secure,
        Infected
    }
    private int _id;
    public int ID
    {
        get { return _id; }
        private set { _id = value; }
    }
    private string _source;
    public string Source
    {
        get { return _source; }
        private set { _source = value; }
    }
    private string _destination;
    public string Destination
    {
        get { return _destination; }
        private set { _destination = value; }
    }
    private short _destinationRegionID;
    public short DestinationRegionID
    {
        get { return _destinationRegionID; }
        private set { _destinationRegionID = value; }
    }
    private string _finalDestination;
    public string FinalDestination
    {
        get { return _finalDestination; }
        private set { _finalDestination = value; }
    }
    //public byte Type                    { get; private set; }
    private byte _ttl;
    public byte TTL
    {
        get { return _ttl; }
        private set { _ttl = value; }
    }

    // public string NextHop            { get; private set; }
    private Waveform _payload;
    public Waveform Payload
    {
        get { return _payload; }
        private set { _payload = value; }
    }
    public void SetNextHop(string nexthop)
    {
        Destination = nexthop;
    }
    public Package(int id, string source, string destination, short destinationRegionID, string finalDestination, byte ttl, Waveform payload)
    {
        _id = id;
        Source = source;
        Destination = destination;
        DestinationRegionID = destinationRegionID;
        FinalDestination = finalDestination;
        TTL = ttl;
        Payload = payload;
    }
}

