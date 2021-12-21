#ifndef SENSORS_H_
#define SENSORS_H_

#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BNO055.h>
#include <utility/imumaths.h>
#include <mbed.h>
#include <Arduino_PortentaBreakout.h>
#include "LaserRangeFinder.h"
#include "ComplementaryFilter.h"

#include <SPI.h> //Needed for SPI to GNSS

#include <Arduino_PortentaBreakout.h>
#include <SparkFun_u-blox_GNSS_Arduino_Library.h> //http://librarymanager/All#SparkFun_u-blox_GNSS



//The Class to interface between the Firmware and the IMU/GPS.
//---------------------------------------------------------------------------
//Version: V0.1
//Author: Tobias Rothlin
//---------------------------------------------------------------------------
//Methods:
//            Sensors() -> Constructor Does nothing.
//
//            bool init() -> initalises the GPS and IMU will not return until the GPS is read to use!
//                           Returns True if both Sensors work else it will return False
//
//            double* getAttitude() -> Returns the current Attitude as a double Array(pitch,roll,yaw)
//
//            double* getSpeed() -> Returns the current Speed from the GPS as a double Array(Vx,Vy,Vz)

class Sensors {
  public:
    Sensors() {
    }



    bool init() {
      SPI.begin();
      if (altitudeLaser.init() && imu.begin() && myGNSS.begin(SPI, SPI1_CS, 5000000)) {

        myGNSS.setPortOutput(COM_PORT_SPI, COM_TYPE_UBX); //Set the SPI port to output UBX only (turn off NMEA noise)
        myGNSS.saveConfigSelective(VAL_CFG_SUBSEC_IOPORT); //Save (only) the communications port settings to flash and BBR
        myGNSS.setNavigationFrequency(25, 1);
        return true;
      } else {
        return false;
      }
    }

    void initZSpeedEstimation(double alpha , double staticAccOffset)
    {
      zSpeedEstimation.init(alpha, staticAccOffset);
    }

    void initXSpeedEstimation(double alpha , double staticAccOffset)
    {
      xSpeedEstimation.init(alpha, staticAccOffset);
    }

    void initYSpeedEstimation(double alpha , double staticAccOffset)
    {
      ySpeedEstimation.init(alpha, staticAccOffset);
    }

    double* getAttitude() {
      updateAttitudeData();
      return currentAttitude;
    }

    double* getGyro() {
      updateGyroData();
      return currentGyro;
    }

    double* getAcc() {
      updateAccData();
      return currentAcc;
    }

    double* getSpeed(unsigned long* time) {
      updateCurrentSpeed(time);
      return currentSpeed;
    }

    void updateLocation()
    {
      getNewPosFromGPS();
    }

    double* getAlt()
    {
      updateCurrentAlt();
      return currentPos;
    }

    void setRefPoints(double lat, double lon)
    {
      latRefPoint = lat;
      lonRefPoint = lon;
    }

    double* getRawGPSPos()
    {
      return gpsPos;
    }

    int getNumberOfSatellites()
    {
      return sat;
    }

    void resetSpeedEstimation()
    {
      xSpeedEstimation.resetEstimation();
      ySpeedEstimation.resetEstimation();
      zSpeedEstimation.resetEstimation();
    }

  private:
    Adafruit_BNO055 imu = Adafruit_BNO055(55);
    LaserRangeFinder altitudeLaser = LaserRangeFinder();
    SFE_UBLOX_GNSS myGNSS;
    complementaryFilter zSpeedEstimation = complementaryFilter();
    complementaryFilter xSpeedEstimation = complementaryFilter();
    complementaryFilter ySpeedEstimation = complementaryFilter();

    double pitchHWCorrectionAngle =  -2.625;
    double rollHWCorrectionAngle = 4.938;

    double currentAttitude[3] = { 0, 0, 0 };
    double currentGyro[3] = { 0, 0, 0 };
    double currentAcc[3] = { 0, 0, 0 };
    double currentSpeed[3] = { 0, 0, 0 };
    double currentPos[3] = {0, 0, 0};

    double k = 111194.9266;

    double gpsPos[2] = {0, 0};
    int sat = 0;

    double latRefPoint = 0;
    double lonRefPoint = 0;

    const double pi = 3.1415;

    void updateAttitudeData() {
      imu::Vector<3> euler = imu.getVector(Adafruit_BNO055::VECTOR_EULER);
      currentAttitude[0] = euler.y() - pitchHWCorrectionAngle;
      currentAttitude[1] = euler.z() - rollHWCorrectionAngle;
      double yaw = euler.x();
      if (yaw > 180) {
        yaw = yaw - 360;
      }
      currentAttitude[2] = -yaw;
    }

    void updateGyroData() {
      imu::Vector<3> gyro = imu.getVector(Adafruit_BNO055::VECTOR_GYROSCOPE);
      currentGyro[0] = gyro[1];
      currentGyro[1] = -gyro[0];
      currentGyro[2] = gyro[2];
    }

    void updateAccData() {
      imu::Vector<3> acc = imu.getVector(Adafruit_BNO055::VECTOR_LINEARACCEL);
      currentAcc[0] = acc[0];
      currentAcc[1] = -acc[1];
      currentAcc[2] = acc[2];
    }

    void updateCurrentAlt()
    {
      currentPos[2] = ((double) altitudeLaser.getDistance()) / 100;
    }

    void getNewPosFromGPS()
    {
      double yawInRad = currentAttitude[3] * 0.01745329252;

      gpsPos[0] = (double)myGNSS.getLatitude() / 10000000;
      gpsPos[1] = (double)myGNSS.getLongitude() / 10000000;

      currentPos[1] = k * (latRefPoint - gpsPos[0]);
      currentPos[0] = k * (lonRefPoint - gpsPos[1]);
    }

    void updateCurrentSpeed(unsigned long* time) {
      currentSpeed[0] = *xSpeedEstimation.estimateSpeed(time, &currentAcc[0], &currentPos[0]);
      currentSpeed[1] = *ySpeedEstimation.estimateSpeed(time, &currentAcc[1], &currentPos[1]);
      currentSpeed[2] = *zSpeedEstimation.estimateSpeed(time, &currentAcc[2], &currentPos[2]);
    }


};

#endif /*SENSORS_H_*/
