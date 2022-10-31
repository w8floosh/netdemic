using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InterfaceComponent;

[Serializable]
public abstract class NodeState : MonoBehaviour, INodeState
{
    public abstract void SendPackage(Encounter encounter, Package b);
    public abstract void RcvPackage(Package b);
}
