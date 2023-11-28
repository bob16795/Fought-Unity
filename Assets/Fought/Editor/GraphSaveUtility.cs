using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
public class GraphSaveUtility
{
    private MoveGraphView _targetGraphView;
    private MoveContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<MoveNode> Nodes => _targetGraphView.nodes.Cast<MoveNode>().ToList();
    public static GraphSaveUtility GetInstance(MoveGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView,
        };
    }

    public void SaveGraph(string fileName)
    {
        if (!Edges.Any()) return;

        var moveContainer = ScriptableObject.CreateInstance<MoveContainer>();
        moveContainer.links = new List<MoveNodeLinkData>();
        moveContainer.nodes = new List<MoveNodeData>();

        var connected = Edges.Where(x => x.input.node != null).ToArray();

        for (int i = 0; i < connected.Length; i++)
        {
            var outputNode = connected[i].output.node as MoveNode;
            var inputNode = connected[i].input.node as MoveNode;

            moveContainer.links.Add(new MoveNodeLinkData
            {
                BaseNodeGUID = outputNode.GUID,
                PortName = connected[i].output.portName,
                TargetNodeGUID = inputNode.GUID,
            });
        }

        foreach (var moveNode in Nodes.Where(node => !node.EntryPoint).ToArray())
        {
            moveContainer.nodes.Add(new MoveNodeData
            {
                NodeGUID = moveNode.GUID,
                Position = moveNode.GetPosition().position,
                Name = moveNode.name,
                Time = moveNode.time,
            });
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        AssetDatabase.CreateAsset(moveContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string fileName)
    {
        Debug.Log(fileName);
        _containerCache = Resources.Load<MoveContainer>(fileName);
        if (_containerCache == null)
        {
            EditorUtility.DisplayDialog("No such file", "The specified file dosent exits!", "OK");

            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    private void ClearGraph()
    {
        Nodes.Find(x => x.EntryPoint).GUID = _containerCache.links[0].BaseNodeGUID;
        foreach (var node in Nodes)
        {
            if (node.EntryPoint) continue;

            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

            _targetGraphView.RemoveElement(node);
        }
    }

    private void CreateNodes()
    {
        foreach (var nodeData in _containerCache.nodes)
        {
            var tempNode = _targetGraphView.CreateMoveNode(nodeData.Name, nodeData.Time);
            tempNode.GUID = nodeData.NodeGUID;
            tempNode.SetPosition(new Rect(nodeData.Position.x, nodeData.Position.y, 150, 300));

            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.links.Where(x => x.BaseNodeGUID == nodeData.NodeGUID).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddInputPort(tempNode, x.PortName));
        }
    }

    private void ConnectNodes()
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            var connections = _containerCache.links.Where(x => x.BaseNodeGUID == Nodes[i].GUID).ToList();

            for (var j = 0; j < connections.Count; j++)
            {
                var targetNodeGUID = connections[j].TargetNodeGUID;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var edge = new Edge
        {
            output = output,
            input = input,
        };

        edge.input.Connect(edge);
        edge.output.Connect(edge);

        _targetGraphView.Add(edge);
    }
}
