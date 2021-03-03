using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveComponent : MonoBehaviour
{

    public InputField[] fields;
    public SaveInterface listener;

    // Start is called before the first frame update
   
    public void savePersonInfo()
    {
        personInfo pi = new personInfo();
        pi.name = fields[0].text;
        pi.surname = fields[1].text;
        pi.mail = fields[2].text;
        pi.phone = fields[3].text;
        pi.proje = fields[4].text;
        listener.onNewPerson(pi);
        closeWindow();
    }

    public void closeWindow()
    {
        Destroy(gameObject);
    }



    public interface SaveInterface
    {
        void onNewPerson(personInfo newPerson);
    }

    public void OnMailCheck()
    {
        Debug.Log("OnEnd");
    }
    public void OnPersonClick()
    {
        Debug.Log("OnClick");
    }

}
