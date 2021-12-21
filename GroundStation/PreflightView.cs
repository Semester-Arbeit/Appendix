using UIKit;
using System;

using System.Collections.Generic;
namespace GroundStation
{
    public class PreflightView : UIView
    {



        public Checklist leftChecklist;
        public Checklist rightChcklist;

        public Dictionary<string, bool> leftChecklistTitles = new Dictionary<string, bool>();

        public Dictionary<string, bool> rightChecklistTitles = new Dictionary<string,bool>();

        public PreflightView(CoreGraphics.CGRect Frame, DuctedFanPrototype connectedVehicle)
        {

            this.Frame = Frame;

            leftChecklistTitles.Add("Visual inspection of drone completed", false);
            leftChecklistTitles.Add("No structual damage found", false);
            leftChecklistTitles.Add("Liftfan is clear", false);
            leftChecklistTitles.Add("All cabels are secured", false);
            leftChecklistTitles.Add("Batteries are secured", false);
            leftChecklistTitles.Add("SD card has the flight profile loaded", false);
            leftChecklistTitles.Add("Drone has the newest control parameters", false);
            leftChecklistTitles.Add("5V power supply power connector is connected", false);
            leftChecklistTitles.Add("ESC power connector is connected", false);
            leftChecklistTitles.Add("All PWM cabels are connected", false);
            leftChecklistTitles.Add("All I2C cables are connected", false);

            rightChecklistTitles.Add("Both LiPo Batteries are connected", false);
            rightChecklistTitles.Add("LiPo voltage is min 30V", false);
            rightChecklistTitles.Add("5V voltage is min 4.8V", false);
            rightChecklistTitles.Add("FightComputer is starting up (Blinks 10x blue)", false);
            rightChecklistTitles.Add("Power monitoring arduino is ready (Green LED)", false);
            rightChecklistTitles.Add("All Flaps work", false);
            rightChecklistTitles.Add("The Ipad is connected to the 'Alpha' network", false);
            rightChecklistTitles.Add("Flightcomputer is ready(Green LED)", false);
            rightChecklistTitles.Add("All internal checks have passed", true);
            rightChecklistTitles.Add("The GPS is connected and has a current location", false);
            rightChecklistTitles.Add("The flight area is clear", false);


            leftChecklist = new Checklist(new CoreGraphics.CGRect(0, 0, (this.Frame.Width / 2) - 10, (float)this.Frame.Height), leftChecklistTitles, connectedVehicle);
            rightChcklist = new Checklist(new CoreGraphics.CGRect((this.Frame.Width / 2) + 10, 0, this.Frame.Width / 2, (float)this.Frame.Height), rightChecklistTitles, connectedVehicle);
            this.AddSubview(leftChecklist);
            this.AddSubview(rightChcklist);
        }

        public bool areAllChecksCompleted()
        {
            bool res1 = leftChecklist.isListOK();
            bool res2 = rightChcklist.isListOK();
            return res1 && res2;
        }

        public void autoCheck(TelemetryData telemetry)
        {
            rightChcklist.autoCheck(telemetry);
        }

    }
}
