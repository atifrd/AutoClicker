using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker
{
    public class PatternInfo
    {
        string NeededCode;
        public string _NeededCode
        {
            set { NeededCode = value; }
            get { return NeededCode; }
        }
        SelectedPictureClasses[] NededdClass;
        public SelectedPictureClasses[] _NededdClass
        {
            get
            {
                return this.NededdClass;
            }
            set
            {
                this.NededdClass = value;
            }
        }

        string[] NededdClassString;
        public string[] _NededdClassString
        {
            get
            {
                return this.NededdClassString;
            }
            set
            {
                this.NededdClassString = value;
            }
        }
    }
    public class SelectedPictureClasses
    {
        string PictureClasses;
       public string _PictureClasses
       {
           set { PictureClasses = value; }
           get { return PictureClasses; }
       }
    }
}
