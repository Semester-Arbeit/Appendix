#ifndef DATALOGGER_H_
#define DATALOGGER_H_

#include "SDMMCBlockDevice.h"
#include "FATFileSystem.h"

#include <SDRAM.h>

//The Class to handle the data logging
//---------------------------------------------------------------------------
//Version: V0.1
//Author: Tobias Rothlin
//---------------------------------------------------------------------------
//Methods:
//

class DataLogger
{
  public:

    DataLogger()
      : fs("fs")
    {

    }

    bool init()
    {
      SDRAM.begin();

      rawDataPoints = (double*)SDRAM.malloc(5000 * dataFrameCount * 32);

      int err = fs.mount(&block_device);
      mkdir("fs/LogFiles", 0777);
      if (err == 0)
      {
        isMounted = true;
        return true;
      }
      else
      {
        return false;
      }
    }

    void open(String fileName)
    {
      if (isMounted)
      {
        
        String("fs/LogFiles/" + fileName + ".csv").toCharArray(this->fileName, fileNameLength);
        int i = 1;
        String newFileName = fileName;
        FILE *file;
        while(file = fopen(this->fileName,"r"))
        {
          newFileName =  fileName + "_" + String(i);
          String("fs/LogFiles/" + newFileName + ".csv").toCharArray(this->fileName, fileNameLength);
          i++;
          fclose(file);
        }
        fp = fopen(this->fileName, "w");
      }
    }

    void setHeader(char* header, int lengthOfHeader)
    { 
      trueHeaderLenght = lengthOfHeader;
      for (int i = 0; i < fileHeaderLength; i++)
      {
        if (i < lengthOfHeader-1)
        {
          fileHeader[i] = header [i];
        }
        else if (i == lengthOfHeader-1)
        {
          fileHeader[i] = '\r';
          fileHeader[i + 1] = '\n';
        }
      }

    }

    int println(double* const dataPointsInOneLine)
    {
      for (int i = 0; i < dataFrameCount; i++)
      {
        rawDataPoints[lineInArray * dataFrameCount + i] = dataPointsInOneLine[i];
      }
      lineInArray++;
      return lineInArray - 1;
    }

    void printToFile()
    {
      for (int n = 0; n < trueHeaderLenght+1; n++)
      {
        fprintf(fp, "%c", fileHeader[n]);
      }

      for (int i = 0; i < lineInArray; i++)
      {
        for (int m = 0; m < dataFrameCount; m++ )
        {
          fprintf(fp, "%.10f", rawDataPoints[i * dataFrameCount + m]);
          if (m != dataFrameCount - 1)
          {
            fprintf(fp, ",");
          }
        }
        fprintf(fp, "\r\n");
      }
    }

    void close()
    {
      if (isMounted)
      {
        printToFile();
        fclose(fp);
      }
    }



  private:

    FILE *fp;

    enum DataLoggerConsts {
      dataFrameCount = 22,
      fileNameLength = 100,
      fileHeaderLength = 160,
    };

    int trueHeaderLenght = 0;

    int lineInArray = 0;
    double* rawDataPoints;
    char fileName[fileNameLength];
    char fileHeader[fileHeaderLength] = {' '};
    SDMMCBlockDevice block_device;
    mbed::FATFileSystem fs;
    bool isMounted = false;


};

#endif /*DATALOGGER_H_*/
