
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

public static class QRGenerater 
{
    private static Texture2D qr;
    private static Color32[] c;
    private static int W=256, H=256;
    private static string LastResult;
    private static Texture2D encoded;

    public static void startUP()
    {
        encoded = new Texture2D(W, H);

    }

    public static string DecodeQR(Color32[] color,int W,int H)
    {
        // create a reader with a custom luminance source
        var barcodeReader = new BarcodeReader { AutoRotate = false, TryHarder = false };


        try
            {
                // decode the current frame
                Result result = barcodeReader.Decode(color, W, H);
                if (result != null)
                {
                    return LastResult = result.Text;
                    //shouldEncodeNow = true;
                    //print(result.Text);
                }

        }
        catch
        {
            return null;
        }
        return null;

    }

    public static Color32[] Encode(string textForEncoding)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = W,
                Height = H,
            }
    };
        return writer.Write(textForEncoding);
        //encoded = new Texture2D(256, 256);
    }
    public static Sprite EncodeSprite(string textForEncoding)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = H,
                Width = W
            }
        };

        encoded.SetPixels32(writer.Write(textForEncoding));
        encoded.Apply();
        return Sprite.Create(encoded, new Rect(0, 0, encoded.width, encoded.height), new Vector2(encoded.width / 2, encoded.height / 2));
        //return writer.Write(textForEncoding);
        //encoded = new Texture2D(256, 256);
    }
}
