using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Text;
using UnityScript.Steps;

public class ArduinoConnector : MonoBehaviour
{

    public const int OPERATION_TIMEOUT_MS = 50;
    
	/* The serial port where the Arduino is connected. */
    [Tooltip("The serial port where the Arduino is connected")]
    public string port = "COM4";
    /* The baudrate of the serial port. */
    [Tooltip("The baudrate of the serial port")]
    public int baudrate = 9600;
    
    public PlayerController player;

	private SerialPort stream;
    private int totalCommands = 0;

	public void Open(){
		stream = new SerialPort(port,baudrate);
		stream.ReadTimeout = OPERATION_TIMEOUT_MS;
		stream.Open();
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
            // A single read attempt
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

	public void WriteToArduino(string message)
    {
        // Send the request
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }
    
    IEnumerator Send(string command)
    {
        while(true)
        {
            WriteToArduino(command);
            ReadCallback(ReadFromArduino((int)OPERATION_TIMEOUT_MS));
            yield return new WaitForSeconds(OPERATION_TIMEOUT_MS/1000f);
            totalCommands++;
        }
    }
    
    
    public void SendOnce(string command)
    {
        WriteToArduino(command);
        //print("Mandando un PING");
        ReadCallback(ReadFromArduino((int)OPERATION_TIMEOUT_MS));
        totalCommands++;
    }
    

	public void Close()
    {
        stream.Close();
    }

	public void Start()
	{
	    player = GameObject.FindObjectOfType<PlayerController>();
		Open();
	    
	    StartCoroutine(Send("ANGLE"));
	}

    public void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            SendOnce("ANGLE");
        }*/
    }

    private void ReadCallback(string obj)
    {
        if (obj != null)
        {
            string[] temp = obj.Split(',');

            for (int i = 0; i < temp.Length; i++)
            {
                print(temp[i] + ": " + i);
            }

            player.angleX = float.Parse(temp[0]);
            player.angleZ = float.Parse(temp[1]);
        }

    }

    private void OnApplicationQuit()
    {
        Close();
    }
}
