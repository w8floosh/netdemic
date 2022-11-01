using System;
using System.Collections;
using UnityEngine;
using static InterfaceComponent;
using static SimulationManager;


[Serializable]
public class SecureEncounter : Encounter
{
    override public bool IsNear()
    {
        Debug.Log("Distanza " + name + ": " + Vector3.Distance(Source.transform.position, Destination.transform.position));
        Debug.Log("Potenza sorgente: " + Source.Power + " " + (Vector3.Distance(Source.transform.position, Destination.transform.position) > Source.Power ? "IRRAGGIUNGIBILE" : "RAGGIUNGIBILE"));

        if (Vector3.Distance(Source.transform.position, Destination.transform.position) > Source.Power) return false;
        return true;
    }

    override public void SetupEncounter(Node src, Node dst)
    {
        Source = src;
        Destination = dst;
        Edge = gameObject.AddComponent<LineRenderer>();
        Edge.enabled = false;
        Edge.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        Edge.SetPosition(0, Source.transform.position);
        Edge.SetPosition(1, Destination.transform.position);
        Edge.startColor = Color.cyan;
        Edge.endColor = Color.magenta;
        Edge.startWidth = .01f;
        Edge.endWidth = .05f;
        Edge.numCapVertices = 16;
        IsBusy = false;
        // TROIAIO
        //SimulationManagerInstance.EncounterList.Add(gameObject);
    }
    private void Awake()
    {
        TimeElapsed = 0;
    }
    private void Update()
    {
        TimeElapsed += Time.deltaTime;
    }

}
