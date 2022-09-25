using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SimulationManager;

[Serializable]
public class Region : MonoBehaviour
{
    [SerializeField] private short _regionID = ++NumberOfRegions;
    public short RegionID
    {
        get         {   return _regionID;   }
        private set {   _regionID = value;  }
    }
    
    [SerializeField] private List<Node> _nodeList;
    public List<Node> NodeList
    {
        get         {   return _nodeList;    } 
        private set {   _nodeList = value;   }
    }
    [SerializeField] private Waveform _regionWaveform;
    public Waveform RegionWaveform
    {
        get { return _regionWaveform; }
        private set { _regionWaveform = value; }
    }


    public Region()
    {
        NodeList = new List<Node>();
        RegionWaveform = gameObject.AddComponent<Waveform>();
    }

    public void AddNode(Node node)
    {
        NodeList.Add(node);
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
