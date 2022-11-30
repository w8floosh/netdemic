using System;
using System.Collections.Generic;
using UnityEngine;
using static SimulationManager;

[Serializable]
public class Region : MonoBehaviour
{
    [SerializeField] private byte _regionID = SimulationManagerInstance.ActiveRegions;
    public byte RegionID
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
    [SerializeField] public Waveform RegionWaveform;
    //public Controls controls;
    //public UnityEvent OnZoomIn;
    //public UnityEvent OnZoomOut;
    public void AddNode(Node node)
    {
        NodeList.Add(node);
    }
    public void SwitchInfoPanel()
    {

    }
    private void Awake()
    {
        NodeList = new List<Node>();
        int NodeCount = UnityEngine.Random.Range(1, SimulationManagerInstance.MaxNodesPerRegion + 1);
        for (int i = 0; i < NodeCount; i++)
        {
            GameObject nodeObject = (GameObject)Instantiate(Resources.Load("Node"));
            Node node = nodeObject.AddComponent<Node>();
            nodeObject.name = node.name;
            node.RegionData = this; // NON TI DIMENTICARE!!!!!!!
            AddNode(node);
            nodeObject.SetActive(true);
        }
        Debug.Log("created Region component with " + NodeList.Count + " nodes, regionID: " + _regionID);
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
