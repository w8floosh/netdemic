using UnityEngine;
using static InterfaceComponent;
using static NodeComponent;

public class EncounterComponent : MonoBehaviour
{
    public class SecureEncounter : IEncounter
    {
        public INode Source { get; }
        public INode Destination { get; }
        public int TimeElapsed { get; }
        public void CheckDistance()
        {

        }
        public void CloseEncounter()
        {

        }
        public SecureEncounter(Node src, Node dst)
        {
            Source = src;
            Destination = dst;
            TimeElapsed = 0;
        }
    }
    public class MaliciousEncounter : IEncounter
    {
        public INode Source { get; }
        public INode Destination { get; }
        public int TimeElapsed { get; }
        public float InfectionProgress { get; set; }
        public void CheckDistance()
        {

        }
        public void CloseEncounter()
        {

        }
        public MaliciousEncounter(INode src, INode dst)
        {
            Source = src;
            Destination = dst;
            TimeElapsed = 0;
            InfectionProgress = 0f;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
