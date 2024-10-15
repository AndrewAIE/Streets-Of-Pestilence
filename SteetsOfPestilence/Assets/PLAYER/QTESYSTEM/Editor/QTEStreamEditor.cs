using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using QTESystem;
using UnityEngine.UIElements;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Timeline;

[CustomEditor(typeof(QTEStreamData))]
public class QTEStreamEditor : Editor
{
    //Stream Data to be edited
    QTEStreamData m_streamData;
    QTEAction m_action;
    
    //Visual Elements
    VisualElement customInspector;
    FloatField m_timePerAction;
    FloatField m_pauseAtStart;
    FloatField m_pauseAtEnd;
    FloatField m_successBuffer;
    Button m_addNewAction;
    List<Label> m_actionLabels;
    List<Label> m_inputListLabels;
    List<QTEAction> m_actionList;
    List<Button> m_removeAction;
    List<Button> m_moveActionUp;
    List<Button> m_moveActionDown;
    List<List<EnumField>> m_inputEnums;

    string m_filePath = "PLAYER/QTESYSTEM/ScriptableObjects";
    bool m_gotData = false;
    int InputCount;

    private void OnEnable()
    {
        m_streamData = (QTEStreamData)target;
        m_gotData = false;
        m_actionLabels = new List<Label>();
        m_inputListLabels = new List<Label>();
        m_removeAction = new List<Button>();        
        m_moveActionUp = new List<Button>();
        m_moveActionDown = new List<Button>();
        m_inputEnums = new List<List<EnumField>>();
        m_actionList = new List<QTEAction>();
        InputCount = 0;
    }


    public override VisualElement CreateInspectorGUI()
    {        
        customInspector = new VisualElement();
        m_timePerAction = new FloatField();
        m_pauseAtStart = new FloatField();
        m_pauseAtEnd = new FloatField();
        m_successBuffer = new FloatField();
        m_addNewAction = new Button();
        m_addNewAction.text = "Add New Action";
        m_addNewAction.clicked += CreateActionWindow;
        GetData();
        //Get Time Data from actions and apply them to 
        customInspector.Add(new Label("QTE Stream Creator"));
        customInspector.Add(new Label("QTE Metronome (in seconds)"));
        customInspector.Add(m_timePerAction);
        customInspector.Add(new Label("Pause At Start (in seconds)"));
        customInspector.Add(m_pauseAtStart);
        customInspector.Add(new Label("Pause At End (in seconds)"));
        customInspector.Add(m_pauseAtEnd);
        customInspector.Add(new Label("Success Window (in seconds)"));
        customInspector.Add(m_successBuffer);
        customInspector.Add(new Label("Action List"));
        for(int i = 0; i < m_actionList.Count; i++)
        {
            m_actionLabels.Add(new Label(m_actionList[i].GetType().ToString()));
            customInspector.Add(m_actionLabels[i]);
            m_inputListLabels.Add(new Label("Input List"));
            customInspector.Add(m_inputListLabels[i]);
            //Get Data and create input lists from actions
            m_inputEnums.Add(new List<EnumField>());
            for (int j = 0; j < m_actionList[i].InputList.Count; j++)
            {
                m_inputEnums[i].Add(new EnumField(m_actionList[i].InputList[j]));
                customInspector.Add(m_inputEnums[i][j]);
                InputCount++;
            }
            //Create Buttons for each Action
            m_moveActionUp.Add(new Button());            
            m_moveActionUp[i].text = "Move Action Up";
            m_moveActionUp[i].clicked += () => MoveActionUp(i - 1);
            customInspector.Add(m_moveActionUp[i]);
            m_moveActionDown.Add(new Button());
            m_moveActionDown[i].text = "Move Action Down";
            m_moveActionDown[i].clicked += () => MoveActionDown(i - 1);
            customInspector.Add(m_moveActionDown[i]);
            m_removeAction.Add(new Button());
            m_removeAction[i].text = "Remove Action";
            m_removeAction[i].clicked += () => RemoveAction(i - 1);
            customInspector.Add(m_removeAction[i]);            
        }
        customInspector.Add(m_addNewAction);        
        return customInspector;
    }

    public void OnValidate()
    {        
        
    }

    public void GetData()
    {
        m_timePerAction.value = m_streamData.ActionTimer;
        m_pauseAtStart.value = m_streamData.BeginningOfStreamPause;
        m_pauseAtEnd.value = m_streamData.EndOfStreamPause;
        m_successBuffer.value = m_streamData.SuccessBuffer;
        for(int i = 0; i < m_streamData.Actions.Count; i++)
        {
            m_actionList.Add(m_streamData.Actions[i]);
        }        
        m_gotData = true;
    }


    public void SetData()
    {
        if(m_gotData)
        {
            m_streamData.ActionTimer = m_timePerAction.value;
            m_streamData.BeginningOfStreamPause = m_pauseAtStart.value;
            m_streamData.EndOfStreamPause = m_pauseAtEnd.value;
            m_streamData.SuccessBuffer = m_successBuffer.value;
            m_streamData.Actions.Clear();
            for(int i = 0; i < m_actionList.Count; i++)
            {
                m_streamData.Actions.Add(m_actionList[i]);
            }
        }        
    }
   

    public void CreateActionWindow()
    {
        CreateInstance<NewActionPopUp>();        
        NewActionPopUp.Init(this);        
    }

    public void AddAction(int _indicator)
    {
        string filePath = Application.persistentDataPath + "/" + m_filePath + "/" + m_streamData.name + "/";
        Debug.Log(filePath);
        Directory.CreateDirectory(filePath);
        filePath = "Assets/" + m_filePath + "/" + m_actionList.Count + ".asset";
        switch(_indicator)
        {
            case 0:
                AssetDatabase.CreateAsset(CreateInstance<QTEPressAction>(), filePath);
                QTEPressAction pressAction = AssetDatabase.LoadAssetAtPath<QTEPressAction>(filePath);
                m_actionList.Add(pressAction);
                break;
            case 1:
                AssetDatabase.CreateAsset(CreateInstance<QTEHoldAction>(), filePath);
                QTEHoldAction holdAction = AssetDatabase.LoadAssetAtPath<QTEHoldAction>(filePath);
                m_actionList.Add(holdAction);
                break;
            case 2:
                AssetDatabase.CreateAsset(CreateInstance<QTEMashAction>(), filePath);
                QTEMashAction mashAction = AssetDatabase.LoadAssetAtPath<QTEMashAction>(filePath);
                m_actionList.Add(mashAction);
                break;
            case 3:
                AssetDatabase.CreateAsset(CreateInstance<QTESequenceAction>(), filePath);
                QTESequenceAction sequenceAction = AssetDatabase.LoadAssetAtPath<QTESequenceAction>(filePath);
                m_actionList.Add(sequenceAction);
                break;                
        }        
    }  

    public void RemoveAction(int _index)
    {
        Debug.Log("Size Of List = " + m_removeAction.Count);
        Debug.Log("Trying To Remove At " + _index);
        customInspector.Remove(m_inputListLabels[_index]);
        customInspector.Remove(m_actionLabels[_index]);
        customInspector.Remove(m_removeAction[_index]);
        customInspector.Remove(m_moveActionUp[_index]);
        customInspector.Remove(m_moveActionDown[_index]);
        foreach (EnumField entry in m_inputEnums[_index])
        {
            customInspector.Remove(entry);
        }
        m_inputListLabels.RemoveAt(_index); 
        m_actionLabels.RemoveAt(_index);
        m_streamData.Actions.RemoveAt(_index);
        m_removeAction.RemoveAt(_index);
        m_moveActionUp.RemoveAt(_index);
        m_moveActionDown.RemoveAt(_index);
        m_inputEnums.RemoveAt(_index);
        m_actionList.RemoveAt(_index);        
        AssetDatabase.Refresh();        
        Repaint();
    }

    public void MoveActionUp(int _index)
    {

    }

    public void MoveActionDown(int _index)
    {
        
    }

    
}
