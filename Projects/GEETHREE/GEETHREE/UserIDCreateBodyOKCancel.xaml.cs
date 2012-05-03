using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GEETHREE
{
    public partial class UserIDCreateBodyOKCancel : UserControl
    {
       
        public UserIDCreateBodyOKCancel(bool a)
        {
            InitializeComponent();
            if (a == false)
            {
                mainbody.Text = "\nTinyCircle Exits now.\n\nYou can always come back and create UserID! \n\nThank You!!!";
            }
            else
            {
                mainbody.Text = "\nUserID created! \n\n" + Controller.Instance.getCurrentUserID();
            }
        }
    }
}
