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
            int counter = 0;

            foreach(string file in Directory.GetFiles(cwd, "*.xml")){

                if(File.Exists(file)) {

                    try {

                        string store = getStoreID(file);
                        string fname = file.Substring(cwd.Length + 1);
                        string seqNr = returnSeq(file);

                        if (!Directory.Exists(Path.Combine(cwd, store))){

                            Directory.CreateDirectory(Path.Combine(cwd, store));
                        }    
                        try {
                            File.Copy(file, Path.Combine(cwd, store,$"{fname} - {seqNr}"));
                            counter++;
                                
                            File.Delete(file);
                        }
                            catch(Exception e){
                            Console.WriteLine(e.Message);
                        }
                    }
                    catch(Exception e)  {

                        Console.WriteLine(e.Message);
                    }
                } else {
                    continue;
                }             
            }

            Console.WriteLine($"Succesfully handled {counter} files.");
            Console.ReadLine();
        }

        public static string getStoreID(string file){

            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlNodeList storeId = doc.GetElementsByTagName("RetailStoreID");

            return storeId[0].InnerXml;
        }

        public static string returnSeq(string file){

            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlNodeList seq = doc.GetElementsByTagName("SequenceNumber");

            return seq[0].InnerXml;

        }
    }
}