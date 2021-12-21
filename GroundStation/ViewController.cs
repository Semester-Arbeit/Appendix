using Foundation;
using System;
using UIKit;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Microcharts;                                                             
using Microcharts.iOS;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;


namespace GroundStation
{


    public partial class ViewController : UIViewController
    {

        private DuctedFanPrototype alpha = new DuctedFanPrototype();


        public ViewController(IntPtr handle) : base(handle)
        {
        }

        
        
        ConfigFile configuration = new ConfigFile("testconfig23");
        UIconsole myConsole = new UIconsole(new CoreGraphics.CGRect(20, 800, 1330, 200));
        UIMulitView myMulitView;
        UISegmentedControl ChoseView;




        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            alpha.TelemetryUpdate += this.updateUIValues;

            myMulitView = new UIMulitView(new CoreGraphics.CGRect(100, 120, 1000, 750), alpha);
            myMulitView.rerender(0);

            View.BackgroundColor = UIColor.SystemBackgroundColor;

            UILabel flightTitle = new UILabel();
            flightTitle.Text = "Flight #13";
            flightTitle.Frame = new CoreGraphics.CGRect(1180, 130, 200, 50);
            View.AddSubview(flightTitle);

            UIButton abortFlight = new UIButton();
            abortFlight.SetTitle("Abort Flight", new UIControlState());
            abortFlight.Frame = new CoreGraphics.CGRect(1180, 180, 150, 150);
            abortFlight.BackgroundColor = UIColor.SystemRedColor;
            abortFlight.AddTarget(AbortFlightPressed, UIControlEvent.TouchDown);
            abortFlight.Layer.CornerRadius = 10;
            abortFlight.Layer.ShadowColor = new CoreGraphics.CGColor(0, 0, 0, 1);
            abortFlight.Layer.ShadowOffset = new CoreGraphics.CGSize(5, 5);
            abortFlight.Layer.ShadowOpacity = 0.2F;
            View.AddSubview(abortFlight);

            View.AddSubview(myConsole);

            ChoseView = new UISegmentedControl();
            ChoseView.Frame = new CoreGraphics.CGRect(100, 40, 1000, 40);
            ChoseView.InsertSegment("Standby", 0, true);
            ChoseView.InsertSegment("Preflight", 1, true);
            ChoseView.InsertSegment("Inflight", 2, true);
            ChoseView.AddTarget(ViewHasChanged, UIControlEvent.ValueChanged);
            ChoseView.SelectedSegment = 0 ;
            View.AddSubview(ChoseView);

            View.AddSubview(myMulitView);
        }

        public void ViewHasChanged(object sender, EventArgs e)
        {
            UISegmentedControl currentSegment = sender as UISegmentedControl;

            if(!myMulitView.rerender(UIMulitView.intToStates((int)currentSegment.SelectedSegment)))
            {
                var alert = UIAlertController.Create("Checklist!", "Not all elements of the checklist have been completed!", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("I know the risk", UIAlertActionStyle.Cancel, null));
                PresentViewController(alert, animated: true, completionHandler: null);
            }
       
        }

        public void AbortFlightPressed(object sender, EventArgs e)
        { 
            myConsole.WriteLine("ABORT FLIGHT");
            alpha.abort();
        }


        public void updateUIValues(TelemetryData telemetry)
        {
            Invoke(new Action(() =>
            {
                myConsole.WriteLine(telemetry);
                myMulitView.updateInFlightView(telemetry);
            }), 0);
        }



        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

    }
}















