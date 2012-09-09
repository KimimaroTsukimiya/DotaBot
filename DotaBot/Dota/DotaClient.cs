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
        DotaMatchClient matchClient;
        DotaGCClient gcClient;


        public DotaClient( string user, string pass )
        {
            matchClient = new DotaMatchClient();
            gcClient = new DotaGCClient( user, pass );
        }


        public void ConnectToGC()
        {
            gcClient.Connect();
        }
        public void ConnectToMatch( IPEndPoint server )
        {
            matchClient.Connect( server );
        }

        public void RunFrame()
        {
            matchClient.RunNetworking();
            gcClient.RunCallbacks();
        }

    }

}
