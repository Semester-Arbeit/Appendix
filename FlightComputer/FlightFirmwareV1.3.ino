#include <Servo.h>
#include <mbed.h>
#include <Arduino_PortentaBreakout.h>
#include <WiFi.h>
#include <WiFiUdp.h>
#include "FlightControls.h"
#include "Sensors.h"
#include "Config.h"
#include "ControlSystem.h"
#include "DataLogger.h"

Config configurationData = Config();
FlightControls flightSystem = FlightControls(&configurationData);
Sensors flightSensors = Sensors();


double targetAtt[3] = {0, 0, 0};
double targetGyro[3] = {0, 0, 0};
double targetPos[3] = {0, 0, 0};
double targetSpeed[3] = {0, 0, 0};

double outputPitch, outputRoll, outputYaw, outputPower;

enum measurementData
{
  numberOfMeasurements = 100
};

bool isCalibrated = false;

String startUpStatus = "";

ControlSystem flightControlSystem;

DataLogger flightDataLogger;
DataLogger flightErrorLog;

SDMMCBlockDevice block_device;
mbed::FATFileSystem fs("fs");
FILE *fp;

//UDP Connection
char ssid[] = "Alpha";    // SSID
char pass[] = "00000000";    // Pasword
int keyIndex = 0;             // your network key Index number (needed only for WEP)

int status = WL_IDLE_STATUS;

unsigned int localPort = 2390;      // local port to listen on

char packetBuffer[350]; //buffer to hold incoming packet
char  ReplyBuffer[200];       // a string to send back
char  Terminator[] = "E";       // a string to send back

WiFiUDP Udp;
unsigned long lastExecutionTime = 0;

bool isSerialAvalable = false;

void setup() {
  Serial.begin(9600);
  delay(5000);
  isSerialAvalable = Serial;
  pinMode(LEDR, OUTPUT);
  pinMode(LEDG, OUTPUT);
  pinMode(LEDB, OUTPUT);
  digitalWrite(LEDR, HIGH);
  digitalWrite(LEDG, HIGH);
  digitalWrite(LEDB, HIGH);

  blinkTaskLED(10);
  //LoadDataFromSD
  if (fs.mount(&block_device) == 0)
  {
    fp = fopen("fs/ConfigFiles/KValues.csv", "r");
    char c = fgetc(fp);
    double newKValues[12];
    int indexInNewKValues = 0;
    String currentNumber = "";
    while (c != '\n')
    {
      if (c == ',')
      {
        newKValues[indexInNewKValues] = currentNumber.toDouble();
        currentNumber = "";
        indexInNewKValues++;
      }
      else
      {
        currentNumber += c;
      }
      c = fgetc(fp);
    }
    fclose(fp);
    newKValues[indexInNewKValues] = currentNumber.toDouble();
    configurationData.setControlSystemValues(newKValues);
    if (isSerialAvalable)
    {
      Serial.println("PID Configuration Updated");

    }


    fp = fopen("fs/ConfigFiles/AltitudeProfile.csv", "r");
    c = fgetc(fp);
    currentNumber = "";
    while (c != '\n')
    {
      if (c == ',')
      {
        configurationData.setNextKeyFrame(currentNumber.toDouble());
        currentNumber = "";
      }
      else
      {
        currentNumber += c;
      }
      c = fgetc(fp);
    }
    fclose(fp);
    configurationData.setNextKeyFrame(currentNumber.toDouble());
    startUpStatus += "1,";
    if (isSerialAvalable)
    {
      Serial.println("---Parsed Data---");
      double* altitudeData = configurationData.getAllKeyFrames();
      for (int i = 0; i < configurationData.getNumberOfKeyFrames(); i++)
      {
        Serial.print(i);
        Serial.print(":   ");
        Serial.println(altitudeData[i]);
      }
    }
  }
  else
  {
    startUpStatus += "0,";
    if (isSerialAvalable)
    {
      Serial.println("Warning:  Not the correct UPD Commend Received!!");
      Serial.println("          Using default Values vor K");
    }

  }

  //Initalise WIFI
  blinkTaskLED(1);
  if (isSerialAvalable)
  {
    Serial.println("Init WiFi");
  }
  status = WiFi.beginAP(ssid, pass);
  Udp.begin(localPort);
  startUpStatus += "1,";
  if (isSerialAvalable)
  {
    Serial.println("WiFi Status: Okey");
  }
  delay(500);

  //Initalise Sensors
  blinkTaskLED(2);
  if (isSerialAvalable)
  {
    Serial.println("Init Sensors");
  }
  if (flightSensors.init())
  {
    startUpStatus += "1,";
    if (isSerialAvalable)
    {
      Serial.println("Sensors Status: Okey");
    }
    digitalWrite(LEDG, LOW);
  }
  else
  {
    startUpStatus += "0,";
    if (isSerialAvalable)
    {
      Serial.println("Sensors Status: Failed");
    }
    digitalWrite(LEDR, LOW);
  }




  //Initalise Flight Controls
  blinkTaskLED(3);
  if (isSerialAvalable)
  {
    Serial.println("Init Flight Controls");
  }
  flightSystem.testAilerons();
  digitalWrite(LEDG, LOW);
  startUpStatus += "1,";
  if (isSerialAvalable)
  {
    Serial.println("Flight Controls Status: Okey");
  }
  delay(500);

  //Initalise Flight Data Logger
  blinkTaskLED(4);
  if (isSerialAvalable)
  {
    Serial.println("Init Flight Data Logger");
  }
  if (flightDataLogger.init())
  {
    startUpStatus += "1,";
    if (isSerialAvalable)
    {
      Serial.println("Flight Data Logger Status: Okey");
    }
    digitalWrite(LEDG, LOW);
  }
  else
  {
    startUpStatus += "0,";
    if (isSerialAvalable)
    {
      Serial.println("Flight Data Logger Status: False");
    }
    digitalWrite(LEDR, LOW);
  }
  flightDataLogger.open("FlightLog");
  char csvHeader[] = "Time,Pitch,Roll,Yaw,xGyro,yGyro,ZGyro,xAcc,yAcc,zAcc,latitude,longitude,alt,xSpeed,ySpeed,zSpeed,cP,cR,cY,cPower,PosXError,PosYError";
  flightDataLogger.setHeader(csvHeader, sizeof(csvHeader) / sizeof(csvHeader[0]));
  delay(500);



  //Initalise Control System
  blinkTaskLED(5);
  if (isSerialAvalable)
  {
    Serial.println("Init Control System");
  }
  lastExecutionTime = micros();
  flightControlSystem.init(flightSensors.getAttitude(), flightSensors.getGyro(), flightSensors.getAlt(), flightSensors.getSpeed(&lastExecutionTime), targetAtt, targetGyro, targetPos, targetSpeed, &outputPitch, &outputRoll, &outputYaw, &outputPower, configurationData.getKValuesForController()[0], configurationData.getKValuesForController()[1], configurationData.getKValuesForController()[2], configurationData.getKValuesForController()[3]);
  if (isSerialAvalable)
  {
    Serial.println(flightControlSystem.getStatus());
  }
  startUpStatus += "1,";
  digitalWrite(LEDR, HIGH);
  digitalWrite(LEDG, LOW);
  digitalWrite(LEDB, HIGH);
  if (isSerialAvalable)
  {
    Serial.println("Init Done");
  }
}

void loop() {
  if (getNewUDPPackets())
  {
    //Flight Mode
    String telemetryString = "";
    int gpsUpdate = 100;
    if (packetBuffer[0] == 'L')
    {
      if (isCalibrated)
      {
        digitalWrite(LEDR, LOW);
        digitalWrite(LEDG, HIGH);
        digitalWrite(LEDB, HIGH);
        flightSystem.startMotor();
        flightSensors.resetSpeedEstimation();
        delay(500);
        lastExecutionTime = micros();
        for (int i = 0; i < 5000; i++)
        {
          while (micros() - lastExecutionTime < 8000)
          {
          }
          lastExecutionTime = micros();
          if (getNewUDPPackets())
          {
            if (packetBuffer[0] == 'A')
            {
              flightSystem.stopMotor();
              flightDataLogger.close();
              digitalWrite(LEDR, HIGH);
              digitalWrite(LEDG, LOW);
              digitalWrite(LEDB, HIGH);
              break;
            }
          }
          double * attitude = flightSensors.getAttitude();
          double * gyro = flightSensors.getGyro();
          double* acc = flightSensors.getAcc();
          if (gpsUpdate >= 100)
          {
            flightSensors.updateLocation();
            gpsUpdate = 0;
          }
          double* pos = flightSensors.getAlt();
          double* rawGpsData = flightSensors.getRawGPSPos();
          double* speed = flightSensors.getSpeed(&lastExecutionTime);
          gpsUpdate++;
          targetPos[2] = configurationData.getAllKeyFrames()[i];
          flightControlSystem.updateValues(&lastExecutionTime);

          double tempLastExecutionTimeAsDouble = (double)lastExecutionTime;
          double rawDataPointers[22] = {tempLastExecutionTimeAsDouble, attitude[0], attitude[1], attitude[2], gyro[0], gyro[1], gyro[2], acc[0], acc[1], acc[2], rawGpsData[0], rawGpsData[1], pos[2], speed[0], speed[1], speed[2], outputPitch, outputRoll, outputYaw, outputPower, pos[0], pos[1]};
          flightDataLogger.println(rawDataPointers);
          sendUdpTelemetry(rawDataPointers);


          flightSystem.setAilerons(outputPitch, outputRoll, outputYaw);
          flightSystem.setThrotle(75 + outputPower);

          if (isSerialAvalable)
          {
            Serial.println(pos[0], 8);
          }
        }
        telemetryString = "E";
        telemetryString.toCharArray(ReplyBuffer, 200);
        sendUdpData(ReplyBuffer);
        flightSystem.stopMotor();
        flightDataLogger.close();
        digitalWrite(LEDR, HIGH);
        digitalWrite(LEDG, LOW);
        digitalWrite(LEDB, HIGH);
      }
      else
      {
        telemetryString = "E";
        telemetryString.toCharArray(ReplyBuffer, 200);
        sendUdpData(ReplyBuffer);
      }
    }

    if (packetBuffer[0] == 'P')
    {
      while (!getNewUDPPackets())
      {
        delay(1);
      }

      int i = 0;
      int m = 0;
      String currentNumber = "";
      double newKValues[12];
      while (packetBuffer[i] != '\n')
      {
        if (packetBuffer[i] == ',')
        {
          newKValues[m] = currentNumber.toDouble();
          currentNumber = "";
          m++;
        }
        else
        {
          currentNumber += packetBuffer[i];
        }
        i++;
      }
      newKValues[m] = currentNumber.toDouble();
      configurationData.setControlSystemValues(newKValues);
    }

    if (packetBuffer[0] == 'S')
    {
      delay(1500);
      startUpStatus.toCharArray(ReplyBuffer, 200);
      sendUdpData(ReplyBuffer);
      delay(1000);
      telemetryString = "E";
      telemetryString.toCharArray(ReplyBuffer, 200);
      sendUdpData(ReplyBuffer);
    }

    if (packetBuffer[0] == 'C')
    {
      double alphaX = 0.01;
      double alphaY = 0.01;
      double alphaZ = 0.1;
      double averageXSpeed = 0;
      double averageYSpeed = 0;
      double averageZSpeed = 0;

      double averagePitch = 0;
      double averageRoll = 0;

      for (int i = 0; i < numberOfMeasurements; i++)
      {
        digitalWrite(LEDR, HIGH);
        digitalWrite(LEDG, HIGH);
        digitalWrite(LEDB, LOW);
        double* acc = flightSensors.getAcc();

        delay(50);
        averageXSpeed += acc[0];
        averageYSpeed += acc[1];
        averageZSpeed += acc[2];

        digitalWrite(LEDB, HIGH);
        delay(50);
      }

      flightSensors.initZSpeedEstimation(alphaZ, averageZSpeed / numberOfMeasurements);
      flightSensors.initXSpeedEstimation(alphaX, averageXSpeed / numberOfMeasurements);
      flightSensors.initYSpeedEstimation(alphaY, averageYSpeed / numberOfMeasurements);

      targetAtt[0] = (averagePitch / numberOfMeasurements);
      targetAtt[1] = averageRoll / numberOfMeasurements;

      double startLat = 0;
      double startLon = 0;
      
      for (int i = 0; i < 10; i++)
      {
        flightSensors.updateLocation();
        double* rawGpsData = flightSensors.getRawGPSPos();
        startLat += rawGpsData[0]/10;
        startLon += rawGpsData[1]/10;
        delay(500);
      }

      flightSensors.setRefPoints(startLat,startLon);

      digitalWrite(LEDR, HIGH);
      digitalWrite(LEDG, LOW);
      digitalWrite(LEDB, HIGH);
      isCalibrated = true;
      startUpStatus += "1,";
    }

    if (packetBuffer[0] == 'I')
    {
      digitalWrite(LEDR, LOW);
      digitalWrite(LEDG, HIGH);
      digitalWrite(LEDB, HIGH);
      double * attitude = flightSensors.getAttitude();
      double * gyro = flightSensors.getGyro();
      double* acc = flightSensors.getAcc();
      flightSensors.updateLocation();
      double* pos = flightSensors.getAlt();
      double* rawGpsData = flightSensors.getRawGPSPos();
      unsigned long lastExecutionTime = micros();
      double* speed = flightSensors.getSpeed(&lastExecutionTime);
      double tempLastExecutionTimeAsDouble = (double)lastExecutionTime;
      double rawDataPointers[22] = {tempLastExecutionTimeAsDouble, attitude[0], attitude[1], attitude[2], gyro[0], gyro[1], gyro[2], acc[0], acc[1], acc[2], rawGpsData[0], rawGpsData[1], pos[2], speed[0], speed[1], speed[2], outputPitch, outputRoll, outputYaw, outputPower, pos[0], pos[1]};
      sendUdpTelemetry(rawDataPointers);

      delay(500);
      telemetryString = "E";
      telemetryString.toCharArray(ReplyBuffer, 200);
      sendUdpData(ReplyBuffer);
      digitalWrite(LEDR, HIGH);
      digitalWrite(LEDG, LOW);
      digitalWrite(LEDB, HIGH);
    }

    if (packetBuffer[0] == 'G')
    {
      String reply = flightControlSystem.getConfigParameters();
      reply.toCharArray(ReplyBuffer, 200);
      sendUdpData(ReplyBuffer);
      delay(1000);
      telemetryString = "E";
      telemetryString.toCharArray(ReplyBuffer, 200);
      sendUdpData(ReplyBuffer);
    }
  }
}


bool getNewUDPPackets()
{
  int packetSize = Udp.parsePacket();
  if (packetSize != 0)
  {
    int len = Udp.read(packetBuffer, 350);
    if (len > 0) packetBuffer[len] = '\n';
    return true;
  }
  else
  {
    return false;
  }
}

void blinkTaskLED(int blinks)
{
  digitalWrite(LEDR, HIGH);
  digitalWrite(LEDG, HIGH);
  digitalWrite(LEDB, HIGH);
  for ( int i = 0; i < blinks; i++)
  {
    digitalWrite(LEDB, LOW);
    delay(250);
    digitalWrite(LEDB, HIGH);
    delay(250);
  }
}

void sendUdpData(char * dataToSend)
{
  Udp.beginPacket(Udp.remoteIP(), Udp.remotePort());
  Udp.write(dataToSend);
  Udp.endPacket();
}

void sendUdpTelemetry(double* data)
{
  Udp.beginPacket(Udp.remoteIP(), Udp.remotePort());
  Udp.write((byte*)data, 8 * 22);
  Udp.endPacket();
}
