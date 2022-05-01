using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent OnClickEvent;

    public string Message;
    private void Start()
    {
        if (OnClickEvent == null)
            OnClickEvent = new UnityEvent();
    }
    public void Invoke()
    {
        OnClickEvent.Invoke();
    }
    public void Test()
    {
        Debug.Log("ch0j");
    }
}
