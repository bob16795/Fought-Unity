using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEditor;

public class MoveGraph : EditorWindow
{
    private MoveGraphView _graphView;
    private string _fileName = "New Moves";

    [MenuItem("Graph/Move Graph")]
    public static void OpenMoveGraphWindow()
    {
        var window = GetWindow<MoveGraph>();
        window.titleContent = new GUIContent("Move Graph");
    }

    public void OnEnable()
    {
        ConstructGraphView();
        ConstructToolbar();
    }

    public void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

    private void ConstructGraphView()
    {
        _graphView = new MoveGraphView
        {
            name = "Move Graph",
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void ConstructToolbar()
    {
        var toolbar = new Toolbar();
        var nodeCreateButton = new Button(() =>
        {
            var node = _graphView.CreateMoveNode("New State", 0.5f);
            _graphView.AddNode(node);
        });
        nodeCreateButton.text = "Create Node";

        var filenameTextField = new TextField("File Name:");
        filenameTextField.SetValueWithoutNotify(_fileName);
        filenameTextField.MarkDirtyRepaint();
        filenameTextField.RegisterValueChangedCallback(evt =>
        {
            _fileName = evt.newValue;
        });

        var saveButton = new Button(() =>
        {
            RequestDataOperation(true);
        });
        saveButton.text = "Save";

        var loadButton = new Button(() =>
        {
            RequestDataOperation(false);
        });
        loadButton.text = "Load";

        toolbar.Add(nodeCreateButton);
        toolbar.Add(saveButton);
        toolbar.Add(loadButton);
        toolbar.Add(filenameTextField);
        _graphView.Add(toolbar);
    }

    public void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid File Name!", "You need a file name", "OK");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (save)
            saveUtility.SaveGraph(_fileName);
        else
            saveUtility.LoadGraph(_fileName);
    }
}
