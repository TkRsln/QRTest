using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MainQR : MonoBehaviour,TcpInterface,QRReader.QRReadInterface,SaveComponent.SaveInterface
{

    public GameObject prefab_kayit;
    public GameObject prefab_reader;
    public GameObject prefab_info;
    public GameObject prefab_sql;
    public GameObject prefab_load;
    public GameObject prefab_qrcorrect;
    public GameObject prefab_qrwrong;

    public Button button_previous = null;
    public Button button_next = null;
    public Text txt_id = null;

    public int current_person_id { 
        get { return curent_id; } 
        set {
            txt_id.text = value+"";
            curent_id = value;
        } 
    }
    private int curent_id = 1;

    public Text[] fields;
    public Text txt_err;

    public TcpManager tm;
    public QRComponent qr_component;
    public PrintComponent print_component;


    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 20;
        checkSqlLink();
        TcpManager.listeners.Add(this);
        UnityThread.initUnityThread();
        tm = GetComponent<TcpManager>();
        qr_component = GetComponent<QRComponent>();
        print_component = GetComponent<PrintComponent>();
        button_previous.interactable = false;
    }

    public void checkSqlLink()
    {
        //string link = PlayerPrefs.GetString("sql_link", "");
        //if (string.IsNullOrEmpty(link))
        //{
        //     openLinkWindow("Link henüz girilmemiş...");
        //    return;
        //}
        //Exception e = 
       
    }
    public void openLinkWindow(string err_source)
    {
        GameObject go = Instantiate(prefab_sql, transform.parent);
        go.GetComponent<SQLComponent>().setErrText(err_source);
    }
   

    // INTERFACE
    public void OnReaderButton()
    {
       GameObject go= Instantiate(prefab_reader, transform.parent);
        go.GetComponent<QRReader>().listener = this;
    }
    // INTERFACE
    public void OnKayitButton()
    {
        GameObject go = Instantiate(prefab_kayit, transform.parent);
        go.GetComponent<SaveComponent>().listener = this;
    }
    // INTERFACE
    public void OnNextButton()
    {
        button_next.interactable = false;
        button_previous.interactable = false;

        StartCoroutine(showLoadScreen());

        current_person_id++;
        tm.requestPersonData(current_person_id);

    }
    // INTERFACE
    public void OnPreviousButton()
    {

        button_previous.interactable = false;
        current_person_id--;
        if (current_person_id <= 1)
        {
            current_person_id = 1;
            button_next.interactable = false;
        }
        tm.requestPersonData(current_person_id);
        StartCoroutine(showLoadScreen());

    }






    public void setInfosUI(string[] infos)
    {
        for(int i = 0; i < 5; i++)
        {
            fields[i].text = infos[i];
        }
    }





    public IEnumerator showLoadScreen()
    {
        yield return null;
        load_scene = Instantiate(prefab_load, transform.parent);
    }
    IEnumerator closeLoadScene()
    {
        yield return null;
        button_next.interactable = true;
        if(curent_id!=1)button_previous.interactable = true;
        Destroy(load_scene);
    }
    IEnumerator setPersonData(personInfo inf)
    {
        yield return null;
        setInfosUI(inf.getData());
        qr_component.setQR(inf.name + ":" + inf.ID);

        current_person_id = inf.ID;
    }
    IEnumerator requestPersonData(int id)
    {
        yield return new WaitForSeconds(0.1f);
        tm.requestPersonData(id);
    }
    public void saveNewPerson(personInfo info)
    {
        StartCoroutine(showLoadScreen());
        tm.insertPersonData(info);
    }

    private GameObject load_scene;
    // INTERFACE
    public void onTryConnect()
    {
        showLoadScreen();
    }

    // INTERFACE
    public void onConnected()
    {
        //UnityThread.executeCoroutine(destroyObject());
        Debug.Log("person ID:" + current_person_id);
        tm.requestPersonData(current_person_id);
        //showLoadScreen();
    }
    // INTERFACE
    public void OnPrintButton()
    {
        personInfo pi = new personInfo();
        pi.ID = current_person_id;
        pi.name = fields[0].text;
        pi.surname = fields[1].text;
        pi.mail = fields[2].text;
        pi.phone = fields[3].text;
        pi.proje = fields[4].text;

        print_component.printDocument(pi);
    }

    // INTERFACE
    public void onReqPerson(personInfo inf)
    {
        if (inf != null)
        {
            Debug.Log(inf.name + " " + inf.ID);
        }
        if (inf == null)
        {
            if (current_person_id > 1)
            {
                current_person_id=1;
                button_previous.interactable = false;
                name_requested = null;
                UnityThread.executeCoroutine(requestPersonData(current_person_id));
                Debug.Log("NULL --->>" + current_person_id); 
                //UnityThread.executeCoroutine(closeLoadScene());
                return;
            }
        }
        else if (name_requested == null)
            UnityThread.executeCoroutine(setPersonData(inf));
        else if (inf.name == name_requested)
        {
            UnityThread.executeCoroutine(setPersonData(inf));
            UnityThread.executeCoroutine(foundCorrectQR(inf));
            name_requested = null;
        }
        UnityThread.executeCoroutine(closeLoadScene());
       

    }

    // INTERFACE
    public void onPersonSaved(personInfo inf)
    {
        UnityThread.executeCoroutine(closeLoadScene());
    }


    string name_requested=null;
    // INTERFACE
    public void foundQR(string txt)
    {
        try { 
            if (string.IsNullOrEmpty(txt)) return;
            name_requested = txt.Substring(0,txt.IndexOf(':'));
            int id = 0;

            if(int.TryParse(txt.Substring(txt.IndexOf(':')+1),out id))
            {
                StartCoroutine(showLoadScreen());
                tm.requestPersonData(id);

            }
            else
            {
                foundWrongQR("QRda yazılan txt, database ile eşleşmedi '"+txt+"'");
            }
        }catch(Exception e)
        {
            foundWrongQR(e.Source+" | "+e.Message);
        }
    }
    public void foundWrongQR(string txt)
    {
        Destroy(Instantiate(prefab_qrwrong, transform.parent), 1.25f);
        if (txt != null)
        {
            txt_err.text = "ERR: " + txt;
        }
    }
    public IEnumerator foundCorrectQR(personInfo inf)
    {
        yield return null;
        Destroy(Instantiate(prefab_qrcorrect, transform.parent), 1.25f);
        print_component.printDocument(inf);
    }

    public void onNewPerson(personInfo np)
    {
        if (string.IsNullOrEmpty(np.proje)) np.proje = "Ziyaretçi";
        if (string.IsNullOrEmpty(np.phone)) np.phone = "Belirtilmedi";
        if (string.IsNullOrEmpty(np.name)) saveError();
        else if (string.IsNullOrEmpty(np.surname)) saveError();
        else if (string.IsNullOrEmpty(np.mail)) saveError();
        else
        {
            
            StartCoroutine(showLoadScreen());
            StartCoroutine( tm.coInsertData(np) );
        }

    }
    private void saveError()
    {
        txt_err.text = "Yeni katılımcı kayıt edilirken bilgiler boş bırakılamaz...";
    }
    public void setErrMsg(string msg)
    {
        if (msg != null)
        {
            txt_err.text = "ERR: " + msg;
        }
    }
}

