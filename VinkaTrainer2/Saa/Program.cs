using System;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

using (WebClient client = new WebClient())
{
    string saa = client.DownloadString("https://www.getmetar.com/EFJY");
    int a = saa.IndexOf("> EFJY ");
    int b = saa.IndexOf(" Q");
    saa = saa.Substring(a, b-a);
    Console.WriteLine(saa);
}


