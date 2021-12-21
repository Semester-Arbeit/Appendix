using System;
using System.Collections.Generic;
using System.IO;
namespace GroundStation
{
    public class DataLogger
    {
        private string fileName = "";

        private string fileEnding = "";

        private string header = "Time,Pitch,Roll,Yaw,xGyro,yGyro,ZGyro,xAcc,yAcc,zAcc,latitude,longitude,alt,xSpeed,ySpeed,zSpeed,cP,cR,cY,cP";

        private List<String> linesInFile = new List<String>();

        public enum Type
        {
            csv,
            txt
        }


        public DataLogger(string fileName, Type fileType)
        {
            this.fileName = fileName;
            switch (fileType)
            {
                case Type.csv:
                    this.fileEnding += ".csv";
                    break;
                case Type.txt:
                    this.fileEnding += ".txt";
                    break;
                default:
                    this.fileEnding += ".txt";
                    break;
            }
        }

        public void AppendLine(string line)
        {
            linesInFile.Add(line);
        }

        public void WriteFile()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filepath = Path.Combine(documents, fileName + fileEnding);
            int i = 1;
            while(File.Exists(filepath))
            {
                filepath = Path.Combine(documents, fileName + "_" + i.ToString() + fileEnding);
                i++;
            }
            var myLogFile = File.AppendText(filepath);
            myLogFile.Write(header + "\n");
            foreach (string line in linesInFile)
            {
                myLogFile.Write(line + "\n");
            }
            myLogFile.Close();
        }

    }
}
