using UnityEngine;
using System.Collections;

public class StateCommunicator : MonoBehaviour 
{
	private ArduinoConnector connector;

	private void Start () 
	{
		connector = FindObjectOfType <ArduinoConnector> ();
	}

	private void OnGUI ()
	{
		if (GUILayout.Button ("START"))
			connector.WriteToArduino ("START_ALERT");

		if (GUILayout.Button ("STOP"))
			connector.WriteToArduino ("STOP_ALERT");
	}
}