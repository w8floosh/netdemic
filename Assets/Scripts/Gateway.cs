using System;
using System.Collections.Generic;
using static InterfaceComponent;

[Serializable]
public class Gateway : NodeState
{
    private List<Gateway> _reachableGatewaysList;
    private Dictionary<string, byte[]> _reachableNodeAddressDictionary;
    private readonly Region _regionData;

    public Gateway(Region region)
    {
        this._reachableGatewaysList = new List<Gateway>();
        this._regionData = region;
        _reachableNodeAddressDictionary = new Dictionary<string, byte[]>();
    }

    private byte[] DNSResolve(string name)
    {
        return _reachableNodeAddressDictionary.GetValueOrDefault(name); // prova a trovare una corrispondenza col nome nel dizionario del gateway
    }

    override public void SendBundle(Encounter encounter, Package bundle = null)
    {
        /*if (encounter.Source.BundleQueue.Count != 0)
        {
            Bundle dequeued = BundleQueue.Dequeue();
            byte[] realAddress = DNSResolve(dequeued.Destination);
            IEqualityComparer<byte[]> comparer = EqualityComparer<byte[]>.Default;

            if (!comparer.Equals(realAddress, default(byte[])))
            {
                encounter.Destination.Status.RcvBundle(dequeued);
            }
                
        }*/
    }

    override public void RcvBundle(Package bundle)
    {
        //BundleQueue.Enqueue(bundle);
    }

    public void AddToDictionary(Node node, byte[] address)
    {
        _reachableNodeAddressDictionary.Add(node.Name, address);
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