using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InterfaceComponent;

[Serializable]
public class MaliciousEncounter : Encounter
{
    public float TimeElapsed { get; }
    public float InfectionProgress { get; set; }
    override public bool IsNear()
    {
        return true;
    }
    override public void CloseEncounter()
    {

    }
    public void SetupEncounter(Node src, Node dst)
    {
        SimulationManagerInstance = SimulationManager.SimulationManagerInstance;
        Source = src;
        Destination = dst;
        Edge = gameObject.AddComponent<LineRenderer>();
        Edge.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        Edge.SetPosition(0, Source.transform.position);
        Edge.SetPosition(1, Destination.transform.position);
        Edge.startColor = Color.cyan;
        Edge.endColor = Color.magenta;
        Edge.startWidth = .05f;
        Edge.endWidth = .05f;
        SimulationManagerInstance.EncounterList.Add(gameObject);
    }
    /*public IEnumerator InfectionCoroutine(Node victim)
    {
        //yield return new WaitForSeconds(0.1);
    }*/

}
