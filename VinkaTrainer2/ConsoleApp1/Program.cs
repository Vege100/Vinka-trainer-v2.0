using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using (WebClient client = new WebClient())
{
    string aika = client.DownloadString("https://www.timeanddate.com/sun/finland/jyvaskyla");
    int a = aika.IndexOf("sunalt");
    aika = aika.Substring(a+7, 4);
    string[] ajat = aika.Split(',');
    Console.WriteLine(ajat[0]);
}