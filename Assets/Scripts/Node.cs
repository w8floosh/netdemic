using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static InterfaceComponent;
using static SimulationManager;
using System.Linq;

public class Node : MonoBehaviour, INode
{
    public string Name;  // get
    public Region RegionData; // get
    public float Power; // get
    public float Speed;
    public float EnergyLevel; // get
    public NodeState Status; // get + private set con funzioni in più
    public short MaxConnections; // get
    [SerializeField] private Waveform _tone;
    public Waveform Tone
    {
        get { return _tone; }
        private set { _tone = value; }
    }
    [SerializeField] private string _physicalAddress;
    [SerializeField] private List<GameObject> _currentEncounters;
    [SerializeField] private List<Node> _regionGateways;
    private Queue<Package> _packageQueue;
    private short _queueSize;
    [SerializeField] private float _energyDrainRatePerCycle;                 //  percentuale di batteria da scaricare dopo ogni duty cycle [Range(0.01f, 0.1f)] 
    [SerializeField] private float _chargingRatePerSecond;                   //  percentuale di batteria ricaricata ogni secondo [Range(0.01f, 0.1f)]
    // [Range(3,10)]           private readonly float CooldownTime;                                tempo massimo di stop dell'attività per far tornare il nodo al 100% di batteria, va messo nello stato Sleeping
    [SerializeField] private float _dutyCycleDuration;                       //  (in ms) tempo di operatività effettiva in un secondo [Range(10, 1000)]

    private string GeneratePhysicalAddress()                                                                                // questa funzione cripta in SHA256 una stringa di byte per generare un indirizzo fisico univoco
    {
        SHA256 encryptor = SHA256.Create();                                                                         // creo un'istanza dell'algoritmo SHA256
        float[] spectrum = Tone.GetRegionWaveformSpectrum(this.gameObject);
        byte[] waveformBytes = new byte[spectrum.Length * sizeof(float)];
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

    public GameObject EstablishConnection(GameObject other)
    {
        // usa raycast: se la distanza di un nodo è minore della sua potenza massima allora lo posso raggiungere e stabilisco una connessione
        //if (Vector3.Distance(transform.position, listener.gameObject.transform.position) > Power) return null;
        Node listener = other.GetComponent<Node>();

        // se trovo un encounter con destinazione uguale al componente nodo di other allora vuol dire che il collegamento già esiste
        if (_currentEncounters.Where(e => e.GetComponent<Encounter>().Destination == listener).Any()) return null;
        GameObject encounterObject = new(this.gameObject.name + " --> " + other.name);
        encounterObject.transform.parent = transform;
        if ((Status as Infected) != null)
        {
            MaliciousEncounter e = encounterObject.AddComponent<MaliciousEncounter>();
            e.SetupEncounter(this, listener);
            _currentEncounters.Add(encounterObject);
            return encounterObject;
        }
        else
        {
            SecureEncounter e = encounterObject.AddComponent<SecureEncounter>();
            e.SetupEncounter(this, listener);
            _currentEncounters.Add(encounterObject);
            return encounterObject;
        }
    }
    public void PackageEnqueue(Package p)
    {
        _packageQueue.Enqueue(p);
    }
    public Package PackageDequeue()
    {
        return _packageQueue.Dequeue();
    }
    public void ShowInfo()
    {
    }

    public void SwitchStatusTo(NodeState status)
    {
        if (Status != null) Status.enabled = false;
        else
        {
            Status = status;
            return;
        }
        Status = status;
    }

    public void CreatePackage(Encounter encounter)                                          // crea il bundle e lo rende disponibile all'invio
    {
        Package toSend;
        if ((toSend = PackageDequeue()) == null) Status.SendPackage(encounter, toSend);
        else
        {
            toSend = new(Name, encounter.Destination.Name, encounter.Destination.RegionData.RegionID, (byte)UnityEngine.Random.Range(0, 256), _tone);
            Status.SendPackage(encounter, toSend);
        }

    }

    public List<GameObject> RefreshEncounters()                                              // a ogni ciclo di update controllo:
                                                                                            // 1) se gli encounters sono ancora attivi
                                                                                            // 2) se posso mandare a qualcuno il contenuto della mia coda (se non vuota)
    {
        foreach (GameObject e in _currentEncounters)
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
        return _currentEncounters;
    }
    private IEnumerator WalkCoroutine(float step, float xMove, float yMove)
    {
        step += Speed * Time.deltaTime;
        //Debug.Log(step);
        //Debug.Log("moving " + name + " towards (" + xMove + ", " + yMove + ")");
        Vector3 destination = new(xMove, yMove);
        if (step < 1f)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, transform.position + destination, step);
            //Debug.Log(gameObject.transform.parent);
            //Debug.Log("Distanza di " + gameObject.name + " " + newPosition + " da " + transform.parent.name + " " + transform.parent.position + ": " + Vector3.Distance(gameObject.transform.parent.position, newPosition));
            if (Vector3.Distance(gameObject.transform.parent.position, newPosition) <= 4.5f) // se questo è falso la prossima destinazione oltrepassa il raggio della regione
                transform.position = newPosition;
            else
            {
                transform.localPosition *= .9f;
                xMove = -xMove;
                yMove = -yMove;
            }
        }
        else
        {
            step = 0;
            transform.position += destination;
            xMove = UnityEngine.Random.Range(-SimulationManagerInstance.MaxNodeDisplacement, SimulationManagerInstance.MaxNodeDisplacement);
            yMove = UnityEngine.Random.Range(-SimulationManagerInstance.MaxNodeDisplacement, SimulationManagerInstance.MaxNodeDisplacement);
        }
        yield return SimulationManagerInstance.WaitForPositionUpdate;
        StartCoroutine(WalkCoroutine(step, xMove, yMove));
    }
    void Awake()
    {
        this.enabled = false;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        short queueSize = (short)UnityEngine.Random.Range(5, 100);
        Name = GenerateName(8);
        Power = UnityEngine.Random.Range(.5f, 5f);
        Speed = UnityEngine.Random.Range(3f, 15f);
        gameObject.AddComponent<Healthy>().enabled = true;
        gameObject.AddComponent<Infected>().enabled = false;
        gameObject.AddComponent<Gateway>().enabled = false;
        gameObject.AddComponent<Sleeping>().enabled = false;
        MaxConnections = 3;
        EnergyLevel = 1.0f;
        _energyDrainRatePerCycle = UnityEngine.Random.Range(0.01f, 0.1f);                   // il nodo consuma da 1% a 10% di batteria (rispetto alla capienza massima di 1.0f) per ogni ciclo di operatività
        _chargingRatePerSecond = UnityEngine.Random.Range(0.01f, 0.1f);                     // il nodo ricarica da 1% a 10% di batteria (rispetto alla capienza massima di 1.0f) ogni secondo
        _dutyCycleDuration = UnityEngine.Random.Range(10, 1000);
        _tone = gameObject.AddComponent<Waveform>();
        gameObject.name = Name;
        _tone.SetupWaveform(RegionData);
        _physicalAddress = GeneratePhysicalAddress();
        _regionGateways = new();
        _packageQueue = new(queueSize);
        _currentEncounters = new();
        foreach (var node in RegionData.NodeList)
        {
            if ((node.Status as Gateway) != null)
            {
                _regionGateways.Add(node);
            }
        }
        Healthy status = GetComponent<Healthy>();
        //Debug.Log(status);
        SwitchStatusTo(status);
        float xMove = UnityEngine.Random.Range(-SimulationManagerInstance.MaxNodeDisplacement, SimulationManagerInstance.MaxNodeDisplacement);
        float yMove = UnityEngine.Random.Range(-SimulationManagerInstance.MaxNodeDisplacement, SimulationManagerInstance.MaxNodeDisplacement);
        StartCoroutine(WalkCoroutine(0, xMove, yMove));
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Collider2D[] nodes = Physics2D.OverlapCircleAll(transform.position, Power, SimulationManagerInstance.NodeLayer);
        Debug.Log(gameObject.name + " può raggiungere " + (nodes.Length-1) + " nodi");
        foreach (Collider2D node in nodes)
        {
            if (node.gameObject != this.gameObject) EstablishConnection(node.gameObject);
        }
    }
}
