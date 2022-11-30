using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SettingsMenu : MonoBehaviour
{
    public static float BroadcastFrequency = .5f;
    public static float EncounterChance = .1f;
    public static float ConnectionDropChance = .01f;
    public static float TransmissionChance = .5f;
    public static byte NumberOfRegions = 64;
    public static byte MaxNodesPerRegion = 16;
    public static byte StartingViralLoad = 32;

    /* TEST VALUTAZIONE METRICHE
     * Valori utilizzati:
     * 1) Dimensione della rete (NumberOfRegions e MaxNodesPerRegion)
     *      BASSA: 16 regioni * max 4 nodi
     *      MEDIA: 32 regioni * max 8 nodi
     *      ALTA: 64 regioni * max 16 nodi
     * 2) Stabilità dei collegamenti (ConnectionDropChance)
     *      BASSA: 15%
     *      MEDIA: 6%
     *      ALTA: 1%
     * 3) Attività di trasmissione (BroadcastFrequency, EncounterChance e TransmissionChance)
     *      BASSA: 1500ms, 1% e 5%
     *      MEDIA: 1000ms, 5% e 25%
     *      ALTA: 500ms, 10% e 50%
     * 4) Viralità (StartingViralLoad)
     *      BASSA: 8
     *      MEDIA: 32
     *      ALTA: 64
     *      
     * Configurazioni da utilizzare:
     * 1) MEDIA, MEDIA, MEDIA, ALTA
     * 2) MEDIA, ALTA,  ALTA,  ALTA
     * 3) MEDIA, ALTA,  MEDIA, ALTA
     * 4) MEDIA, MEDIA, ALTA,  ALTA
     * 5) ALTA, MEDIA, MEDIA, MEDIA
     * 6) ALTA, ALTA,  ALTA,  MEDIA
     * 7) ALTA, ALTA,  MEDIA, MEDIA
     * 8) ALTA, MEDIA, ALTA,  MEDIA
     * 9) MEDIA, BASSA, BASSA, ALTA
     * 10) ALTA, BASSA, BASSA, MEDIA
     * 
     * Risultati ottenuti [numero di nodi, encounter totali, pacchetti totali, tempo totale in secondi]:
     * 1) [158, 57103, 5696, 309.0791], [123, 37429, 3902, 334.2220], [122, 42372, 4263, 232.6972]
     * 2) [146, 5339, 6943, 55.72373], [128, 6147, 7963, 158.7843], [142, 6633, 8870, 88.48986]
     * 3) [143, 15081, 5798, 232.9222], [148, 15918, 5782, 316.2937], [175, 20527, 7191, 197.1092]
     * 4) [163, 23945, 8391, 81.00857], [140, 23869, 8261, 104.147], [131, 33930, 11313, 150.9478]
     * 5) [515, 402334, 33245, 215.0225], [535, 408284, 34145, 221.2366], [513, 345357, 29476, 233.1347]
     * 6) [622, 38772, 42704, 46.60674], [627, 34662, 38525, 44.76921], [540, 35400, 41016, 70.46951]
     * 7) [595, 110196, 34788, 145.1699], [500, 86976, 28554, 150.6111], [632, 121568, 37777, 154.1642]
     * 8) [557, 148801, 44820, 72.18525], [551, 164744, 49282, 88.14633], [586, 193534, 54497, 86.68715]
     * 9) [149, 769845, 4676, 7907.545], [152, 870215, 5161, 7765.583], [150, 882675, 5173, 9459.033]
     * 10) [482, 9777778, 46086, 9641.645], [523, 8167648, 39657, 8391.541], [549, 10070049, 47302, 8727.444]
     */

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetBroadcastFrequency(GameObject handleArea)
    {
        BroadcastFrequency = handleArea.transform.parent.GetComponent<Slider>().value;
        SetHandle((decimal)BroadcastFrequency, handleArea.GetComponentInChildren<TextMeshProUGUI>());
    }
    public void SetEncounterChance(GameObject handleArea)
    {
        EncounterChance = handleArea.transform.parent.GetComponent<Slider>().value;
        SetHandle((decimal)EncounterChance, handleArea.GetComponentInChildren<TextMeshProUGUI>());
    }
    public void SetConnectionDropChance(GameObject handleArea)
    {
        ConnectionDropChance = handleArea.transform.parent.GetComponent<Slider>().value;
        SetHandle((decimal)ConnectionDropChance, handleArea.GetComponentInChildren<TextMeshProUGUI>());
    }
    public void SetTransmissionChance(GameObject handleArea)
    {
        TransmissionChance = handleArea.transform.parent.GetComponent<Slider>().value;
        SetHandle((decimal)TransmissionChance, handleArea.GetComponentInChildren<TextMeshProUGUI>());
    }
    public void SetNumberOfRegions(GameObject handleArea)
    {
        NumberOfRegions = (byte)handleArea.transform.parent.GetComponent<Slider>().value;
        SetHandle(NumberOfRegions, handleArea.GetComponentInChildren<TextMeshProUGUI>());
    }
    public void SetMaxRegionCapacity(GameObject handleArea)
    {
        MaxNodesPerRegion = (byte)handleArea.transform.parent.GetComponent<Slider>().value;
        SetHandle(MaxNodesPerRegion, handleArea.GetComponentInChildren<TextMeshProUGUI>());

    }
    public void SetStartingViralLoad(GameObject handleArea)
    {
        StartingViralLoad = (byte)handleArea.transform.parent.GetComponent<Slider>().value;
        SetHandle(StartingViralLoad, handleArea.GetComponentInChildren<TextMeshProUGUI>());

    }
    public void SetHandle<T>(T value, TextMeshProUGUI currentValue)
    {
        if (value is decimal)
        {
            decimal v = Convert.ToDecimal(value);
            currentValue.text = Decimal.Round(v, 2).ToString();
        }
        else
            currentValue.text = value.ToString();
    }
    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(BroadcastFrequency + " " + EncounterChance + " " + ConnectionDropChance + " " + TransmissionChance + " " + NumberOfRegions + " " + StartingViralLoad + " " + MaxNodesPerRegion);

    }
}
