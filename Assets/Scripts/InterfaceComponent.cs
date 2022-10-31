using System;
using System.Collections.Generic;
using UnityEngine;
using static SimulationManager;

public class InterfaceComponent
{
    [Serializable]
    public abstract class Encounter : MonoBehaviour
    {
        public Node Source;
        public Node Destination;
        public LineRenderer Edge;
        public bool IsBusy;
        public float TimeElapsed { get; protected set; }
        public abstract bool IsNear();
        public abstract void SetupEncounter(Node src, Node dst);
        public void CloseEncounter()
        {
            // TROIAIO
            gameObject.SetActive(false);
            //SimulationManagerInstance.EncounterList.RemoveAll(e => e == gameObject);
            Source.GetComponent<Node>().CurrentEncounters.RemoveAll(e => e == gameObject);
            Destroy(gameObject);
        }


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
        List<GameObject> RefreshEncounters(List<Collider2D> nodes);
    }

}
