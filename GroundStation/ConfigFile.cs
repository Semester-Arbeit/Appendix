using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


//sheet[X] = element X von { parameter , wert }








namespace GroundStation
{
    public class ConfigFile
    {
        private string fileName;
        private Dictionary<string,string> myDictionary = new Dictionary<string, string>();


        public ConfigFile(string name)
        {
            this.fileName = name + ".csv";
        }

        public void AddNewParameter(string Name, string Value)
        {
            myDictionary.Add(Name, Value);  
        }

        public void SetParameter(string Name, string Value)
        {
            if (myDictionary.ContainsKey(Name))
                myDictionary[Name] = Value;
            else
                Console.WriteLine("ERROR: tried to write to a non existant parameter");
        }


        public string GetParameter(string Name)
        {

            if (myDictionary.ContainsKey(Name))
                return myDictionary[Name];
            else
            {
                Console.WriteLine("ERROR: tried to read a non existant parameter");
                return "ERROR: tried to read a non existant parameter";
            }
        }



        public void WriteFile()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filepath = Path.Combine(documents, fileName);
            var myLogFile = File.AppendText(filepath);
            foreach (KeyValuePair<string, string> entry in myDictionary)
            {

                myLogFile.Write(entry.Key+entry.Value+"\n");
            }
            myLogFile.Close();
        }

        public void ReadConfigFile(string configName)
        {
            configName += ".csv";
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filepath = Path.Combine(documents, configName);
            var myConfigFile = File.ReadAllLines(filepath);


            for(int line = 0; line < myConfigFile.Length;line++)
            {
                var tempList = new List<string>();
                tempList = myConfigFile[line].Split(',').ToList();
                myDictionary.Add(tempList[0], tempList[1]);
            }
            
        }
    }
}
