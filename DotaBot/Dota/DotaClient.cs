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


        public DotaClient()
        {
            matchClient = new DotaMatchClient();

            DotaGCClient.Instance.FoundMatch += gcClient_FoundMatch;
        }


        public void ConnectToGC( string user, string pass )
        {
            DotaGCClient.Instance.Connect( user, pass );
        }

        public void RunFrame()
        {
            matchClient.RunNetworking();
            DotaGCClient.Instance.RunCallbacks();
        }


        void gcClient_FoundMatch( object sender, FoundMatchEventArgs e )
        {
            // todo: this code assumes we got the appticket in a timely fashion from steam
            matchClient.Connect( e.Server, e.Password );
        }

    }

}
