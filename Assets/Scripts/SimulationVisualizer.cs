using JetBrains.Annotations;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InterfaceComponent;

public class SimulationVisualizer : MonoBehaviour
{
    public static SimulationVisualizer SimulationVisualizerInstance;
    public Camera SimulationCamera;
    public SimulationManager SimulationManager;
    public int MeshGridCols;
    public int MeshGridRows;
    private List<(Vector3 position, Region region)> _meshGrid;
    private GameObject _stubs;


    private void GenerateGrid()
    {
        Vector3 cameraPosition = SimulationCamera.ScreenToWorldPoint(new Vector2(0, SimulationCamera.pixelHeight)); // conservo l'angolo in alto a sinistra della telecamera
        _meshGrid = new();
        for (int x = 1; x <= MeshGridRows; x++)
        {
            for (int y = 1; y <= MeshGridCols; y++)
            {
                // divido lo schermo in una griglia di GridSize+1 celle in verticale e orizzontale, e prendo ogni volta il punto in comune tra 4 celle vicine
                _meshGrid.Add( 
                    new(
                    /* Vector3 */  SimulationCamera.ScreenToWorldPoint(new Vector3(SimulationCamera.pixelWidth * x / (MeshGridRows + 1), SimulationCamera.pixelHeight * y / (MeshGridCols + 1))),
                    /* Region */   null)
                    );
            }
        }
    }
    public void SetupVisualization(Camera simulationCamera, SimulationManager simulationManager, int meshGridRows, int meshGridCols)
    {
        SimulationCamera = simulationCamera;
        SimulationManager = simulationManager;
        MeshGridRows = meshGridRows;
        MeshGridCols = meshGridCols;
        GenerateGrid();
        foreach (Region r in SimulationManager.RegionList)
        {
            // pesco una ad una le regioni e le posiziono negli slot vuoti della meshgrid
            List<(Vector3 position, Region region)> slots = _meshGrid.FindAll(v => v.region == null);
            int index = UnityEngine.Random.Range(0, slots.Count);
            GameObject regionStubObject = (GameObject)Instantiate(Resources.Load("Region Stub"));
            regionStubObject.name = "Region " + r.RegionID + " stub";
            regionStubObject.transform.position = new Vector3(slots[index].position.x, slots[index].position.y, -1);
            regionStubObject.transform.parent = _stubs.transform;
            _meshGrid[_meshGrid.IndexOf(slots[index])] = new(slots[index].position, r);
            //float gatewayDistanceFromCenter = 0;
            //Node gatewayNode;
            foreach (Node n in r.NodeList)
            {
                n.gameObject.transform.parent = regionStubObject.transform;
                /*n.gameObject.transform.localPosition = new Vector3(UnityEngine.Random.Range(0, regionStubObject.transform.localScale.x / 2 - (n.gameObject.transform.localScale.x * regionStubObject.transform.localScale.x) / 2),
                                                                   UnityEngine.Random.Range(0, regionStubObject.transform.localScale.y / 2 - (n.gameObject.transform.localScale.y * regionStubObject.transform.localScale.y) / 2),
                                                                   0);*/
                n.gameObject.transform.localPosition = Random.insideUnitCircle * .3f;
                n.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                //if (Vector3.Distance(n.gameObject.transform.position, gatewayNode.gameObject.transform.position)
            }
        }
    }
    public void DrawEncounters()
    {
        foreach(Region region in SimulationManager.RegionList)
        {
            foreach(Node n in region.NodeList)
            {
                List<GameObject>  nodeEncounters = n.RefreshEncounters();
                foreach (GameObject e in nodeEncounters)
                {
                    Encounter ec = e.GetComponent<Encounter>();
                    ec.Edge.SetPositions(new Vector3[2]
                    {
                        ec.Source.transform.position,
                        ec.Destination.transform.position
                    });
                }
            }
        }
    }
    private void Awake()
    {
        if (SimulationVisualizerInstance == null)
        {
            SimulationVisualizerInstance = this;
            DontDestroyOnLoad(gameObject);
            _stubs = new GameObject("Visualized regions");
            _stubs.transform.position = transform.position;
        }
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (_meshGrid == null)
        {
            return;
        }
        Gizmos.color = Color.black;
        foreach ((Vector3 position, Region region) in _meshGrid)
        {
            Gizmos.DrawSphere(position, 0.5f);
        }
    }
}
