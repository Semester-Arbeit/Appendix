using UIKit;
using System.Collections.Generic;
using System.IO;
using System;
namespace GroundStation
{
    public class Checklist : UIView
    {

        public List<CheckBox> checkBoxes = new List<CheckBox>();

        public int autoCheckNumber = -1;

        public Checklist(CoreGraphics.CGRect frameNew, Dictionary<string, bool> ListOfCheckListItems, DuctedFanPrototype connectedVehicle)
        {
            this.Frame = frameNew;
            int i = 0;
            foreach(KeyValuePair<string,bool> kyp in ListOfCheckListItems)
            {
                checkBoxes.Add(new CheckBox(kyp.Key, new CoreGraphics.CGRect(0, (50+10)*i, 500, 50), connectedVehicle, kyp.Value));
                if(kyp.Value)
                {
                    autoCheckNumber = i;
                }
                this.AddSubview(checkBoxes[i]);
                i++;
            }
            
        }



        public bool isListOK()
        {
            bool res = true;
            foreach (CheckBox element in checkBoxes)
            {
                if (!element.isChecked())
                {
                    res =  false;
                }
            }
            return res;
        }

        public void autoCheck(TelemetryData telemetry)
        {
            checkBoxes[autoCheckNumber].updateAutoComplete(telemetry);
        }
    }
}
