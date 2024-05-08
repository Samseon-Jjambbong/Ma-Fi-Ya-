using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class Server : MonoBehaviour
{
    [SerializeField] RectTransform logContent;
    [SerializeField] TMP_Text logTextPrefab;
    [SerializeField] TMP_InputField ipField;
    [SerializeField] TMP_InputField portField;

    private TcpListener listener;
    private List<TcpClient> clients = new List<TcpClient>();
    private List<TcpClient> disconnects = new List<TcpClient>();

    private IPAddress ip;
    private int port;
    
    public bool IsOpened { get; private set; }

    private void Start()
    {
        ShowIPAddress();
    }

    private void ShowIPAddress()
    {
        // Get computer's IP Address on Start
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        ip = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        ipField.text = ip.ToString();
    }

    private void Update()
    {
        if (!IsOpened)
            return;

        foreach (var client in clients)
        {
            if (!CheckClient(client))
            {
                client.Close();
                disconnects.Add(client);
                continue;
            }
            
            NetworkStream stream = client.GetStream();
            if (stream.DataAvailable)
            {
                StreamReader reader = new StreamReader(stream);
                string text = reader.ReadLine();
                //AddLog(text);
                SendAll(text);
            }
        }

        foreach (var client in disconnects)
        {
            clients.Remove(client);
        }
        disconnects.Clear();
    }

    private void OnDestroy()
    {
        if (IsOpened)
        {
            Close();
        }
    }

    public void Open()
    {
        if (IsOpened)
            return;
        
        AddLog("Try to Open");

        port = int.Parse(portField.text);

        try
        {
            listener = new TcpListener(ip, port);
            listener.Start();

            IsOpened = true;
            listener.BeginAcceptTcpClient(AcceptCallback, listener);
        }
        catch (Exception ex)
        {
            AddLog(ex.Message);
        }
    }

    public void Close()
    {
        if (!IsOpened)
            return;
        
        listener?.Stop();
        listener = null;
        IsOpened = false;
        AddLog("Close");
    }

    public void SendAll(string chat)
    {
        foreach (var client in clients)
        {
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream);

            try
            {
                writer.WriteLine(chat);
                writer.Flush();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }    
        }
    }

    private void AcceptCallback(IAsyncResult ar)
    {
        if (listener == null)
        {
            Debug.Log("Server Closed");
            return;
        }

        // On Callback, add incoming client to clients list
        TcpClient client = listener.EndAcceptTcpClient(ar);
        clients.Add(client);
        
        // Check for more incoming clients
        listener.BeginAcceptTcpClient(AcceptCallback, listener);
    }

    private void AddLog(string message)
    {
        Debug.Log($"[Server] {message}");
        TMP_Text newLog = Instantiate(logTextPrefab, logContent);
        newLog.text = message;
    }

    private bool CheckClient(TcpClient client)
    {
        try
        {
            if (client != null && client.Client != null && client.Connected)
            {
                if (client.Client.Poll(0, SelectMode.SelectRead))
                    return client.Client.Receive(new byte[1], SocketFlags.Peek) != 0;

                return true;
            }
            else
                return false;
        }
        catch (Exception e)
        {
            AddLog("Connect Check Error");
            AddLog(e.Message);
            return false;
        }
    }
}
