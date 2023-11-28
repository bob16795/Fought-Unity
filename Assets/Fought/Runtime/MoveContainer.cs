using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MoveContainer : ScriptableObject {
    public List<MoveNodeLinkData> links;
    public List<MoveNodeData> nodes;
}
