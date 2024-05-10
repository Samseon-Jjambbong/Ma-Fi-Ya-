using System;
using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField] Chat chat;

    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField ipField;
    [SerializeField] TMP_InputField portField;

    private TcpClient client;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    private string clientName;
    private string ip;
    private int port;

    public bool IsConnected { get; private set; }

    private void Update()
    {
        if (IsConnected == false || stream.DataAvailable == false)
            return;

        string text = reader.ReadLine();
        ReceiveChat(text);
    }

    public void Connect()
    {
        if (IsConnected)
            return;

        clientName = nameField.text;
        ip = ipField.text;
        port = int.Parse(portField.text);

        try
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            
            Debug.Log("Connect success");
            IsConnected = true;
        }
        catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void DisConnect()
    {
        if (!IsConnected)
            return;
        
        writer?.Close();
        writer = null;
        reader?.Close();
        reader = null;
        stream?.Close();
        stream = null;
        client?.Close();
        client = null;
        IsConnected = false;
    }

    public void SendChat(string chatText)
    {
        if (!IsConnected)
            return;

        try
        {
            writer.WriteLine($"{clientName} : {chatText}");
            writer.Flush();
        }
        catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void ReceiveChat(string chatText)
    {
        chat.AddMessage(chatText);
    }
}
