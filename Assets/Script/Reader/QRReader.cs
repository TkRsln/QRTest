using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QRReader : MonoBehaviour
{

    public const int CODE_VERIFIED= 1;
    public const int CODE_NOTVERIFIED= 2;

    public RawImage image;
    public WebCamTexture wt;
    public GameObject prefab_verified;
    public GameObject prefab_notverified;

    public QRReadInterface listener;

    void Start()
    {
        wt = new WebCamTexture(WebCamTexture.devices[0].name);
        image.texture = wt;
        wt.Play();

        StartCoroutine(getPicture());
    }

    public void closeWindow(int exitCode)
    {
        if (wt != null&&wt.isPlaying)
        {
            wt.Stop();
        }
        if (exitCode == CODE_VERIFIED)
        {
            Destroy(Instantiate(prefab_verified, transform.parent),2.5f);
        }
        else if (exitCode == CODE_NOTVERIFIED)
        {
            Destroy( Instantiate(prefab_notverified, transform.parent),2.5f);
        }
        Destroy(gameObject);
    }

    IEnumerator getPicture()
    {

        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            string data = QRGenerater.DecodeQR(wt.GetPixels32(), wt.width, wt.height);

            if (data != null) {
                listener.foundQR(data);
                closeWindow(0);
            }

        }

    }
    public interface QRReadInterface
    {
        void foundQR(string txt);
       
    }
}
