using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WS_TestApp.msgServiceReference;

namespace WS_TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            msgServiceReference.MsgServiceClient client = new msgServiceReference.MsgServiceClient();

            client.postMessage("123", "126", "test");
            client.postMessage("123", "126", "hest");
            client.postMessage("123", "126", "pest");
            client.postMessage("126", "123", "errer");

            WireMessage[] wmsga = client.getMyMessages("126");
            foreach (WireMessage wmsg in wmsga)
            {
                Console.WriteLine(wmsg.recipientUserId + " received " + wmsg.msgText + " from " + wmsg.senderUserId);
            }

            wmsga = client.getMyMessages("123");
            foreach (WireMessage wmsg in wmsga)
            {
                Console.WriteLine(wmsg.recipientUserId + " received " + wmsg.msgText + " from " + wmsg.senderUserId);
            }

            Console.ReadLine();
            client.Close();

        }
    }
}
