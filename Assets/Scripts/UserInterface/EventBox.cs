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
    private List<string> eventsLog = new List<string>();

    public int maxNumberOfEvents = 13;
    private int maxNumberOfLogEvents = 40;

    public Text eventText;
    public Text logText;

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
            events.RemoveAt(maxNumberOfEvents-1);
            events.Insert(0, _event);
        }
        addEventLog(_event);
        DrawText();
    }

    public void addEventLog(string _event)
    {
        if (eventsLog.Count < maxNumberOfLogEvents)
        {
            // We have room so just add the event 
            eventsLog.Insert(0, _event);
        }
        else
        {
            // This will be executed if the events is filled to the brink 
            eventsLog.RemoveAt(maxNumberOfLogEvents - 1);
            eventsLog.Insert(0, _event);
        }
    }

    void DrawText()
    {
        eventText.text = string.Empty;
        for(int i = 0; i < events.Count; i++)
        {
            if(i == 0)
                eventText.text += "<size=24><color=#ffffffff>" + events[i] + "\n </color></size>";
            if(i == 1)
                eventText.text += "<size=24><color=#ffffffff>" + events[i] + "\n </color></size>";
            if (i == 2)
                eventText.text += "<size=24><color=#ffffff99>" + events[i] + "\n </color></size>";
            if (i == 3)
                eventText.text += "<size=24><color=#ffffff60>" + events[i] + "\n </color></size>";
            if (i == 4)
                eventText.text += "<size=24><color=#ffffff30>" + events[i] + "\n </color></size>";
        }

        logText.text = string.Empty;
        for(int i = 0; i < eventsLog.Count; i++)
        {
            logText.text += eventsLog[i] + "\n";
        }
    }
}