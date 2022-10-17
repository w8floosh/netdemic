using System;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceComponent
{
    [Serializable]
    public abstract class Encounter : MonoBehaviour
    {
        public Node Source;
        public Node Destination;
        public LineRenderer Edge;
        public SimulationManager SimulationManagerInstance;
        public abstract bool IsNear();
        public abstract void CloseEncounter();

    }

    public interface INodeState
    {
        void SendPackage(Encounter encounter, Package b);
        void RcvPackage(Package b);

    }

    public interface INode
    {
        GameObject EstablishConnection(GameObject other);
        void ShowInfo();
        void PackageEnqueue(Package p);
        Package PackageDequeue();
        List<GameObject> RefreshEncounters();
    }

}
