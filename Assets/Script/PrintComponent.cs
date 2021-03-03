using Aspose.Words;
using Aspose.Words.Replacing;
using SmartDLL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Xceed.Words.NET;

public class PrintComponent : MonoBehaviour
{

    public GameObject togle_dock;
    public GameObject togle_print;
    public GameObject button_print;
    public bool isOsWindows=true;

    public bool auto_print = true;
    public bool auto_dock = true;

    private Process word;
    private SmartPrinter printer;

    // Start is called before the first frame update
    void Start()
    {
        isOsWindows = Application.platform == RuntimePlatform.WindowsPlayer|| Application.platform == RuntimePlatform.WindowsEditor;
        if (!isOsWindows)
        {
            togle_dock.SetActive(false);
            togle_print.SetActive(false);
            button_print.SetActive(false);
            printer = new SmartPrinter();
        }
    }



    public void printDocument(personInfo inf)
    {
        if (!isOsWindows) return;
        if (auto_print)
        {
            printer.PrintDocument(getPrintText(inf), @"C:\Users\utkua\OneDrive\Masaüstü\Dosyalar\Unity\QRUnity\header.png");
        }
        else if (auto_dock)
        {
            try {
                if (!File.Exists(@".\template.docx"))
                {
                    GetComponent<MainQR>().setErrMsg("Yazdırmak için geren 'template.docx' dosyası bulunamadı. Lürfen şablon hazırlayın veya şablonu kontrol edin.");
                    return;
                }
                if (word != null && !word.HasExited)
                    word.Kill();
                string fileName = @".\temp.docx";
                replaceDocx(inf);
                word = Process.Start("WINWORD.EXE", fileName);

                /*

                DocX doc = DocX.Create(fileName);

                doc.InsertParagraph("İsim:  "+ getUpperString(inf.name));
                doc.InsertParagraph("Soyisim:  "+ getUpperString(inf.surname));
                doc.InsertParagraph("Etkinlik:  "+(inf.proje.ToLower()=="ziyaretçi"? getUpperString(inf.proje):(inf.proje.ToLower() == "görevli"? getUpperString(inf.proje):"Proje Sahibi")));
                doc.Save();
                word=Process.Start("WINWORD.EXE", fileName);
                */
            }
            catch(Exception e)
            {
                GetComponent<MainQR>().setErrMsg(e.Message + "|" + e.StackTrace);
            }
        }
    }
    public string getUpperString(string txt)
    {
        return Regex.Replace(txt, @"(^\w)|(\s\w)", m => m.Value.ToUpper());
    }
    public void replaceDocx(personInfo inf)
    {
        DocX readFrom = DocX.Load(@".\template.docx");

        foreach(var para in readFrom.Paragraphs)
        {
            if (para.Text.Contains("%isim%"))
                para.ReplaceText("%isim%", getUpperString(inf.name),false);
            if (para.Text.Contains("%soyisim%"))
                para.ReplaceText("%soyisim%", getUpperString(inf.surname),false);
            if (para.Text.Contains("%mail%"))
                para.ReplaceText("%mail%", getUpperString(inf.mail),false);
            if (para.Text.Contains("%telefon%"))
                para.ReplaceText("%telefon%", getUpperString(inf.phone),false);
            if (para.Text.Contains("%proje%"))
                para.ReplaceText("%proje%", getUpperString(inf.proje),false);
            if (para.Text.Contains("%id%"))
                para.ReplaceText("%id%", getUpperString(inf.ID+""),false);
        }
        readFrom.SaveAs(new FileStream(@".\temp.docx",FileMode.OpenOrCreate));

    }
    public string getPrintText(personInfo inf)
    {
        return "İsim:   " + inf.name + "\nSoyisim:  " + inf.surname + "";

    }

   public void OnButtonPrint(bool active)
    {
        auto_print = active;
    }
    public void OnButtonDoc(bool active)
    {
        auto_dock = active;
    }
}
