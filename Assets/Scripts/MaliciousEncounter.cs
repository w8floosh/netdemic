using System;
using UnityEngine;
using static InterfaceComponent;


[Serializable]
public class MaliciousEncounter : Encounter
{
    public float InfectionProgress { get; set; }
    override public bool IsNear()
    {
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
        Edge.startColor = new Color(255, 165, 0);
        Edge.endColor = Color.yellow;
        Edge.startWidth = .01f;
        Edge.endWidth = .05f;
        Edge.numCapVertices = 16;
        IsBusy = false;
        // TROIAIO
        //SimulationManagerInstance.EncounterList.Add(gameObject);
    }
    /*public IEnumerator InfectionCoroutine(Node victim)
    {
        //yield return new WaitForSeconds(0.1);
    }*/
    private void Awake()
    {
        TimeElapsed = 0;
    }

    private void Update()
    {
        TimeElapsed += Time.deltaTime;
    }
}
