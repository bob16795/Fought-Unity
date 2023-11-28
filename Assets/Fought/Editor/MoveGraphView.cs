using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class MoveGraphView : GraphView
{
    public MoveGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        AddElement(GenerateEntryPointNode());
    }

    public Port GeneratePort(MoveNode node, Direction portDirection, Port.Capacity capacity)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    public void AddNode(MoveNode node)
    {
        AddElement(node);
    }

    public MoveNode CreateMoveNode(String name, float time)
    {
        var node = new MoveNode
        {
            title = "State Node",
            GUID = Guid.NewGuid().ToString(),
            EntryPoint = false,
            time = time,
            name = name,
        };

        var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Trigger";

        var button = new Button(() =>
        {
            AddInputPort(node);
        });
        button.text = "New Action";

        node.inputContainer.Add(inputPort);
        node.titleContainer.Add(button);

        var nameField = new TextField("State Name: ");
        nameField.value = name;
        nameField.RegisterValueChangedCallback(evt => node.name = evt.newValue);

        node.extensionContainer.Add(nameField);

        var timeField = new FloatField("State Timeout: ");
        timeField.value = time;
        timeField.RegisterValueChangedCallback(evt => node.time = evt.newValue);

        node.extensionContainer.Add(timeField);

        node.RefreshPorts();
        node.RefreshExpandedState();

        node.SetPosition(new Rect(0, 0, 150, 300));

        return node;
    }

    public void AddInputPort(MoveNode node, string name = "")
    {
        var outputPort = GeneratePort(node, Direction.Output, Port.Capacity.Single);

        var oldLabel = outputPort.contentContainer.Q<Label>("type");
        outputPort.contentContainer.Remove(oldLabel);

        node.RefreshPorts();

        var outputPortCount = node.outputContainer.Query("connector").ToList().Count;
        var choiceName = $"Action {outputPortCount}";

        if (name != "")
        {
            choiceName = name;
        }

        var textField = new TextField(string.Empty);
        textField.value = choiceName;
        textField.style.minWidth = 100;
        textField.style.maxWidth = 100;

        textField.RegisterValueChangedCallback(evt => outputPort.portName = evt.newValue);
        var deleteButton = new Button(() => RemovePort(node, outputPort))
        {
            text = "X",
        };

        outputPort.contentContainer.Add(deleteButton);
        outputPort.contentContainer.Add(new Label("  "));
        outputPort.contentContainer.Add(textField);
        outputPort.portName = choiceName;

        textField.MarkDirtyRepaint();
        outputPort.contentContainer.MarkDirtyRepaint();

        node.outputContainer.Add(outputPort);

        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    private void RemovePort(MoveNode node, Port port)
    {
        var targetEdges = edges.ToList().Where(x => x.output.portName == port.portName && x.output.node == port.node);

        if (targetEdges.Any()) {
            var edge = targetEdges.First();

            edge.input.Disconnect(edge);

            RemoveElement(edge);
        }

        node.outputContainer.Remove(port);

        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    private MoveNode GenerateEntryPointNode()
    {
        var node = new MoveNode
        {
            title = "Start",
            GUID = Guid.NewGuid().ToString(),
            EntryPoint = true,
        };

        node.SetPosition(new Rect(100, 100, 100, 300));

        var generatedPort = GeneratePort(node, Direction.Output, Port.Capacity.Single);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.RefreshPorts();
        node.RefreshExpandedState();

        return node;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port) =>
        {
            if (port == startPort) return;
            if (port.node == startPort.node) return;

            compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }
}
