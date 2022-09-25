using System;
using System.Collections;
using UnityEngine;
using static InterfaceComponent;
using static Node;

public class EncounterComponent : MonoBehaviour
{
    [Serializable]
    public class SecureEncounter : Encounter
    {
        public int TimeElapsed { get; }

        override public void IsNear()
        {

        }
        override public void CloseEncounter()
        {

        }
        public SecureEncounter(Node src, Node dst) : base(src, dst)
        {
            TimeElapsed = 0;
        }
    }
    [Serializable]
    public class MaliciousEncounter : Encounter
    {
        public int TimeElapsed { get; }
        public float InfectionProgress { get; set; }
        override public void IsNear()
        {

        }
        override public void CloseEncounter()
        {

        }
        /*public IEnumerator InfectionCoroutine(Node victim)
        {
            //yield return new WaitForSeconds(0.1);
        }*/
        public MaliciousEncounter(Node src, Node dst): base(src, dst)
        {
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
