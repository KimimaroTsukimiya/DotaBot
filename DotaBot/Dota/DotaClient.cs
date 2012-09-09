using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace DotaBot
{
    class DotaClient
    {
        NetClient tvClient;
        DotaGCClient gcClient;


        public DotaClient()
        {
            gcClient = new DotaGCClient();
        }


        public void ConnectToGC()
        {
            gcClient.Connect();
        }
        public void ConnectToTV( IPEndPoint server )
        {
            tvClient = new NetClient( server );
        }

        public void RunFrame()
        {
            RunNetworking();

            gcClient.RunCallbacks();
        }
        void RunNetworking()
        {
            if ( tvClient == null )
                return;

            // run stv networking logic
        }
    }

}
