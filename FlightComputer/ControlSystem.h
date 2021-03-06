#ifndef CONTROLSYSTEM_H_
#define CONTROLSYSTEM_H_

#include "PController.h"

//The Class to handle all configuration Parameters
//---------------------------------------------------------------------------
//Version: V0.1
//Author: Tobias Rothlin
//---------------------------------------------------------------------------
//Methods:
//            ControlSystem(double** kValues, double** currentValues, double** aleronPosition, double** targetValue) . Initalies PID Controllers
//                          kValues . double [[KpPitch, KiPitch, KdPitch],[KpRoll, KiRoll, KdRoll], [KpYaw, KiYaw, Kdyaw]



class ControlSystem
{
  public:
    ControlSystem()
    {

    }

    void init(double* attitude, double* gyro, double* position, double* speed, double* targetAttitude, double* targetGyro, double* targetPosition, double* targetSpeed, double* outputPitch, double* outputRoll, double* outputYaw, double* outputPower, double* kAtt, double* kGyro, double* kPos, double* kSpeed)
    {
      pPitch.init(&pOutPitch, &attitude[0], &targetAttitude[0], &kAtt[0]);
      pRoll.init(&pOutRoll, &attitude[1], &targetAttitude[1], &kAtt[1]);
      pYaw.init(&pOutYaw, &attitude[2], &targetAttitude[2], &kAtt[2]);

      pGyroX.init(&pOutGyroX, &gyro[0], &targetGyro[0], &kGyro[0]);
      pGyroY.init(&pOutGyroY, &gyro[1], &targetGyro[1], &kGyro[1]);
      pGyroZ.init(&pOutGyroZ, &gyro[2], &targetGyro[2], &kGyro[2]);

      pPosX.init(&pOutPosX, &position[0], &targetPosition[0], &kPos[0]);
      pPosY.init(&pOutPosY, &position[1], &targetPosition[1], &kPos[1]);
      pPosZ.init(&pOutPosZ, &position[2], &targetPosition[2], &kPos[2]);

      pSpeedX.init(&pOutSpeedX, &speed[0], &targetSpeed[0], &kSpeed[0]);
      pSpeedY.init(&pOutSpeedY, &speed[1], &targetSpeed[1], &kSpeed[1]);
      pSpeedZ.init(&pOutSpeedZ, &speed[2], &targetSpeed[2], &kSpeed[2]);

      driverOutputPitch = outputPitch;
      driverOutputRoll = outputRoll;
      driverOutputYaw = outputYaw;
      driverOutPower = outputPower;

      pitchTarget = &targetAttitude[0];
      rollTarget = &targetAttitude[1];
      originalPitchTarget = targetAttitude[0];
      originalRollTarget = targetAttitude[1];

    }

    void updateValues(unsigned long* time)
    {
      pPitch.updateValues();
      pRoll.updateValues();
      pYaw.updateValues();

      pGyroX.updateValues();
      pGyroY.updateValues();
      pGyroZ.updateValues();

      pPosX.updateValues();
      pPosY.updateValues();
      pPosZ.updateValues();

      pSpeedX.updateValues();
      pSpeedY.updateValues();
      pSpeedZ.updateValues();

      *driverOutputPitch = pOutPitch + pOutGyroX;
      *driverOutputRoll = pOutRoll + pOutGyroY;
      *driverOutputYaw = pOutYaw + pOutGyroZ;
      *driverOutPower = pOutPosZ + pOutSpeedZ;

      double newPitchTarget = pOutPosX + pOutSpeedX;
      double newRollTarget = pOutPosY + pOutSpeedY;

      if(newPitchTarget > 10)
      {
        newPitchTarget = 10;
      }

      if(newPitchTarget < -10)
      {
        newPitchTarget = -10;
      }

      if(newRollTarget > 10)
      {
        newRollTarget = 10;
      }

      if(newRollTarget < -10)
      {
        newRollTarget = -10;
      }

      *pitchTarget = originalPitchTarget + newPitchTarget;
      *rollTarget = originalRollTarget - newRollTarget;
    }

    String getStatus()
    {
      String status = "ControlSystem: Status Report \n";
      status += "Pitch I:" + pPitch.getStatus();
      status += "Pitch P:" + pGyroX.getStatus();
      status += "--------------------------------\n";
      status += "Roll I:" + pRoll.getStatus();
      status += "Roll P:" + pGyroY.getStatus();
      status += "--------------------------------\n";
      status += "Yaw I:" + pYaw.getStatus();
      status += "Yaw P:" + pGyroZ.getStatus();
      status += "--------------------------------\n";
      status += "PosX I:" + pPosX.getStatus();
      status += "PosX P:" + pSpeedX.getStatus();
      status += "--------------------------------\n";
      status += "PosY I:" + pPosY.getStatus();
      status += "PosY P:" + pSpeedY.getStatus();
      status += "--------------------------------\n";
      status += "Alt I:" + pPosZ.getStatus();
      status += "Alt P:" + pSpeedZ.getStatus();
      status += "--------------------------------\n";
      return status;
    }

    String getConfigParameters()
    {
      String params = "";
      params += pPitch.getKValue() + ",";
      params += pRoll.getKValue() + ",";
      params += pYaw.getKValue() + ",";

      params += pGyroX.getKValue() + ",";
      params += pGyroY.getKValue() + ",";
      params += pGyroZ.getKValue() + ",";


      params += pPosX.getKValue() + ",";
      params += pPosY.getKValue() + ",";
      params += pPosZ.getKValue() + ",";

      params += pSpeedX.getKValue() + ",";
      params += pSpeedY.getKValue() + ",";
      params += pSpeedZ.getKValue();

      return params;
    }


  private:

    double* pitchTarget;
    double* rollTarget;

    double originalPitchTarget = 0;
    double originalRollTarget = 0;

    PController pPitch;
    PController pGyroX;
    double pOutPitch = 0;
    double pOutGyroX = 0;

    PController pRoll;
    PController pGyroY;
    double pOutRoll = 0;
    double pOutGyroY = 0;

    PController pYaw;
    PController pGyroZ;
    double pOutYaw = 0;
    double pOutGyroZ = 0;

    PController pPosX;
    PController pSpeedX;
    double pOutPosX = 0;
    double pOutSpeedX = 0;

    PController pPosY;
    PController pSpeedY;
    double pOutPosY = 0;
    double pOutSpeedY = 0;

    PController pPosZ;

    PController pSpeedZ;
    double pOutPosZ = 0;
    double pOutSpeedZ = 0;

    double* driverOutputPitch;
    double* driverOutputRoll;
    double* driverOutputYaw;
    double* driverOutPower;

};

#endif /*CONTROLSYSTEM_H_*/
