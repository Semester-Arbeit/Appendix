using System;
using UIKit;
namespace GroundStation
{
    public class UIStatusDisplay:UIView
    {



        private UILabel statusbar;

        public UIStatusDisplay(string name, string stateNow, bool stateOk , CoreGraphics.CGRect Frame)
        {
            this.Frame = Frame;

            UILabel myTitle = new UILabel(new CoreGraphics.CGRect( 0,0,(this.Frame.Width/3)*2,this.Frame.Height));
            myTitle.Text = name;
            this.AddSubview(myTitle);

            UILabel myState = new UILabel(new CoreGraphics.CGRect((this.Frame.Width / 3) * 2, 0, (this.Frame.Width / 3) * 1, this.Frame.Height));
            myState.Text = stateNow;
            this.AddSubview(myState);
            Console.WriteLine(this.Frame);


            statusbar = new UILabel();
            statusbar.Frame = new CoreGraphics.CGRect(0, this.Frame.Height-5, this.Frame.Width,1 );
            if (stateOk==true)
                statusbar.BackgroundColor = UIColor.Black;
            if (stateOk == false)
                statusbar.BackgroundColor = UIColor.Red;

            this.AddSubview(statusbar);
        }
    }
}
