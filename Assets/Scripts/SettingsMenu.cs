using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SettingsMenu : MonoBehaviour
{
    public static float BroadcastFrequency = 1f;
    public static float EncounterChance = .01f;
    public static float ConnectionDropChance = .01f;
    public static float TransmissionChance = .05f;
    public static byte NumberOfRegions = 2;
    public static byte MaxNodesPerRegion = 8;
    public static byte StartingViralLoad = 1;

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
