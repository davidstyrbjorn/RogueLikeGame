using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Base class for displaying events in the game
/// Other classes will call the addEvent method which adds event to the events list
/// This class then presents the list of events and checks for which ones to remove
/// </summary>

public class EventBox : MonoBehaviour {

    private List<string> events = new List<string>();
    private int maxNumberOfEvents = 5;

    public Text eventText;

    public void addEvent(string _event)
    {
        if (events.Count < maxNumberOfEvents)
        {
            // We have room so just add the event 
            events.Insert(0, _event);
        }
        else
        {
            // This will be executed if the events is filled to the brink 
            events.RemoveAt(4);
            events.Insert(0, _event);
        }
        DrawText();
    }

    void DrawText()
    {
        eventText.text = string.Empty;
        for(int i = 0; i < events.Count; i++)
        {
            eventText.text += events[i] + "\n";
        }
    }

}
