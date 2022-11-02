using Codice.CM.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InterfaceComponent;
using static SimulationVisualizer;

[Serializable]
public class Infected : NodeState
{
    [SerializeField] private float _infectionSpeed;
    public float InfectionSpeed
    {
        get { return _infectionSpeed; }
        private set { _infectionSpeed = value; }
    }

    override public void SendPackage(Encounter mEncounter, Package toSend)
    {
        StartCoroutine(InfectionCoroutine(mEncounter, toSend));
    }

    override public void RcvPackage(Package toSend)
    {
        UnityEngine.Debug.Log("butto il pacchetto proveniente da (" + toSend.Destination + ", " + toSend.DestinationRegionID + ")");
    }
    public void Setup(float infectionSpeed)
    {
        _infectionSpeed = infectionSpeed;
    }
    // metterlo in una coroutine è sbagliato?
    public IEnumerator InfectionCoroutine(Encounter encounter, Package toSend, float delay = 0)
    {
        if (encounter.Destination.Status is Infected) encounter.CloseEncounter();
        encounter.IsBusy = true;
        if (delay == 0) yield return null;
        else yield return new WaitForSeconds(delay);
        //Debug.Log(encounter.Destination.Name + " received package " + toSend.ID + " after " + (delay * 100) + "ms");
        encounter.Destination.Status.RcvPackage(toSend);
        if (encounter.Destination.Status is Healthy victim)
        {
            victim.InfectionSteps++;
            Waveform.InterpolateWaveforms(encounter.Source.Tone, encounter.Destination.OriginalTone, victim.InfectionSteps);
        }
        encounter.IsBusy = false;
    }
    void Awake()
    {
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }
}
