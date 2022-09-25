using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using static Package;
using static EncounterComponent;
using static InterfaceComponent;
using static SimulationManager;

public class Node : MonoBehaviour, INode
{
    public string Name;  // get
    public Region RegionData; // get
    public short Power; // get
    public float EnergyLevel; // get
    public INodeState Status; // get + private set con funzioni in più
    public short MaxConnections; // get
    [SerializeField] private Waveform _tone;
    public Waveform Tone
    {
        get { return _tone; }
        private set { _tone = value; }
    }
    [SerializeField] private string _physicalAddress;
    [SerializeField] private List<Encounter> _currentEncounters;
    [SerializeField] private List<Node> _regionGateways;
    private Queue<Package> _packageQueue;
    [SerializeField] private readonly float _energyDrainRatePerCycle;                 //  percentuale di batteria da scaricare dopo ogni duty cycle [Range(0.01f, 0.1f)] 
    [SerializeField] private readonly float _chargingRatePerSecond;                   //  percentuale di batteria ricaricata ogni secondo [Range(0.01f, 0.1f)]
    // [Range(3,10)]           private readonly float CooldownTime;                                tempo massimo di stop dell'attività per far tornare il nodo al 100% di batteria, va messo nello stato Sleeping
    [SerializeField] private readonly float _dutyCycleDuration;                       //  (in ms) tempo di operatività effettiva in un secondo [Range(10, 1000)]





    public Node(string name, Region region, short power, INodeState firstState, short maxConnections, short capacity)
    {
        Name = name;
        RegionData = region;
        Power = power;
        Status = firstState;
        MaxConnections = maxConnections;
        EnergyLevel = 1.0f;
        _energyDrainRatePerCycle = UnityEngine.Random.Range(0.01f, 0.1f);                   // il nodo consuma da 1% a 10% di batteria (rispetto alla capienza massima di 1.0f) per ogni ciclo di operatività
        _chargingRatePerSecond = UnityEngine.Random.Range(0.01f, 0.1f);                     // il nodo ricarica da 1% a 10% di batteria (rispetto alla capienza massima di 1.0f) ogni secondo
        //CooldownTime            = UnityEngine.Random.Range(3,10);                         // va messo dentro la classe Sleeping
        _dutyCycleDuration = UnityEngine.Random.Range(10, 1000);
        _tone = new(RegionData);
        _physicalAddress = GeneratePhysicalAddress();
        _regionGateways = new();
        _packageQueue = new(capacity);
        _currentEncounters = new();
        foreach (var node in RegionData.NodeList)
        {
            if ((node.Status as Gateway) != null)
            {
                _regionGateways.Add(node);
            }
        }

    }


    private string GeneratePhysicalAddress()                                                                                // questa funzione cripta in SHA256 una stringa di byte per generare un indirizzo fisico univoco
    {
        SHA256 encryptor = SHA256.Create();                                                                                 // creo un'istanza dell'algoritmo SHA256
        byte[] waveformBytes = new byte[NumberOfFrequencies * sizeof(int)];
        Buffer.BlockCopy(Tone.GetRegionWaveformSpectrum(this.gameObject), 0, waveformBytes, 0, NumberOfFrequencies);        // copio i singoli byte dall'array dei campioni della waveform ORIGINALE
        byte[] addressBytes = encryptor.ComputeHash(waveformBytes);                                                         // computo l'hash e lo salvo in una variabile PhysicalAddress
        return System.Text.Encoding.ASCII.GetString(addressBytes);
    }

    public Encounter EstablishConnection(Node listener)
    {
        // usa raycast: se la distanza di un nodo è minore della sua potenza massima allora lo posso raggiungere e stabilisco una connessione
        Encounter encounter;
        if ((Status as Infected) != null)
        {
            encounter = new MaliciousEncounter(this, listener);
            _currentEncounters.Add(encounter);
            return encounter;
        }
        else
        {
            encounter = new SecureEncounter(this, listener);
            _currentEncounters.Add(encounter);
            return encounter;
        }
    }

    public void ShowInfo()
    {
    }

    private void CreateBundle(Encounter encounter)                                          // crea il bundle e lo rende disponibile all'invio
    {
        Package toSend = new(Name, encounter.Destination.Name, encounter.Destination.RegionData.RegionID, (byte)UnityEngine.Random.Range(0, 256), _tone);
        Status.SendBundle(encounter, toSend);
    }

    public void RefreshEncounters()                                                         // a ogni ciclo di update controllo:
                                                                                            // 1) se gli encounters sono ancora attivi
                                                                                            // 2) se posso mandare a qualcuno il contenuto della mia coda (se non vuota)
    {
        foreach (Encounter e in _currentEncounters)
        {
            //if ("circle raycast non tocca più il nodo")                                     // 1) quali encounters sono ancora attivi? taglia quelli non raggiungibili
            //{
            //    CurrentEncounters.Remove(e);
            //}

            //else                                                                            // 2) ho dei bundle in coda d'attesa o posso inviarne di nuovi?
            //{
            //    if (BundleQueue.Count != 0)                                                 // se la coda non è vuota
            //    {
            //        Status.SendBundle(e, BundleQueue.Dequeue());                            // svuota la coda dei bundle da reindirizzare inviandoli su quanti più encounter possibili
            //    }
            //    else
            //    {
            //        CreateBundle(e);                                                        // altrimenti invia un nuovo bundle sul prossimo encounter analizzato
            //    }
            //}
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
