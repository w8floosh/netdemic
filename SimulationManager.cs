using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static InterfaceComponent;
using static NodeComponent;
using static Waveform;
using UnityEngine.UIElements;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance    = new();
    public const int SampleRatePerSecond        = 44100;
    public const byte BitDepth                  = 16;
    private List<Region> RegionList;

    [Range(2, 64)] public short NumberOfRegions;
    public class Region
    {
        public readonly short RegionID;
        public List<Node> NodeList         { get; }
        public Waveform RegionWaveform      { get; }

        // tra quanti valori interi sono estratti casualmente i due numeri: la sua metà è l'estremo positivo della funzione Random.Range
        [Range(128, 4096)]  public readonly double MeanRandomness; 
        [Range(2, 10)]      public readonly double VarianceRandomness;


        public Region(short id)
        {
            RegionID        = id;
            NodeList        = new List<Node>();
            RegionWaveform  = new();
            GaussianGenerator();
        }

        public void AddNode(Node node)
        {
            NodeList.Add(node);
        }

        private void GaussianGenerator()
        {
            // genero una gaussiana con media e varianza randomiche
            double GDMean       = UnityEngine.Random.Range((float)-MeanRandomness / 2, (float)MeanRandomness / 2);
            double GDVariance   = Math.Pow(UnityEngine.Random.Range((float)-VarianceRandomness / 2, (float)VarianceRandomness / 2), 2);
            Array.Copy(GaussianDistributor(GDMean, GDVariance), RegionWaveform.SamplesArray, SampleRatePerSecond);
        }
    }


    private SimulationManager()
    {
        RegionList = new List<Region>();
        InitializeSimulation(NumberOfRegions);
    }

    private void InitializeSimulation(short length)
    {
        RegionList = new List<Region>();
        for (short i = 1; i <= length; i++) 
            RegionList.Add(new Region(i));
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
