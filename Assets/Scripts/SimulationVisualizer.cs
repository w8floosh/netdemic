using JetBrains.Annotations;
using PlasticPipe.PlasticProtocol.Messages;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;
using static InterfaceComponent;
using static SimulationManager;
using UnityEngine.UI;
using System;
using MathNet.Numerics.Statistics;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Drawing2D;
using Unity.Jobs;
using Unity.Collections;
using System.Threading.Tasks;
using NugetForUnity;
using GluonGui.Dialog;

public class SimulationVisualizer : MonoBehaviour
{
    public static SimulationVisualizer SimulationVisualizerInstance;
    public Camera SimulationCamera;
    //public SimulationManager SimulationManager;
    public int MeshGridCols;
    public int MeshGridRows;
    public Vector3 BottomLeftPoint;
    public Vector3 TopRightPoint;
    public Controls controls;
    public GameObject RegionPanel;
    public GameObject NodePanel;
    public UnityEvent OnZoomIn;
    public UnityEvent OnZoomOut;
    public UnityEvent<Node> OnShowNodeInfo;
    public UnityEvent<Region> OnShowRegionInfo;
    public readonly int NetworkOrtographicSize = 50;
    public readonly int RegionOrtographicSize = 12;
    public readonly int NodeOrtographicSize = 5;
    public GameObject VisualizingRegion;
    public GameObject VisualizingNode;
    private GameObject _lastRegionVisualized;
    private GameObject _lastNodeVisualized;
    private List<(Vector3 position, Region region)> _meshGrid;
    private GameObject _regions;
    private Vector3 _originalPosition;
    private readonly float minHzX = -607f;
    private readonly float maxHzX = 607f;
    private readonly float minPercentageY = -97f;
    private readonly float maxPercentageY = 97f;

    public class NodeUIEvent : UnityEvent<Node> { }
    public class RegionUIEvent : UnityEvent<Region> { }

    //public struct WaveformGraphicsJob : IJobParallelFor
    //{
    //    public NativeArray<Vector3> ControlPoints;
    //    public NativeArray<Vector3> Results;
    //    public NativeArray<float> WaveformToDisplay;
    //    public float minHzX;
    //    public float maxHzX;
    //    public float minPercentageY;
    //    public float maxPercentageY;
    //    public int j;
    //    public int firstValueIndex;
    //    public void Execute(int i)
    //    {
    //        if (i < 2 || i >= ControlPoints.Length - 2)
    //        {
    //            Debug.Log(Results.Length);
    //            if (i < 2) j = 0;
    //            Results[i] = ControlPoints[i];
    //        }


    //        else
    //        {
    //            Results[i] = new()
    //            {
    //                x = Mathf.Lerp(minHzX, maxHzX, (float)(firstValueIndex + i) / NumberOfFrequencies),
    //                y = Mathf.Lerp(minPercentageY, maxPercentageY, WaveformToDisplay[j] / 10),
    //                z = 0
    //            };
    //            j++;
    //        }
    //    }
    //}

    private void GenerateGrid()
    {
        //Vector3 cameraPosition = SimulationCamera.ScreenToWorldPoint(new Vector2(0, SimulationCamera.pixelHeight)); // conservo l'angolo in alto a sinistra della telecamera
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
    public void SetupVisualization(int meshGridRows, int meshGridCols, List<GameObject> viruses)
    {
        SimulationCamera = gameObject.GetComponent<Camera>();
        MeshGridRows = meshGridRows;
        MeshGridCols = meshGridCols;
        _regions = new GameObject("Regions");
        GenerateGrid();
        foreach (Region r in SimulationManagerInstance.RegionList)
        {
            // pesco una ad una le regioni e le posiziono negli slot vuoti della meshgrid
            List<(Vector3 position, Region region)> slots = _meshGrid.FindAll(v => v.region == null);
            int index = UnityEngine.Random.Range(0, slots.Count);
            GameObject regionObject = r.gameObject;
            regionObject.name = "Region " + r.RegionID;
            regionObject.transform.position = new Vector3(slots[index].position.x, slots[index].position.y, 0);
            regionObject.transform.parent = _regions.transform;
            _meshGrid[_meshGrid.IndexOf(slots[index])] = new(slots[index].position, r);
            foreach (Node n in r.NodeList)
            {
                n.gameObject.transform.parent = regionObject.transform;
                n.gameObject.transform.localPosition = UnityEngine.Random.insideUnitCircle * .3f;
                n.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        Vector3 pivot = gameObject.transform.position;
        foreach(GameObject v in viruses)
        {
            v.transform.localPosition = new Vector3(UnityEngine.Random.Range(-pivot.x / 2, pivot.x / 2),
                                                    UnityEngine.Random.Range(-pivot.y / 2, pivot.y / 2));
        }
        
        BottomLeftPoint = SimulationVisualizerInstance.SimulationCamera.ViewportToWorldPoint(Vector2.zero);
        TopRightPoint = SimulationVisualizerInstance.SimulationCamera.ViewportToWorldPoint(Vector2.one);
        Debug.Log("BottomLeftPoint: " + BottomLeftPoint + " TopRightPoint: " + TopRightPoint);
    }
    private void DisableEncounterRendering(Encounter e)
    {
        e.Edge.enabled = false;
    }
    private void UpdateEncounterGraphics(Encounter ec)
    {
        if (ec == null) return;
        ec.Edge.SetPositions(new Vector3[2]
        {
            ec.Source.transform.position,
            ec.Destination.transform.position
        });
        if (ec is MaliciousEncounter)
        {
            if (ec.IsBusy) ec.Edge.startColor = ec.Edge.endColor = Color.red;
            else
            {
                ec.Edge.startColor = new Color(255, 165, 0);
                ec.Edge.endColor = Color.yellow;
            }
        }
        else
        {
            if (ec.IsBusy)
            {
                ec.Edge.startColor = ec.Edge.endColor = Color.green;
            }
            else
            {
                ec.Edge.startColor = Color.cyan;
                ec.Edge.endColor = Color.magenta;
            }
        }
    }
    private void DrawIncomingAndOutgoingEncounters(Node n, bool show)
    {
        Encounter[] outgoingEncounters = n.gameObject.GetComponentsInChildren<Encounter>(includeInactive: true);
        foreach (Encounter enc in outgoingEncounters) // outgoing encounters
        {
            if (show)
            {
                if (enc.Edge.enabled == false) enc.Edge.enabled = true;
                UpdateEncounterGraphics(enc);
            }
            else DisableEncounterRendering(enc);
        }

        foreach (GameObject v in SimulationManagerInstance.VirusSources) // incoming malicious encounters
        {
            Node vn = v.GetComponent<Node>();
            List<Encounter> incomingEncounters = v.GetComponentsInChildren<Encounter>(includeInactive: true).ToList().FindAll(e => e.Destination == n);
            // prendi gli encounters del virus che hanno n come nodo destinazione
            //List<GameObject> incomingEncounters = vn.CurrentEncounters.FindAll(e => e.GetComponent<Encounter>().Destination == n);
            foreach (Encounter enc in incomingEncounters)
            {
                if (show)
                {
                    if (enc.Edge.enabled == false) enc.Edge.enabled = true;
                    UpdateEncounterGraphics(enc);
                }
                else DisableEncounterRendering(enc);
            }
        }
    }
    public void DrawEncounters()
    {
        if (Camera.main.orthographicSize == NetworkOrtographicSize) return;
        else if (Camera.main.orthographicSize == RegionOrtographicSize)
        {
            foreach (Node n in VisualizingRegion.GetComponent<Region>().NodeList)
            {
                DrawIncomingAndOutgoingEncounters(n, show: true);
            }
        }
        else
        {
            DrawIncomingAndOutgoingEncounters(VisualizingNode.GetComponent<Node>(), show: true);
        }

    }
    //public void DrawEncounters() //funzionante
    //{
    //    foreach (Region r in SimulationManagerInstance.RegionList)
    //    {
    //        foreach (Node n in r.NodeList)
    //        {
    //            foreach(GameObject e in n.CurrentEncounters)
    //            {
    //                UpdateEncounterGraphics(e);
    //            }
    //        }
    //    }
    //    foreach (GameObject v in SimulationManagerInstance.VirusSources)
    //    {
    //        Node vn = v.GetComponent<Node>();
    //        foreach(GameObject e in vn.CurrentEncounters)
    //        {
    //            UpdateEncounterGraphics(e);

    //        }
    //    }
    //}
    public void ZoomInAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnZoomIn.Invoke();
        }
    }
    public void ZoomOutAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnZoomOut.Invoke();
        }
    }
    private RaycastHit2D CheckRaycastedTo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit;
        ContactFilter2D contactFilter;
        if (VisualizingRegion == null)
            contactFilter = new()
            {
                layerMask = 
                    SimulationManagerInstance.NodeLayer | 
                    SimulationManagerInstance.RegionLayer | 
                    SimulationManagerInstance.VirusLayer
            };
        else
            contactFilter = new()
            {
                layerMask =
                    SimulationManagerInstance.NodeLayer |
                    SimulationManagerInstance.VirusLayer
            };
        //Debug.Log(ray.origin + " " + ray.direction);
        //Debug.DrawLine(ray.origin, Vector3.one, Color.yellow);
        //Debug.DrawRay(ray.origin, ray.direction, Color.yellow);
        hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, contactFilter.layerMask, -1f);
        return hit;
    }
    private void ZoomIn()
    {
        Debug.Log("zooming in");
        RaycastHit2D hit = CheckRaycastedTo();
        if (hit.collider is null) return;
        else Debug.Log(hit.collider.gameObject.name);
        Region r;
        Node n;
        Debug.Log(hit.collider.gameObject.layer + " " + SimulationManagerInstance.RegionLayer.value + " " + SimulationManagerInstance.RegionLayer);
        if (1 << hit.collider.gameObject.layer == SimulationManagerInstance.RegionLayer.value)
        {
            r = hit.collider.transform.GetComponent<Region>();
            Camera.main.orthographicSize = RegionOrtographicSize;
            Camera.main.transform.position = hit.transform.position;
            if (VisualizingRegion != null)
            {
                foreach (Node node in VisualizingRegion.GetComponent<Region>().NodeList)
                {
                    DrawIncomingAndOutgoingEncounters(node, show: false);
                }
            }
            VisualizingRegion = hit.collider.gameObject;
            VisualizingNode = null;
            OnShowRegionInfo.Invoke(r);
        }
        else if (1 << hit.collider.gameObject.layer == SimulationManagerInstance.NodeLayer.value)
        {
            r = hit.collider.transform.parent.GetComponent<Region>();
            n = hit.collider.transform.GetComponent<Node>();
            Camera.main.orthographicSize = NodeOrtographicSize;
            Camera.main.transform.position = hit.transform.position;
            if (VisualizingRegion != null) 
             // stavo visualizzando una regione o un nodo sano di quella regione
             // 4 casi:
             // 1) ho cliccato su un virus
             // 2) ho cliccato su un nodo della stessa regione
             // 3) ho cliccato su un nodo sano di un'altra regione
             // 4) ho cliccato su un nodo infetto di un'altra regione
            {
                Region vr = VisualizingRegion.GetComponent<Region>();
                foreach (Node node in vr.NodeList)
                {
                    DrawIncomingAndOutgoingEncounters(node, show: false);
                }
            }
            else
            // stavo visualizzando un virus oppure tutta la rete
            // 2 casi:
            // 1) ho cliccato su un virus
            // 2) ho cliccato su un qualsiasi nodo di una regione
            {
                if (VisualizingNode != null)
                    DrawIncomingAndOutgoingEncounters(VisualizingNode.GetComponent<Node>(), show: false);
            }
            VisualizingRegion = hit.collider.transform.parent.gameObject;
            VisualizingNode = hit.collider.gameObject;
            OnShowRegionInfo.Invoke(r);
            OnShowNodeInfo.Invoke(n);
        }
        else if (1 << hit.collider.gameObject.layer == SimulationManagerInstance.VirusLayer.value)
        {
            n = hit.collider.transform.GetComponent<Node>();
            Camera.main.orthographicSize = NodeOrtographicSize;
            Camera.main.transform.position = hit.transform.position;
            if (VisualizingRegion != null)
            // stavo visualizzando una regione o un nodo sano di quella regione
            // 4 casi:
            // 1) ho cliccato su un virus
            // 2) ho cliccato su un nodo della stessa regione
            // 3) ho cliccato su un nodo sano di un'altra regione
            // 4) ho cliccato su un nodo infetto di un'altra regione
            {
                Region vr = VisualizingRegion.GetComponent<Region>();
                foreach (Node node in vr.NodeList)
                {
                    DrawIncomingAndOutgoingEncounters(node, show: false);
                }
            }
            else
            // stavo visualizzando un virus oppure tutta la rete
            // 2 casi:
            // 1) ho cliccato su un virus
            // 2) ho cliccato su un qualsiasi nodo di una regione
            {
                if (VisualizingNode != null)
                    DrawIncomingAndOutgoingEncounters(VisualizingNode.GetComponent<Node>(), show: false);
            }
            VisualizingRegion = null;
            VisualizingNode = hit.collider.gameObject;
            OnShowNodeInfo.Invoke(n);
        }
    }

    private void ZoomOut()
    {
        Debug.Log("zooming out");
        Camera.main.orthographicSize = NetworkOrtographicSize;
        Camera.main.transform.position = _originalPosition;
        if (VisualizingNode != null) DrawIncomingAndOutgoingEncounters(VisualizingNode.GetComponent<Node>(), show: false);
        if (VisualizingRegion != null)
        {
            foreach (Node node in VisualizingRegion.GetComponent<Region>().NodeList)
            {
                DrawIncomingAndOutgoingEncounters(node, show: false);
            }
        }
        NodePanel.SetActive(false);
        VisualizingNode = null;
        RegionPanel.SetActive(false);
        VisualizingRegion = null;
    }

    private void OpenNodeInfoPanel(Node nodeData)
    {
        //CanvasRenderer[] fields = gameObject.GetComponentsInChildren<CanvasRenderer>(includeInactive: true);
        GameObject[] nodeFields = new GameObject[NodePanel.transform.childCount];
        for (int i = 0; i < NodePanel.transform.childCount; i++) nodeFields[i] = NodePanel.transform.GetChild(i).gameObject;
        /* Name */
        nodeFields[0].GetComponentInChildren<TextMeshProUGUI>(includeInactive: true).text = nodeData.Name;
        /* Status */
        nodeFields[1].GetComponentInChildren<TextMeshProUGUI>(includeInactive: true).text = nodeData.Status is Healthy ? "healthy" : "infected";
        ///* Physical address */
        nodeFields[2].GetComponentInChildren<TextMeshProUGUI>(includeInactive: true).text = "<no access to physical address>";

        ///* Energy */
        nodeFields[3].GetComponentInChildren<Slider>(includeInactive: true).value = nodeData.EnergyLevel;
        ///* Region ID */
        if (nodeData.RegionData != null)
            nodeFields[4].transform.GetChild(0).gameObject.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true).text = Convert.ToInt32(nodeData.RegionData.RegionID).ToString();
        else
        {
            nodeFields[4].transform.GetChild(0).gameObject.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true).text = "virus";
        }
        ///* Power */
        nodeFields[5].GetComponentInChildren<Slider>(includeInactive: true).value = nodeData.Power;
        ///* Speed */
        nodeFields[6].GetComponentInChildren<Slider>(includeInactive: true).value = nodeData.Speed;
        ///* Resistance */
        nodeFields[7].GetComponentInChildren<Slider>(includeInactive: true).value = nodeData.ResistanceLimit;
        ///* Package queue */
        nodeFields[8].GetComponentInChildren<TextMeshProUGUI>(includeInactive: true).text = nodeData.NumberOfQueuedPackages().ToString();
        ///* Energy drain rate */
        nodeFields[9].GetComponentInChildren<Slider>(includeInactive: true).value = nodeData.EnergyDrainRatePerCycle;
        ///* Charging speed */
        nodeFields[10].GetComponentInChildren<Slider>(includeInactive: true).value = nodeData.ChargingRatePerSecond;
        ///* Duty cycle */
        nodeFields[11].GetComponentInChildren<Slider>(includeInactive: true).value = nodeData.DutyCycleDuration;
        NodePanel.SetActive(true);

        ///* Waveform */
        VisualizingNode = nodeData.gameObject;
        UpdateWaveformGraphicsAsync(true);
    }
    private void OpenRegionInfoPanel(Region regionData)
    {
        GameObject[] regionFields = new GameObject[RegionPanel.transform.childCount];
        for (int i = 0; i < RegionPanel.transform.childCount; i++)
        {
            //Debug.Log(RegionPanel.transform.GetChild(i));
            regionFields[i] = RegionPanel.transform.GetChild(i).gameObject;
        }
        /* Name */
        regionFields[0].GetComponentInChildren<TextMeshProUGUI>(includeInactive: true).text = "Region " + regionData.RegionID;
        /* Region ID */
        regionFields[1].GetComponentInChildren<TextMeshProUGUI>(includeInactive: true).text = Convert.ToInt32(regionData.RegionID).ToString();
        /* Node list */
        regionFields[2].GetComponentInChildren<TextMeshProUGUI>(includeInactive: true).text = "contains " + regionData.NodeList.Count + " nodes";

        /* List */
        Node[] nodes = regionData.NodeList.ToArray();
        for (int i=0; i < regionFields[3].transform.childCount; i++)
        {
            Transform tableEntry = regionFields[3].transform.GetChild(i);
            TextMeshProUGUI nodeStatusText;
            if (i < nodes.Length)
            {
                nodeStatusText = tableEntry.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                tableEntry.gameObject.GetComponent<TextMeshProUGUI>().text = nodes[i].Name;
                nodeStatusText.text = nodes[i].Status is Healthy ?
                    "healthy (" + (100 - (int)(((Healthy)nodes[i].Status).InfectionSteps / (float)nodes[i].ResistanceLimit * 100)) + "%)"
                    :
                    "infected";
                if (nodeStatusText.text == "infected")
                {
                    nodeStatusText.color = Color.red;
                }
                else nodeStatusText.color = Color.green;
                tableEntry.gameObject.SetActive(true);
            }
            else tableEntry.gameObject.SetActive(false);
        }
        /* Waveform */
        VisualizingRegion = regionData.gameObject;
        if (!RegionPanel.activeInHierarchy) UpdateWaveformGraphicsAsync(false);
        RegionPanel.SetActive(true);
    }
    private List<Vector3> CreateWaveformGraphicsSegment(ArraySegment<double> values, double min)
    {
        float minHzX = -607f;
        float maxHzX = 607f;
        float minPercentageY = -97f;
        float maxPercentageY = 97f;
        List<Vector3> positions = new()
        {
            new Vector3(
            x: Mathf.Lerp(minHzX, maxHzX, (float)(values.Offset - 1) / NumberOfFrequencies),
            y: Mathf.Lerp(minPercentageY, maxPercentageY, (float)(min / 5)),
            z: 0)
        };

        for (int i = values.Offset; i < values.Offset + values.Count; i++)
        {

            positions.Add(new Vector3(
                            x: Mathf.Lerp(minHzX, maxHzX, (float)i / NumberOfFrequencies),
                            y: Mathf.Lerp(minPercentageY, maxPercentageY, (float)(values.ElementAt(i - values.Offset) / 5)), // ANALIZZARE - OK
                            z: 0));
        }
        positions.Add(new Vector3(
            x: Mathf.Lerp(minHzX, maxHzX, (float)(values.Offset + values.Count) / NumberOfFrequencies),
            y: positions.ElementAt(0).y,
            z: 0));
        return positions;
    }

        //private Vector3[] CreateWaveformGraphics(float[] waveformToDisplay, float[] notMinValues) //funzionante
        //{
        //  
        //    float minHzX = -607f;
        //    float maxHzX = 607f;
        //    float minPercentageY = -97f;
        //    float maxPercentageY = 97f;
        //    int notMinValuesCount = notMinValues.Count();
        //    Vector3[] spectrumPositions = new Vector3[notMinValuesCount + 4];
        //    float min = waveformToDisplay.Min();
        //    int firstValueIndex = -1, lastValueIndex;
        //    for (int i = 0; i < waveformToDisplay.Length; i++)
        //    {
        //        if (waveformToDisplay[i] == min) continue;
        //        else
        //        {
        //            firstValueIndex = i;
        //            break;
        //        }
        //    }
        //    lastValueIndex = firstValueIndex + notMinValuesCount;
        //    Debug.Log(firstValueIndex + " " + lastValueIndex + " " + notMinValuesCount);
        //    //NativeArray<Vector3> native_spectrumPositions = new(spectrumPositions, Allocator.TempJob);
        //    //NativeArray<Vector3> native_results = new(spectrumPositions, Allocator.TempJob);
        //    //NativeArray<float> native_waveformToDisplay = new(notMinValues.ToArray(), Allocator.TempJob);

        //    spectrumPositions[0] = new Vector3(minHzX, Mathf.Lerp(minPercentageY, maxPercentageY, min/5), 0);
        //    spectrumPositions[1] = new Vector3(Mathf.Lerp(minHzX, maxHzX, (float)firstValueIndex / NumberOfFrequencies), spectrumPositions[0].y, 0);
        //    spectrumPositions[notMinValuesCount + 4 - 1] = new Vector3(maxHzX, spectrumPositions[0].y, 0);
        //    spectrumPositions[notMinValuesCount + 4 - 2] = new Vector3(Mathf.Lerp(minHzX, maxHzX, (float)lastValueIndex / NumberOfFrequencies), spectrumPositions[0].y, 0);
        //    //native_spectrumPositions[0] = new Vector3(minHzX, Mathf.Lerp(minPercentageY, maxPercentageY, min), 0);
        //    //native_spectrumPositions[1] = new Vector3(Mathf.Lerp(minHzX, maxHzX, (float)firstValueIndex / NumberOfFrequencies), native_spectrumPositions[0].y, 0);
        //    //native_spectrumPositions[notMinValues.Count() + 4 - 1] = new Vector3(maxHzX, native_spectrumPositions[0].y, 0);
        //    //native_spectrumPositions[notMinValues.Count() + 4 - 2] = new Vector3(Mathf.Lerp(minHzX, maxHzX, (float)lastValueIndex / NumberOfFrequencies), native_spectrumPositions[0].y, 0);
        //    int j = 0;
        //    for (int i = 0; i < spectrumPositions.Length; i++)
        //    {
        //        if (i < 2 || i >= spectrumPositions.Length - 2) continue;
        //        else
        //        {
        //            spectrumPositions[i].x = Mathf.Lerp(minHzX, maxHzX, (float)(firstValueIndex + i) / NumberOfFrequencies);
        //            spectrumPositions[i].y = Mathf.Lerp(minPercentageY, maxPercentageY, notMinValues[j] / 5);
        //            spectrumPositions[i].z = 0;
        //        };
        //        j++;
        //    }
        //    //WaveformGraphicsJob generateGraphicsParallelJob = new()
        //    //{
        //    //    ControlPoints = native_spectrumPositions,
        //    //    Results = native_results,
        //    //    WaveformToDisplay = native_waveformToDisplay,
        //    //    minHzX = -607f,
        //    //    maxHzX = 607f,
        //    //    minPercentageY = -97f,
        //    //    maxPercentageY = 97f,
        //    //    firstValueIndex = firstValueIndex
        //    //};
        //    //JobHandle handle = generateGraphicsParallelJob.Schedule(native_spectrumPositions.Length, native_results.Length);
        //    //handle.Complete();
        //    //native_spectrumPositions.Dispose();
        //    //native_waveformToDisplay.Dispose();
        //    //native_results.CopyTo(spectrumPositions);
        //    //native_results.Dispose();
        //    return spectrumPositions;
        //}
    public List<ArraySegment<double>> GetChunks(double[] waveform, double min)
    {
        int chunkStartIndex = -1, chunkEndIndex;
        bool inChunk = false;
        List<ArraySegment<double>> notMinValuesChunks = new();
        for (int i = 0; i < waveform.Length; i++)
        {
            if (waveform[i] == min) {
                if (inChunk)
                {
                    chunkEndIndex = i-1;
                    //Debug.Log("(" + i + ", " + waveform[i] + ")");
                    ArraySegment<double> chunk = new(waveform, chunkStartIndex, chunkEndIndex - chunkStartIndex + 1);
                    //UnityEngine.Debug.Log("found chunk from index " + chunkStartIndex + " to " + chunkEndIndex);
                    notMinValuesChunks.Add(chunk);
                    inChunk = false;
                }
            }
            else
            {
                //Debug.Log("(" + i + ", " + waveform[i] + ")");
                if (!inChunk)
                {
                    chunkStartIndex = i;
                    inChunk = true;
                }
            }
        }
        //Debug.Log("found " + notMinValuesChunks.Count + " chunks");
        return notMinValuesChunks;
    }
    private Vector3[] UpdateWaveformGraphicsTask(List<ArraySegment<double>> notMinValuesChunks, double min)
    {
        List<Vector3> spectrumPositions = new()
            {
                new Vector3(
                x: minHzX,
                y: Mathf.Lerp(minPercentageY, maxPercentageY, (float)(min / 5)),
                z: 0)
            };

        foreach (ArraySegment<double> chunk in notMinValuesChunks)
        {
            // ogni chiamata a questa funzione genera i due punti minimi di delimitazione di ogni picco
            spectrumPositions.AddRange(CreateWaveformGraphicsSegment(chunk, min));
        }
        spectrumPositions.Add(new Vector3(
            x: maxHzX,
            y: Mathf.Lerp(minPercentageY, maxPercentageY, (float)(min / 5)),
            z: 0)
        );
        return spectrumPositions.ToArray();
    }
    public async void UpdateWaveformGraphicsAsync(bool isNode)
    {
        LineRenderer spectrumLine;
        double[] waveformToDisplay;
        //float minHzX = -607f;
        //float maxHzX = 607f;
        //float minPercentageY = -97f;
        //float maxPercentageY = 97f;
        if (isNode) {
            spectrumLine = NodePanel.GetComponentInChildren<LineRenderer>();
            waveformToDisplay = VisualizingNode.GetComponent<Node>().Tone.GetSpectrum(gameObject);
        }
        else {
            spectrumLine = RegionPanel.GetComponentInChildren<LineRenderer>();
            waveformToDisplay = VisualizingRegion.GetComponent<Region>().RegionWaveform.GetSpectrum(gameObject);
        }
        double min = waveformToDisplay.Min();

        List<ArraySegment<double>> notMinValuesChunks = GetChunks(waveformToDisplay, min);
        // !!!!!!
        int newPositionCount = 2;
        foreach (ArraySegment<double> chunk in notMinValuesChunks)
        {
            newPositionCount += chunk.Count + 2;
        }
        Vector3[] spectrumPositions = await Task.Run(() => { return UpdateWaveformGraphicsTask(notMinValuesChunks, min); });
        spectrumLine.positionCount = newPositionCount;
        spectrumLine.SetPositions(spectrumPositions);
    }
    //public async void UpdateWaveformGraphicsAsync(bool isNode) //funzionante
    //{
    //    LineRenderer spectrumLine;
    //    float[] waveformToDisplay;
    //    if (isNode) {
    //        spectrumLine = NodePanel.GetComponentInChildren<LineRenderer>();
    //        waveformToDisplay = VisualizingNode.GetComponent<Node>().Tone.GetSpectrum(gameObject);
    //    }
    //    else {
    //        spectrumLine = RegionPanel.GetComponentInChildren<LineRenderer>();
    //        waveformToDisplay = VisualizingRegion.GetComponent<Region>().RegionWaveform.GetSpectrum(gameObject);
    //    }
    //    float min = waveformToDisplay.Min();
    //    float[] notMinValues = waveformToDisplay.Where(s => s != min).ToArray();
    //    foreach (float s in notMinValues) Debug.Log(s);
    //    spectrumLine.positionCount = notMinValues.Count() + 4;
    //    Vector3[] spectrumPositions = await Task.Run(() => { return CreateWaveformGraphics(waveformToDisplay, notMinValues); });
    //    spectrumLine.SetPositions(spectrumPositions);
    //    //Task update = new(() => { CreateWaveformGraphics(spectrumLine, waveformToDisplay); });
    //}
    public void UpdateNodeGraphics() 
    {
        foreach (Region r in SimulationManagerInstance.RegionList)
        {
            foreach (Node n in r.NodeList)
            {
                if (n.Status is Healthy)
                {
                    Healthy status = n.Status as Healthy;
                    status.gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, (float)status.InfectionSteps / n.ResistanceLimit);
                }
            }
        }
    }


    private void Awake()
    {
        if (SimulationVisualizerInstance == null)
        {
            SimulationVisualizerInstance = this;
            _originalPosition = gameObject.transform.position;
            DontDestroyOnLoad(gameObject);
            controls = new Controls();
            OnZoomIn = new();
            OnZoomOut = new();
            OnShowNodeInfo = new();
            OnShowRegionInfo = new();
            GameObject panels = transform.GetChild(0).gameObject;
            NodePanel = panels.transform.GetChild(0).gameObject;
            RegionPanel = panels.transform.GetChild(1).gameObject;
            OnZoomIn.AddListener(ZoomIn);
            OnZoomOut.AddListener(ZoomOut);
            OnShowNodeInfo.AddListener(OpenNodeInfoPanel);
            OnShowRegionInfo.AddListener(OpenRegionInfoPanel);
            controls.SimulationInteraction.Enable();
            controls.SimulationInteraction.ZoomIn.performed += ZoomInAction;
            controls.SimulationInteraction.ZoomOut.performed += ZoomOutAction;
            VisualizingNode = null;
            VisualizingRegion = null;
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
        UpdateNodeGraphics();
        DrawEncounters();
    }
    private void LateUpdate()
    {
        if (Camera.main.orthographicSize == NodeOrtographicSize)
        {
            Camera.main.transform.position = VisualizingNode.transform.position;
        }
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
