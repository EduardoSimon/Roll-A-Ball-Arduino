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
    
    [Header("Connection Options")]
    [Tooltip("The serial port where the Arduino is connected")]
    public string port = "COM4";
    
    [Tooltip("The baudrate of the serial port")]
    public int baudrate = 9600;
    
    [Header("Player Controller Reference")]
    public PlayerController player;

    [Header("Active Command")]
    public ECommands activeCommand;
    
	private SerialPort stream;
    
    public enum ECommands
    {
        ANGLE, PING
    }
    
	public void OpenSerial(){
		stream = new SerialPort(port,baudrate);
		stream.ReadTimeout = OPERATION_TIMEOUT_MS;
		stream.Open();
	}
    
    public void Close()
    {
        stream.Close();
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

	public void WriteToArduino(string message)
    {
        // Send the request
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }
    
    IEnumerator SendCommandToArduino(ECommands command)
    {
        while(true)
        {
            WriteToArduino(command.ToString());
            ReadCallback(ReadFromArduino((int)OPERATION_TIMEOUT_MS));
            yield return new WaitForSeconds(OPERATION_TIMEOUT_MS/1000f);
        }
    }
    
	public void Start()
	{
	    player = GameObject.FindObjectOfType<PlayerController>();
		OpenSerial();
	    
	    StartCoroutine(SendCommandToArduino(activeCommand));
	}


    private void ReadCallback(string obj)
    {
        if (obj != null)
        {
            if (activeCommand == ECommands.ANGLE)
            {
                //mandamos los datos del arduino separado de una coma
                string[] temp = obj.Split(',');

                for (int i = 0; i < temp.Length; i++)
                {
                    print(temp[i] + ": " + i);
                }

                player.angleX = float.Parse(temp[0]);
                player.angleZ = float.Parse(temp[1]);
            }
            else if (activeCommand == ECommands.PING)
            {
                print(obj);
            }

        }

    }

    private void OnApplicationQuit()
    {
        Close();
    }
}
