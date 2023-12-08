using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MoveController : MonoBehaviour
{
    [SerializeField]
    public MoveContainer moves;

    [SerializeField]
    public UnityEvent<string> onSwitch;

    private string _action;
    private string action { get {
            return _action;
        } set {
            onSwitch.Invoke(value);
            _action = value;
        } }
    private string actionGUID = "";
    private float timeout = 0;

    public void Action(string name)
    {
        if (actionGUID == "") {
            MoveNodeData targ = moves.nodes.Where((e) => e.Name == "Entry").First();
            if (targ != null) {
                action = targ.Name;
                timeout = targ.Time;
                actionGUID = targ.NodeGUID;
                Debug.Log(targ.Name);
            }
        }

        foreach (MoveNodeLinkData link in moves.links) {
            if (link.BaseNodeGUID == actionGUID && link.PortName == name) {
                MoveNodeData targ = moves.nodes.Where((e) => e.NodeGUID == link.TargetNodeGUID).First();
                if (targ != null) {
                    action = targ.Name;
                    actionGUID = link.TargetNodeGUID;
                    timeout = targ.Time;
                    Debug.Log(targ.Name);
                } else {
                    continue;
                }
            }
        }
    }

    public void Update() {
        if (timeout > 0) {
            timeout -= Time.deltaTime;
        } else if (action != "None") {
            action = "None";
            actionGUID = "";
            Debug.Log("None");
        }
    }
}
