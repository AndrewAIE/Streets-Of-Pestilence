using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using QTESystem;
using Unity.VisualScripting;


public class NewActionPopUp : EditorWindow
{
    static QTEStreamEditor m_editor;
    [MenuItem("Example/Popup Example")]
    static public void Init(QTEStreamEditor _editor)
    {
        EditorWindow window = CreateInstance<NewActionPopUp>();
        m_editor = _editor;
        window.Show();        
    } 

    public void SetEditor(QTEStreamEditor _editor)
    {
        m_editor = _editor;
    }

    public void CreateGUI()
    {
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/PLAYER/QTESYSTEM/Editor/NewActionPopUp.uxml");
        visualTreeAsset.CloneTree(rootVisualElement);
        
        var buttonPress = rootVisualElement.Q<Button>("Press");
        buttonPress.clicked += () => CreateAction(0);

        var buttonHold = rootVisualElement.Q<Button>("Hold");
        buttonHold.clicked += () => CreateAction(1);

        var buttonMash = rootVisualElement.Q<Button>("Mash");
        buttonMash.clicked += () => CreateAction(2);

        var buttonSequence = rootVisualElement.Q<Button>("Sequence");
        buttonSequence.clicked += () => CreateAction(3);
    }

    public void CreateAction(int _indicator)
    {
        Debug.Log("Create Action type " + _indicator);        
        m_editor.AddAction(_indicator);
    }

    private void OnLostFocus()
    {
        this.Close();   
    }
}