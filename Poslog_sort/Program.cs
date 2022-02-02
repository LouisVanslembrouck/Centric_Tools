using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO.Ports;
using System.Data;

namespace PoslogSort
{
    class Program
    {
        static void Main(string[] args)
        {
            string cwd = Directory.GetCurrentDirectory();
            string outFile = Path.Combine(cwd, "output.csv");
            int counter = 0;
            int failed = 0;

            if(!File.Exists(outFile)){
                File.Create(outFile);
            }

            using(var w = new StreamWriter(outFile)){

                foreach(string file in Directory.GetFiles(cwd, "*.xml")){

                    try{
                        Row Items = returnAll(file);
                        var newLine = $"{Items.storeId},{Items.posNumber},{Items.date},{Items.seq},{Items.amount},{Items.count}";
                        w.WriteLine(newLine);
                        Console.WriteLine($"Handled {file}");
                        counter++;
                    }
                    catch(Exception e){
                        Console.WriteLine($"{e.Message}");
                        failed++;
                        continue;
                    }
                }    
            }

            Console.WriteLine($"Succesfully handled {counter} files. Failed {failed}.");
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }

        public static Row returnAll(string file){ 

            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlNodeList store = doc.GetElementsByTagName("RetailStoreID");
            XmlNodeList seqNr = doc.GetElementsByTagName("SequenceNumber");
            XmlNodeList totalAmount = doc.GetElementsByTagName("Total");
            XmlNodeList posNr = doc.GetElementsByTagName("WorkstationID");
            XmlNodeList Date = doc.GetElementsByTagName("EndDateTime");
            XmlNodeList counter = doc.GetElementsByTagName("Tender");

            Row Item = new Row
                {
                    storeId = store[0].InnerXml,
                    seq = seqNr[0].InnerXml,
                    amount = totalAmount[0].InnerXml,
                    posNumber = posNr[0].InnerXml,
                    date = Date[0].InnerXml,
                    count = counter.Count.ToString()
                };

            return Item;
        }
    }
}
