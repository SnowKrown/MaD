/* ArduinoConnector by Alan Zucconi
 * http://www.alanzucconi.com/?p=2979
 */
using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;

public class ArduinoConnector : MonoBehaviour 
{

	[SerializeField, Tooltip("The serial port where the Arduino is connected")]
	private string port = "COM4";
	[SerializeField, Tooltip("The baudrate of the serial port")]
	private int baudrate = 9600;

    private SerialPort stream;
	private bool isOpened;

	private void Start ()
	{
		isOpened = false;
		Open ();
	}

	private void Update ()
	{
		if (stream.IsOpen && !isOpened)
		{
			isOpened = true;
			Debug.Log ("Serial port opened");
		}
	}
	
	public void Open () 
	{
		stream = new SerialPort(port, baudrate);
		stream.ReadTimeout = 50;
		stream.Open();
//		stream.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
	}

    public void WriteToArduino(string message)
    {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    public string ReadFromArduino(int timeout = 0)
    {
        stream.ReadTimeout = timeout;
        try
        {
            return stream.ReadLine();
        }
        catch (TimeoutException)
        {
            return null;
        }
    }
    

    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = null;

        do
        {
            try
            {
                dataString = stream.ReadLine();
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                callback(dataString);
                yield return null;
            } else
                yield return new WaitForSeconds(0.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
		
        yield return null;
    }

    public void Close()
    {
        stream.Close();
		Debug.Log ("Serial port closed.");
    }

	private void OnApplicationQuit ()
	{
		if (stream.IsOpen)
			Close ();
	}
}