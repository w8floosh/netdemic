using System;
using System.Collections.Generic;
using static InterfaceComponent;

[Serializable]
public class Gateway : NodeState
{
    private List<Gateway> _reachableGatewaysList;
    private Dictionary<string, string> _reachableNodeAddressDictionary;
    private Region _regionData;

    public void Setup(Region regionData)
    {
        this._reachableGatewaysList = new List<Gateway>();
        this._regionData = regionData;
        _reachableNodeAddressDictionary = new Dictionary<string, string>();
    }

    private string DNSResolve(string name)
    {
        return _reachableNodeAddressDictionary.GetValueOrDefault(name); // prova a trovare una corrispondenza col nome nel dizionario del gateway
    }

    override public void SendPackage(Encounter encounter, Package bundle = null)
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

    override public void RcvPackage(Package bundle)
    {
        //BundleQueue.Enqueue(bundle);
    }

    public void AddToDictionary(Node node, string address)
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