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
    public static SimulationManager Instance        = new();
    public const int NumberOfFrequencies            = 22000;
    public static WaitForSeconds SendingInterval    = new(0.1f);
    private List<Region> RegionList;

    [Range(2, 128)] public static byte NumberOfRegions;



    private SimulationManager()
    {
        RegionList = new List<Region>();
        InitializeSimulation(NumberOfRegions);
    }

    private void InitializeSimulation(byte length)
    {
        RegionList = new List<Region>();
        for (byte i = 1; i <= length; i++)
        {
            RegionList.Add(CreateRegion(i));
        }
           
    }


    private Region CreateRegion(byte id)
    {
        GameObject region = new("Region " + id, typeof(Region));

        region.transform.position = new Vector3(id * UnityEngine.Random.Range(1,10), id * UnityEngine.Random.Range(1, 10), -1);
        return region.GetComponent<Region>();
    }
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
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
