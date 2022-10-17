using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static InterfaceComponent;
using static Node;
using static Waveform;
using static Region;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager SimulationManagerInstance;
    public GameObject SimulationVisualizer;
    public const int NumberOfFrequencies = 22000;
    public float PositionUpdateInterval = 1f;
    public float MaxNodeDisplacement;
    [Range(2, 64)] public byte NumberOfRegions;
    [Range(1, 16)] public byte MaxNodesPerRegion;
    public WaitForSeconds WaitForPositionUpdate;
    public LayerMask NodeLayer;
    [SerializeField] private List<GameObject> _encounterList;
    public List<GameObject> EncounterList
    {
        get { return _encounterList; }
        private set { _encounterList = value; }
    }
    private GameObject _regions;
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
        for (byte i = 1; i <= length; i++)
        {
            ActiveRegions++;
            RegionList.Add(CreateRegion());
            
        }
        Debug.Log("simulation initialized");
    }

    public Region GetRegionData(byte id)
    {
        return _regionList.Find(r => r.RegionID == id);
    }

    private Region CreateRegion() // disegna uno sprite prefab Region nelle coordinate (frequenza, probabilità 0-100) e lo renderizza
    {
        GameObject regionObject = (GameObject)Instantiate(Resources.Load("Region"));
        Region r = regionObject.AddComponent<Region>();
        Waveform rW = regionObject.AddComponent<Waveform>();
        rW.SetupWaveform(r);
        r.RegionWaveform = rW;
        regionObject.name = "Region " + r.RegionID;
        (int frequency, float probability) fundamental = r.RegionWaveform.GetFundamental();
        //Debug.Log("fundamental: " + fundamental);
        regionObject.transform.position = new Vector3(fundamental.frequency/100, fundamental.probability, -1);
        regionObject.GetComponent<SpriteRenderer>().enabled = true;
        regionObject.transform.parent = _regions.transform;
        return r;
    }
    private void VisualizeSimulation(int rows, int cols)
    {
        SimulationVisualizer simulator = SimulationVisualizer.AddComponent<SimulationVisualizer>();
        simulator.SetupVisualization(SimulationVisualizer.GetComponent<Camera>(), SimulationManagerInstance, rows, cols);
    }
    private void Awake()
    {
        if (SimulationManagerInstance == null)
        {
            SimulationManagerInstance = this;
            ActiveRegions = 0;
            DontDestroyOnLoad(gameObject);
            _regions = new GameObject("Active regions");
            _regions.transform.position = transform.position;
            WaitForPositionUpdate = new WaitForSeconds(PositionUpdateInterval);
        }
        else Destroy(gameObject);
        RegionList = new();
        EncounterList = new();
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
