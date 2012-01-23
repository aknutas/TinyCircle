using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GEETHREE
{
    public class Controller
    {
        //Variables
        private DataMaster dm;
        private Communicator cm;

        //Singleton instance
        private static Controller instance;

        //Private constructor, no external access!
        private Controller() {
            dm = new DataMaster();
            cm = new Communicator(dm);
        }

        //Get us the singleton instance
        public static Controller Instance
        {
            get
            {
                //If not created yet, create one
                if (instance == null)
                {
                    instance = new Controller();
                }
                return instance;
            }
        }
    }
}
