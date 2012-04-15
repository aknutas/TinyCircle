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
using Microsoft.Phone.Controls;

namespace GEETHREE.Pages
{
    public static class StateManager
    {
        public static void SaveState(this PhoneApplicationPage phoneApplicationPage, string key, object value)
        {
            if (phoneApplicationPage.State.ContainsKey(key))
            {
                phoneApplicationPage.State.Remove(key);
            }

            phoneApplicationPage.State.Add(key, value);
        }

        public static T LoadState<T>(this PhoneApplicationPage phoneApplicationPage, string key)
            where T : class
        {
            if (phoneApplicationPage.State.ContainsKey(key))
            {
                return (T)phoneApplicationPage.State[key];
            }

            return default(T);
        }
    }
}
