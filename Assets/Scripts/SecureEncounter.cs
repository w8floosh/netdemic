using System;
using System.Collections;
using UnityEngine;
using static InterfaceComponent;

[Serializable]
public class SecureEncounter : Encounter
{
    public int TimeElapsed { get; }

    override public bool IsNear()
    {
        if (Vector3.Distance(Source.transform.position, Destination.transform.position) > Source.Power) return false;
        return true;
    }
    override public void CloseEncounter()
    {
        SimulationManagerInstance.EncounterList.Remove(gameObject);
        Destroy(gameObject);
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
    private void Awake()
    {
            
    }
    private void FixedUpdate()
    {
        if (!IsNear()) CloseEncounter();
    }

}
