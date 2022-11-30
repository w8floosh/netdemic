using System;
using UnityEngine;
using static InterfaceComponent;

[Serializable]
public abstract class NodeState : MonoBehaviour, INodeState
{
    public abstract void SendPackage(Encounter encounter, Package b);
    public abstract void RcvPackage(Package b);
}
