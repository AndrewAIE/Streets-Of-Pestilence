using QTESystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
[CustomEditor(typeof(QTEStreamData))]
public class QTEStreamEditor : Editor
{
    const string resourceFilename = "QTEStreamUie";
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement customInspector = new VisualElement();
        var visualTree = Resources.Load(resourceFilename) as VisualTreeAsset;
        visualTree.CloneTree(customInspector);
        customInspector.styleSheets.Add(Resources.Load($"{resourceFilename}-style") as StyleSheet);
        return customInspector;
    }    
}
