using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SQLComponent : MonoBehaviour
{

    public Text err_txt;
    public InputField in_field;

   public void OnConnectButton()
   {
        if(string.IsNullOrEmpty(in_field.text))
        {
            setErrText("Link alanı boş bırakılamaz...");
            return;
        }
        else
        {
            StartCoroutine( tryConnect() );
            setErrText("<color=cyan>Bağlantı deneniyor...</color>");
        }

   }
    IEnumerator tryConnect()
    {
        Debug.Log("Connecting");
        yield return null;
        string link = in_field.text;
        Debug.Log(">>>"+ link);
        /*
        if (e == null)
        {
            Debug.Log("Connected...");
            PlayerPrefs.SetString("sql_link", in_field.text); 
            setErrText("<color=green> Bağlantı kuruldu...</color>");
            Destroy(gameObject);
            
        }
        else
        {
            Debug.Log("ERR...");
            setErrText(e.Source + "|" + e.Message);
        }
        */
    }
    public void setErrText(string err)
    {
        err_txt.text = err;
    }
}
