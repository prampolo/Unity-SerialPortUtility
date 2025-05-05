using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

/**
 * SerialPortUtility 
 * Author: Mustafa Erdem Köşk <erdemkosk@gmail.com>
 * http://www.erdemkosk.com
 */

public delegate void SerialPortMessageEventHandler(byte[] sendData);
public delegate void SerialPortSendMessageReportHandler(byte[] sendData);
public class SerialCommunication
{
    public event SerialPortMessageEventHandler SerialPortMessageEvent;
    public event SerialPortSendMessageReportHandler SerialPortSendMessageReportEvent;
    private SerialPort serialPort;
    private Thread threadReceive;

    public SerialCommunication(SerialPort serialPort)
    {
        this.serialPort = serialPort;
    }
    public SerialCommunication(string portName, int boudrate)
    {
        serialPort = new SerialPort(portName, boudrate);

    }

    public void OpenSerialPort()
    {
        serialPort.Open();
        serialPort.ReadTimeout = 1;
        threadReceive = new Thread(ListenSerialPort);
        threadReceive.Start();
    }
    public bool IsSerialPortIsOpen()
    { 
        return serialPort.IsOpen;
    }
    public void CloseSerialPort()
    {
        serialPort.Close();
    }

    private void ListenSerialPort()
    {
        while ((serialPort.IsOpen == true))
        {
            try
            {
                int bufferSize = serialPort.ReadBufferSize;
                //										
                byte[] buf = new byte[bufferSize];
                int count = serialPort.Read(buf, 0, bufferSize);
                if (count > 0)
                {
                    Debug.Log("[SERIALE] Ricevuti byte: " + count);
                    string testo = System.Text.Encoding.ASCII.GetString(buf, 0, count);
                    Debug.Log("[SERIALE] Contenuto: " + testo);
                    SerialPortMessageEvent?.Invoke(buf);
                }
            }
            catch (System.Exception)
            {
            }
        }
    }

    public bool SendMessageFromSerialPort(byte[] byteArray)
    {
        if (serialPort != null && serialPort.IsOpen == true)
        {

            serialPort.Write(byteArray, 0, byteArray.Length);

            if (SerialPortSendMessageReportEvent != null && SerialPortSendMessageReportEvent.GetInvocationList().Length > 0) // If somebody is listening
            {
                SerialPortSendMessageReportEvent(byteArray);
            }
            return true;
        }
        else
        {
            return false;
        }


    }
}




