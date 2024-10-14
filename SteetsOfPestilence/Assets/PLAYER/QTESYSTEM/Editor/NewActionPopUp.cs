using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using PopupWindow = UnityEditor.PopupWindow;


public class NewActionPopUp : EditorWindow
{
    [MenuItem("Example/Popup Example")]
    static public void Init()
    {
        EditorWindow window = CreateInstance<NewActionPopUp>();
        window.Show();
    }

    public void CreateGUI()
    {
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/PLAYER/QTESYSTEM/Editor/NewActionPopUp.uxml");
        visualTreeAsset.CloneTree(rootVisualElement);

        var button = rootVisualElement.Q<Button>();
        button.clicked += () => PopupWindow.Show(button.worldBound, new NewActionPopupContent());
    }
}