using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using QTESystem;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System;

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
    

    private void OnEnable()
    {
        m_streamData = (QTEStreamData)target;
        m_action = new QTEHoldAction();
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
        m_addNewAction.clicked += CreateAction;
        //GetActionList();
        customInspector.Add(new Label("QTE Stream Creator"));
        customInspector.Add(new Label("Time per Action (in seconds)"));
        customInspector.Add(m_timePerAction);
        customInspector.Add(new Label("Pause At Start (in seconds)"));
        customInspector.Add(m_pauseAtStart);
        customInspector.Add(new Label("Pause At End (in seconds)"));
        customInspector.Add(m_pauseAtEnd);
        customInspector.Add(new Label("Success Window (in seconds)"));
        customInspector.Add(m_successBuffer);
        customInspector.Add(new Label("Action List"));
        customInspector.Add(m_action);
        customInspector.Add(m_addNewAction);
        GetData();
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
    }


    public void SetData()
    {
        m_streamData.ActionTimer = m_timePerAction.value;
        m_streamData.BeginningOfStreamPause = m_pauseAtStart.value;
        m_streamData.EndOfStreamPause = m_pauseAtEnd.value;
        m_streamData.SuccessBuffer = m_successBuffer.value;
    }

    void GetActionList()
    {
        //m_actionList.Clear();
        //for(int i = 0; i < streamData.Actions.Count; i++)
        //{
        //    m_actionList.Add(streamData.Actions[i]);
        //}
    }

    void SetActionList()
    {
        //streamData.Actions.Clear();
        //for(int i = 0; i < m_actionList.Count; i++)
        //{
        //    streamData.Actions.Add(m_actionList[i]);
        //}
    }

    public void CreateAction()
    {
        CreateInstance<NewActionPopUp>();
        NewActionPopUp.Init();
    }

    public void AddAction(QTEAction _action)
    {

    }

    public void RemoveAction(QTEAction _action)
    {

    }
}
