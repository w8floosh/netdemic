using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SettingsMenu : MonoBehaviour
{
    public static float BroadcastFrequency = .5f;
    public static float EncounterChance = .1f;
    public static float ConnectionDropChance = .06f;
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
     * 
     * Risultati ottenuti [numero di nodi, encounter totali, pacchetti totali, tempo totale in secondi]:
     * 1)
     * 2)
     * 3)
     * 4)
     * 5)
     * 6) [622, 38772, 42704, 46.60674], [627, 34662, 38525, 44.76921], [540, 35400, 41016, 70.46951]
     * 7) [595, 110196, 34788, 145.1699], [500, 86976, 28554, 150.6111], [632, 121568, 37777, 154.1642]
     * 8) 
     * 
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
