using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TcpManager : MonoBehaviour
{


    public TcpClient client;
    public static int CODE_GET = 0;
    public static int CODE_INSERT = 1;
    public static int CODE_ERR = 2;
    public static int CODE_GRANTED = 3;
    public StreamWriter sw;
    public StreamReader sr;
    public static List<TcpInterface> listeners = new List<TcpInterface>();

    private readonly object locker = new object();



    void Start()
    {
        sendTryConnect();
        setUpNetwork();
    }
    public  void setUpNetwork()
    {
        lock (locker)
        {
            if (client == null || !client.Connected)
            {
                new Thread(() =>
                {
                    Debug.Log("set Up start");
                    client = new TcpClient("185.86.4.79", 4141);

                    Debug.Log(" connected to 185.86.4.79");
                    sw = new StreamWriter(client.GetStream());
                    sr = new StreamReader(client.GetStream());
                    sendConnected();

                }).Start();
            }
        }

    }
    public void requestPersonData(int id)
    {
        lock (locker) { 
            if (client == null || !client.Connected) return;
            sw.WriteLine(CODE_GET + "");
            sw.WriteLine(id + "");
            //Debug.Log("req id: "+id);
            sw.Flush();

            string name = sr.ReadLine();
            if (string.IsNullOrEmpty(name)||name == "" + CODE_ERR) 
                sendPersonInf(null);
            else
            {
                personInfo pi = new personInfo();
                pi.ID = id;
                pi.name = name;
                //Debug.Log(name);
                pi.surname = sr.ReadLine();
                pi.mail = sr.ReadLine();
                pi.phone = sr.ReadLine();
                pi.proje = sr.ReadLine();
                //Debug.Log(name+" "+ pi.surname+" " + pi.mail+" "+ pi.phone+" " + pi.proje);
                sendPersonInf(pi);
            }
        }
    }
    public IEnumerator coInsertData(personInfo inf)
    {
        yield return null;
        insertPersonData(inf);
    }
    public void insertPersonData(personInfo inf)
    {

        lock (locker)
        {
            if (inf == null) return;
            if (client == null || !client.Connected) return;
            sw.WriteLine(CODE_INSERT + "");
            sw.WriteLine(inf.name + "");
            sw.WriteLine(inf.surname + "");
            sw.WriteLine(inf.mail + "");
            sw.WriteLine(inf.phone + "");
            sw.WriteLine(inf.proje + "");
            sw.Flush();
            string read = sr.ReadLine();
            Debug.Log(read+" "+(read==CODE_GRANTED+"") );
            sendPersonSaved(inf);
        }
    }


    private void sendTryConnect()
    {
        foreach (TcpInterface inter in listeners)
            inter.onTryConnect();
    }
    private void sendConnected()
    {
        foreach (TcpInterface inter in listeners)
            inter.onConnected();
    }
    private void sendPersonInf(personInfo inf)
    {
        foreach (TcpInterface inter in listeners)
            inter.onReqPerson(inf);
    }
    private void sendPersonSaved(personInfo inf)
    {
        foreach (TcpInterface inter in listeners)
            inter.onPersonSaved(inf);
    }

}
public class personInfo
{

    public int ID = 0;
    public string name = "";
    public string surname = "";
    public string mail = "";
    public string phone = "";
    public string proje = "";

    public string[] getData()
    {
        return new string[] { name, surname, mail, phone, proje };
    }
    
}
public interface TcpInterface
{
    void onTryConnect();
    void onConnected();
    void onReqPerson(personInfo inf);
    void onPersonSaved(personInfo inf);

}