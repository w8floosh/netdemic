using System.Collections.Generic;
using UnityEngine;
using static InterfaceComponent;
using static EncounterComponent;
using static BundleComponent;
using static NodeComponent;
using static SimulationManager;

public class StateManager : MonoBehaviour
{
    public class Gateway : INodeState
    {
        private Queue<Bundle> BundleQueue;
        private List<Gateway> ReachableGatewaysList;
        private Dictionary<string, byte[]> ReachableNodeAddressDictionary;
        private readonly int Capacity;
        private readonly Region RegionData;

        public Gateway(int capacity, Region region)
        {
            this.Capacity = capacity;
            this.BundleQueue = new Queue<Bundle>(capacity);
            this.ReachableGatewaysList = new List<Gateway>();
            this.RegionData = region;
        }

        private byte[] DNSResolve(string name)
        {
            return ReachableNodeAddressDictionary.GetValueOrDefault(name); // prova a trovare una corrispondenza tra il nome e 
        }

        public void SendBundle(IEncounter encounter)
        {
            if (BundleQueue.Count != 0)
            {
                Bundle NextBundle = BundleQueue.Dequeue();
                string RealAddress = DNSResolve(NextBundle.DstName);
                Bundle NewBundle = new Bundle(NextBundle.SrcName, RealAddress, this.RegionID, "data", NextBundle.TTL, NextBundle.Payload);
                encounter.Destination.Behaviour.RcvBundle(encounter, NewBundle);
            }
        }

        public void RcvBundle(IEncounter encounter, Bundle bundle)
        {
            BundleQueue.Enqueue(bundle);
        }
        
        public void AddToDictionary(Node node, byte[] address)
        {
            ReachableNodeAddressDictionary.Add(node.Name, address);
        }
    }

    public class Infected : INodeState
    {
        public readonly short InfectionSpeed;

        public Infected(short speed) => InfectionSpeed = speed;

        public void SendBundle(IEncounter mEncounter)
        {
            Bundle NewBundle = new Bundle(mEncounter.Source.Name, mEncounter.Destination.Name, mEncounter.Destination.RegionID, "virus", Random.Range(4, 16), mEncounter.Source.getTone());
            mEncounter.Destination.Behaviour.RcvBundle(mEncounter, NewBundle);
        }

        public void RcvBundle(IEncounter encounter, Bundle bundle)
        {
            // buttalo via
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
