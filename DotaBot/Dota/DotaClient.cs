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
            var ticketManager = new TicketManager();

            matchClient = new DotaMatchClient( ticketManager );

            gcClient = new DotaGCClient( user, pass, ticketManager );
            gcClient.FoundMatch += gcClient_FoundMatch;
        }


        public void ConnectToGC()
        {
            gcClient.Connect();
        }

        public void RunFrame()
        {
            matchClient.RunNetworking();
            gcClient.RunCallbacks();
        }


        void gcClient_FoundMatch( object sender, FoundMatchEventArgs e )
        {
            // todo: this code assumes we got the appticket in a timely fashion from steam
            matchClient.Connect( e.Server, e.Password, gcClient.AppTicket );
        }

    }

}
