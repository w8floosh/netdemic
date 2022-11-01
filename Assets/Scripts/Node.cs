using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static InterfaceComponent;
using static SimulationManager;
using static SimulationVisualizer;
using System.Linq;
using UnityEngine.Pool;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class Node : MonoBehaviour, INode
{
    public string Name;  // get
    public Region RegionData; // get
    public float Power; // get
    public float Speed;
    public float EnergyLevel; // get
    public NodeState Status; // get + private set con funzioni in più
    public short MaxConnections; // get
    public float EnergyDrainRatePerCycle;                 //  percentuale di batteria da scaricare dopo ogni duty cycle [Range(0.01f, 0.1f)] 
    public float ChargingRatePerSecond;                   //  percentuale di batteria ricaricata ogni secondo [Range(0.01f, 0.1f)]
    [Range(3, 10)] public float _cooldownTime;                   //  tempo massimo di stop dell'attività per far tornare il nodo al 100% di batteria, va messo nello stato Sleeping
    public float DutyCycleDuration;                       //  (in ms) tempo di operatività effettiva in un secondo [Range(10, 1000)]

    [SerializeField] private byte _resistanceLimit;
    public byte ResistanceLimit { 
        get { return _resistanceLimit; } 
        private set { _resistanceLimit = value; } 
    }
    [SerializeField] private Waveform _tone;
    public Waveform Tone
    {
        get { return _tone; }
        private set { _tone = value; }
    }
    private Waveform _originalTone;
    public Waveform OriginalTone
    {
        get { return _originalTone; }
        private set { _originalTone = value; }
    }
    // USARE OBJECT POOLING PER ENCOUNTERS
    private ObjectPool<Encounter> _encounterPool;
    [SerializeField] private List<GameObject> _currentEncounters;
    public List<GameObject> CurrentEncounters
    {
        get { return _currentEncounters;}
        private set { _currentEncounters = value; }
    }
    [SerializeField] private string _physicalAddress;
    [SerializeField] private List<Node> _regionGateways;
    [SerializeField] private Queue<Package> _packageQueue;
    private short _queueSize;
    private System.Random _randomGenerator;
    private Vector3 _endPoint;
    private Collider2D _nodeCollider;
    private string GeneratePhysicalAddress()                                                                                // questa funzione cripta in SHA256 una stringa di byte per generare un indirizzo fisico univoco
    {
        SHA256 encryptor = SHA256.Create();                                                                         // creo un'istanza dell'algoritmo SHA256
        double[] spectrum = Tone.GetSpectrum(gameObject);
        byte[] waveformBytes = new byte[spectrum.Length * sizeof(double)];
        // copio i singoli byte dall'array dei campioni della waveform ORIGINALE
        Buffer.BlockCopy(spectrum, 0, waveformBytes, 0, waveformBytes.Length);
        // computo l'hash e lo salvo in una variabile PhysicalAddress
        return BitConverter.ToString(encryptor.ComputeHash(waveformBytes)).Replace("-", string.Empty);
        //return System.Text.Encoding.ASCII.GetString(addressBytes);
    }

    private string GenerateName(int length)
    {
        System.Random random = new();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public GameObject EstablishConnectionFunzionante(GameObject other) // FUNZIONANTE
    {
        // usa raycast: se la distanza di un nodo è minore della sua potenza massima allora lo posso raggiungere e stabilisco una connessione
        //if (Vector3.Distance(transform.position, listener.gameObject.transform.position) > Power) return null;
        Node listener = other.GetComponent<Node>();
        // se trovo un encounter con destinazione uguale al componente nodo di other allora vuol dire che il collegamento già esiste
        if (_currentEncounters.Where(e => e.GetComponent<Encounter>().Destination == listener).Any()) return null;

        GameObject encounterObject = new(this.gameObject.name + " --> " + other.name);
        encounterObject.transform.parent = transform;
        Encounter e;
        if (Status is Infected) e = encounterObject.AddComponent<MaliciousEncounter>();
        else e = encounterObject.AddComponent<SecureEncounter>();

        e.SetupEncounter(this, listener);
        return encounterObject;
    }
    public GameObject EstablishConnection(GameObject other) 
    {
        Node listener = other.GetComponent<Node>();
        // se trovo un encounter con destinazione uguale al componente nodo di other allora vuol dire che il collegamento già esiste
        if (_currentEncounters.Where(e => e.GetComponent<Encounter>().Destination == listener).Any()) return null;

        GameObject encounterObject;
        Transform child;
        if ((child = transform.Find(this.gameObject.name + " --> " + other.name)) == null) 
        {
            encounterObject = new(this.gameObject.name + " --> " + other.name);
            encounterObject.transform.parent = transform;
            Encounter e;
            if (Status is Infected) e = encounterObject.AddComponent<MaliciousEncounter>();
            else e = encounterObject.AddComponent<SecureEncounter>();
            e.SetupEncounter(this, listener);
        }
        else
        {
            encounterObject = child.gameObject;
        }
        encounterObject.SetActive(true);

        return encounterObject;
    }
    public void PackageEnqueue(Package p)
    {
        _packageQueue.Enqueue(p);
    }
    public Package PackageDequeue()
    {
        return _packageQueue.Dequeue();
    }
    public int NumberOfQueuedPackages()
    {
        return _packageQueue.Count;
    }
    public void ShowInfo()
    {
    }

    //TROIAIO

    public List<GameObject> RefreshEncounters(List<Collider2D> nodes)
    // a ogni ciclo di update controllo:
    // 1) se gli encounters sono ancora attivi
    // 2) se posso mandare a qualcuno il contenuto della mia coda (se non vuota)
    // !!! uso un for reversed al posto di for each per evitare InvalidOperationException causati dalla modifica della collezione
    {
        for (int i = CurrentEncounters.Count - 1; i >= 0; i--)
        {
            Encounter current = CurrentEncounters[i].GetComponent<Encounter>();
            //Debug.Log("refreshing encounter: " + CurrentEncounters[i].name);
            if (!nodes.Contains(current.Destination.gameObject.GetComponent<Collider2D>()) || current.Destination.Status is Infected)
            {
                //Debug.Log(CurrentEncounters[i].name + ": unreachable after " + CurrentEncounters[i].GetComponent<Encounter>().TimeElapsed + " seconds");
                current.CloseEncounter();
            }
            else if (_randomGenerator.NextDouble() < SimulationManagerInstance.ConnectionDropChance)
            {
                //Debug.Log(CurrentEncounters[i].name + ": connection dropped after " + CurrentEncounters[i].GetComponent<Encounter>().TimeElapsed + " seconds");
                current.CloseEncounter();
            }
        }

        List<GameObject> newEncounters = new();
        foreach (Collider2D node in nodes)
        {
            if (CurrentEncounters.Capacity == CurrentEncounters.Count + newEncounters.Count) 
                break;
            Node n = node.gameObject.GetComponent<Node>();
            // stabilirò una connessione se e solo se node non è già destinazione di un encounter E se non è infetto
            if (!CurrentEncounters.Where(e => e.GetComponent<Encounter>().Destination == n).Any()
                && n.Status is not Infected)
            {
                if (_randomGenerator.NextDouble() < SimulationManagerInstance.EncounterChance)
                    newEncounters.Add(EstablishConnection(n.gameObject));
            }
        }
        if (newEncounters.Count > 0) CurrentEncounters.AddRange(newEncounters);
        return CurrentEncounters;
    }

    public void BroadcastTransmission()
    {
        if (CurrentEncounters.Count > 0)
        {
            Package toSend = null;
            if (_packageQueue.Count > 0)
            {
                toSend = PackageDequeue();
            }
            else
            {
                SimulationManagerInstance.LastPacketSentID++;
                toSend = CreatePackage();
            }

            for (int i = CurrentEncounters.Count - 1; i >= 0; i--)
            {
                if (_randomGenerator.NextDouble() < SimulationManagerInstance.TransmissionChance)
                {
                    Encounter ec = CurrentEncounters[i].GetComponent<Encounter>();
                    if (toSend.Destination == null)
                    {
                        toSend.SetNextHop(ec.Destination.Name);
                    }
                    //toSend ??= new(SimulationManagerInstance.LastPacketSentID, Name, ec.Destination.Name, ec.Destination.RegionData.RegionID, (byte)UnityEngine.Random.Range(0, 256), _tone);
                    //Debug.Log("created package with ID: " + toSend.ID);
                    Status.SendPackage(ec, toSend);
                }
            }
        }
    }
    private IEnumerator BroadcastTransmissionCoroutine() 
    {
        while (true)
        {
            if (Status is Infected status)
            {
                yield return new WaitForSeconds(SimulationManagerInstance.BroadcastFrequency / status.InfectionSpeed);
            }
            else yield return SimulationManagerInstance.WaitForBroadcast;
            BroadcastTransmission();
        }
    }
    private Package CreatePackage()
    {
        Region finalDestinationRegion = SimulationManagerInstance.RegionList.ElementAt(_randomGenerator.Next(0, SimulationManagerInstance.RegionList.Count - 1));
        Node finalDestinationNode = finalDestinationRegion.NodeList.ElementAt(_randomGenerator.Next(0, finalDestinationRegion.NodeList.Count - 1));
        return new Package(SimulationManagerInstance.LastPacketSentID, Name, null, finalDestinationNode.RegionData.RegionID, finalDestinationNode.Name, (byte)UnityEngine.Random.Range(0, 256), _tone);
    }
    private void StatusUpdate()
    {
        if (EnergyLevel <= 0f)
        {
            Status.enabled = false;
            StopAllCoroutines();
            NodeState lastStatus = Status;
            Sleeping newStatus = gameObject.AddComponent<Sleeping>();
            Status = newStatus;
            newStatus.Setup(lastStatus, UnityEngine.Random.Range(0f, 3f));
        }
        if (Status is Healthy)
        {
            Healthy status = Status as Healthy;
            if (ResistanceLimit <= status.InfectionSteps)
            {
                Debug.Log("Infected!");
                Destroy(status);
                Infected newStatus = gameObject.AddComponent<Infected>();
                newStatus.Setup(UnityEngine.Random.Range(1f, 10f));
                Status = newStatus;
                gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }

    private void Walk()
    {
        if (_endPoint == gameObject.transform.position) SetRandomWalkingPath();
        if (Status is Infected)
            //gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, gameObject.transform.position + newPosition, Speed * 5 * Time.deltaTime);
            //while (gameObject.transform.position != gameObject.transform.position + newPosition) 
            gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, _endPoint, 2 * Speed * Time.deltaTime);
        else
            //while (gameObject.transform.position != gameObject.transform.parent.position + newPosition) 
            gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, _endPoint, Speed * Time.deltaTime);
    }

    private void SetRandomWalkingPath()
    {
        // determino un nuovo punto all'interno di un cerchio con centro il nodo (se infetto) o la regione (se il nodo è sano) con raggio 5 (che è la grandezza standard di una regione)
        Vector2 nextPoint = UnityEngine.Random.insideUnitCircle * 5f;
        if (Status is Infected) 
        {
            _endPoint = new Vector2(Mathf.Clamp(gameObject.transform.position.x + nextPoint.x, SimulationVisualizerInstance.BottomLeftPoint.x, SimulationVisualizerInstance.TopRightPoint.x),
                                    Mathf.Clamp(gameObject.transform.position.y + nextPoint.y, SimulationVisualizerInstance.BottomLeftPoint.y, SimulationVisualizerInstance.TopRightPoint.y));
        }
        else
        {
            _endPoint = new Vector2(Mathf.Clamp(gameObject.transform.parent.position.x + nextPoint.x, SimulationVisualizerInstance.BottomLeftPoint.x, SimulationVisualizerInstance.TopRightPoint.x),
                                    Mathf.Clamp(gameObject.transform.parent.position.y + nextPoint.y, SimulationVisualizerInstance.BottomLeftPoint.y, SimulationVisualizerInstance.TopRightPoint.y));
        }                    
    }
    void Awake()
    {
        this.enabled = false;
        gameObject.SetActive(false);
        _randomGenerator = new();
        //_encounterPool = new ObjectPool<Encounter>(CreateEncounter, OnConnection, OnDisconnection);
        _nodeCollider = gameObject.GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        short queueSize = (short)UnityEngine.Random.Range(5, 100);
        Name = GenerateName(8);
        gameObject.name = Name;

        //Power = UnityEngine.Random.Range(.5f, 5f);
        Power = UnityEngine.Random.Range(2f, 5f);
        Speed = UnityEngine.Random.Range(1f, 3f);
        if (RegionData != null)
        {
            Status = gameObject.AddComponent<Healthy>();
            ResistanceLimit = (byte)UnityEngine.Random.Range(8, 16);
        }
        else
        {
            Status = gameObject.AddComponent<Infected>();
            (Status as Infected).Setup(UnityEngine.Random.Range(1f, 10f));
        }
        SetRandomWalkingPath();
        MaxConnections = 3;
        EnergyLevel = 1.0f;
        EnergyDrainRatePerCycle = UnityEngine.Random.Range(0.01f, 0.1f);                   // il nodo consuma da 1% a 10% di batteria (rispetto alla capienza massima di 1.0f) per ogni ciclo di operatività
        ChargingRatePerSecond = UnityEngine.Random.Range(0.01f, 0.1f);                     // il nodo ricarica da 1% a 10% di batteria (rispetto alla capienza massima di 1.0f) ogni secondo
        DutyCycleDuration = UnityEngine.Random.Range(10, 1000);
        if (Status is Healthy || SimulationManagerInstance.VirusWaveform is null)
        {
            _tone = gameObject.AddComponent<Waveform>();
            _tone.SetupWaveform(RegionData);
            SimulationManagerInstance.VirusWaveform = _tone;
        }
        else
            _tone = SimulationManagerInstance.VirusWaveform;

        _originalTone = _tone;
        _physicalAddress = GeneratePhysicalAddress();
        _regionGateways = new();
        _packageQueue = new(queueSize);
        _currentEncounters = new(MaxConnections);
        if (RegionData != null)
        {
            foreach (var node in RegionData.NodeList)
            {
                if (node.Status is Gateway)
                {
                    _regionGateways.Add(node);
                }
            }
        }
        Status.enabled = true;
        StartCoroutine(BroadcastTransmissionCoroutine());
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        StatusUpdate();
        Walk();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider2D[] nodes = Physics2D.OverlapCircleAll(transform.position, Power, SimulationManagerInstance.NodeLayer);
        //Debug.Log(gameObject.name + " può raggiungere " + (nodes.Length-1) + " nodi");
        RefreshEncounters(nodes.Where(collider => collider != _nodeCollider).ToList());
    }

}