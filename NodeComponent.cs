using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InterfaceComponent;
using static SimulationManager;
using static EncounterComponent;
using static StateManager;
using System.Security.Cryptography;
using System;
using System.Text;
using System.Linq;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;

public class NodeComponent : MonoBehaviour
{
    public class Node : INode
    {
        public string Name          { get; }
        public Region RegionData    { get; }
        public short Power          { get; }
        public float EnergyLevel    { get; }
        public INodeState Behaviour {
            get { return Behaviour; }
            private set { 
                Behaviour = value; 
                // altre operazioni necessarie
            } 
        }
        public short MaxConnections { get; }
        private Waveform Tone;
        private readonly byte[] PhysicalAddress;
        private List<IEncounter> CurrentEncounters;
        private List<Node> RegionGateways;

        [Range(0.01f, 0.1f)]    private readonly float EnergyDrainRatePerCycle;                 // percentuale di batteria da scaricare dopo ogni duty cycle
        [Range(0.01f, 0.1f)]    private readonly float ChargingRatePerSecond;                   // percentuale di batteria ricaricata ogni secondo
        //[Range(3,10)]           private readonly float CooldownTime;                             tempo massimo di stop dell'attività per far tornare il nodo al 100% di batteria, va messo nello stato Sleeping
        [Range(10,1000)]        private readonly float DutyCycleDuration;                       // (in ms) tempo di operatività effettiva in un secondo
        

        public Node(string name, Region region, short power, INodeState firstState, short maxConnections)
        {
            Name                    = name;
            RegionData              = region;
            Power                   = power;
            Behaviour               = firstState;
            MaxConnections          = maxConnections;
            EnergyLevel             = 1.0f;
            EnergyDrainRatePerCycle = UnityEngine.Random.Range(0.01f, 0.1f);
            ChargingRatePerSecond   = UnityEngine.Random.Range(0.01f, 0.1f);
            //CooldownTime            = UnityEngine.Random.Range(3,10);                         // va messo nello stato Sleeping
            DutyCycleDuration       = UnityEngine.Random.Range(10,1000);
            Tone                    = new(RegionData);
            PhysicalAddress         = GeneratePhysicalAddress();
            RegionGateways          = new();
            foreach(var node in RegionData.NodeList)
            {
                if ((node.Behaviour as Gateway) != null)
                {
                    RegionGateways.Add(node);
                }
            }
            
        }


        private byte[] GeneratePhysicalAddress()                                                // questa funzione cripta in SHA256 una stringa di byte per generare un indirizzo fisico univoco
        {
            SHA256 encryptor        = SHA256.Create();                                          // creo un'istanza dell'algoritmo SHA256
            byte[] waveformBytes    = new byte[SampleRatePerSecond * sizeof(int)];             
            Buffer.BlockCopy(Tone.SamplesArray, 0, waveformBytes, 0, SampleRatePerSecond);      // copio i singoli byte dall'array dei campioni della waveform ORIGINALE
            return encryptor.ComputeHash(waveformBytes);                                        // computo l'hash e lo salvo in una variabile PhysicalAddress
        }

        public IEncounter EstablishConnection() { }

        public void ShowInfo() { }

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
