using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager SimulationManagerInstance;
    public int LastPacketSentID;
    public GameObject SimulationVisualizer;
    public const int NumberOfFrequencies = 22000;
    //public float PositionUpdateInterval = 1f;
    //public float MaxNodeDisplacement = 0.3f;
    [Range(.5f, 5f)]    public float BroadcastFrequency = 1f;
    [Range(.01f, .1f)]  public float EncounterChance = .01f;
    [Range(.01f, .2f)]  public float ConnectionDropChance = .01f;
    [Range(.05f, 1f)]   public float TransmissionChance = .05f;
    [Range(2, 64)]      public byte NumberOfRegions;
    [Range(2, 16)]      public byte MaxNodesPerRegion;
    [Range(1, 64)]      public byte StartingViralLoad;
    //public WaitForSeconds WaitForPositionUpdate;
    public WaitForSeconds WaitForBroadcast;
    public LayerMask RegionLayer;
    public LayerMask NodeLayer;
    public LayerMask VirusLayer;
    public Waveform VirusWaveform;
    public List<GameObject> VirusSources;
    //private GameObject _virusSource;
    //[SerializeField] private List<GameObject> _encounterList;
    //public List<GameObject> EncounterList
    //{
    //    get { return _encounterList; }
    //    private set { _encounterList = value; }
    //}
    //private GameObject _regions;
    [SerializeField] private List<Region> _regionList;
    public List<Region> RegionList
    {
        get { return _regionList; }
        private set { _regionList = value; }
    }
    [SerializeField] private byte _activeRegions;
    public byte ActiveRegions
    {
        get { return _activeRegions; }
        private set { _activeRegions = value; }
    }

    private void InitializeSimulation(byte length)
    {
        RegionList = new List<Region>();
        VirusSources = new();
        for (byte i = 1; i <= length; i++)
        {
            ActiveRegions++;
            RegionList.Add(CreateRegion());
        }
        GameObject virusList = new GameObject("Virus sources");
        virusList.transform.position = SimulationVisualizer.transform.position;
        while (StartingViralLoad-- > 0)
        {
            GameObject _virusSource = (GameObject)Instantiate(Resources.Load("Infected Node"));
            _virusSource.AddComponent<Node>();
            _virusSource.transform.parent = virusList.transform;
            VirusSources.Add(_virusSource);
            _virusSource.SetActive(true);
        }
        Debug.Log("simulation initialized");

        //_virusSource = (GameObject)Instantiate(Resources.Load("Infected Node"));
        //Node infected = _virusSource.AddComponent<Node>();
        //Destroy(_virusSource.GetComponent<Healthy>());
        //Infected status = _virusSource.AddComponent<Infected>();
        //status.Setup(UnityEngine.Random.Rangex(1f, 10f));
        //infected.Status = status;
        //_virusSource.SetActive(true);
        //Debug.Log("simulation initialized");
    }

    public Region GetRegionData(byte id)
    {
        return _regionList.Find(r => r.RegionID == id);
    }

    public static double Lerp(double a, double b, float t)
    {
        return a + (b - a) * Mathf.Clamp01(t);
    }

    private Region CreateRegion() // disegna uno sprite prefab Region nelle coordinate (frequenza, probabilità 0-100) e lo renderizza
    {
        GameObject regionObject = (GameObject)Instantiate(Resources.Load("Region"));
        Region r = regionObject.AddComponent<Region>();
        Waveform rW = regionObject.AddComponent<Waveform>();
        rW.SetupWaveform(r);
        r.RegionWaveform = rW;
        //regionObject.name = "Region " + r.RegionID;
        //(int frequency, float probability) fundamental = r.RegionWaveform.GetFundamental();
        ////Debug.Log("fundamental: " + fundamental);
        //regionObject.transform.position = new Vector3(fundamental.frequency/100, fundamental.probability, -1);
        //regionObject.GetComponent<SpriteRenderer>().enabled = true;
        //regionObject.transform.parent = _regions.transform;
        return r;
    }
    private void VisualizeSimulation(int rows, int cols)
    {
        SimulationVisualizer simulator = SimulationVisualizer.AddComponent<SimulationVisualizer>();
        simulator.SetupVisualization(rows, cols, VirusSources);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject[] menus = GameObject.FindGameObjectsWithTag("Menu");
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
    }
    private void Awake()
    {
        BroadcastFrequency = SettingsMenu.BroadcastFrequency;
        EncounterChance = SettingsMenu.EncounterChance;
        ConnectionDropChance = SettingsMenu.ConnectionDropChance;
        TransmissionChance = SettingsMenu.TransmissionChance;
        NumberOfRegions = SettingsMenu.NumberOfRegions;
        StartingViralLoad = SettingsMenu.StartingViralLoad;
        MaxNodesPerRegion = SettingsMenu.MaxNodesPerRegion;
        Debug.Log(BroadcastFrequency + " " + EncounterChance + " " + ConnectionDropChance + " " + TransmissionChance + " " + NumberOfRegions + " " + StartingViralLoad + " " + MaxNodesPerRegion);
        SceneManager.sceneLoaded += OnSceneLoaded;
        //foreach (GameObject menu in GameObject.FindGameObjectsWithTag("Menu"))
        //{
        //    menu.SetActive(false);
        //}
        if (SimulationManagerInstance == null)
        {
            SimulationManagerInstance = this;
            LastPacketSentID = 0;
            ActiveRegions = 0;
            DontDestroyOnLoad(gameObject);
            //_regions = new GameObject("Active regions");
            //_regions.transform.position = transform.position;
            //WaitForPositionUpdate = new WaitForSeconds(PositionUpdateInterval);
            WaitForBroadcast = new(BroadcastFrequency);
            RegionList = new();
            //EncounterList = new();
        }
        else Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializeSimulation(NumberOfRegions);
        VisualizeSimulation(16, 8);
        foreach (Region region in RegionList)
        {
            foreach (Node node in region.NodeList)
            {
                node.enabled = true;
            }
        }
        foreach(GameObject v in VirusSources)
        {
            v.GetComponent<Node>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
