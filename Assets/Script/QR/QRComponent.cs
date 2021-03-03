using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing.QrCode;

public class QRComponent : MonoBehaviour
{
    // Start is called before the first frame update
    public Image QRArea;
    private void Awake()
    {

        QRGenerater.startUP();
    }


    public void setQR(personInfo inf)
    {
        setQR(inf.name + ":" + inf.ID);
    }


    //isim:id
    public void setQR(string txt)
    {
        QRArea.sprite = QRGenerater.EncodeSprite(txt);
    }



    void Start()
    {
        //txt = new Texture2D(256, 256);

        
        //txt.SetPixels32(QRGenerater.Encode("HELLLOOOOOOOO"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
