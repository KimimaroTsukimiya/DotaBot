using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SteamKit2;
using System.IO;

namespace DotaBot
{
    class DotaMatchClient
    {
        NetClient tvClient;

        string password;
        int clientChallenge;


        public DotaMatchClient()
        {
            tvClient = new NetClient();
        }


        public void Connect( IPEndPoint server, string password )
        {
            DebugLog.WriteLine( "DotaMatchClient", "Connecting to {0} using {1}", server, password );

            this.password = password;
            clientChallenge = new Random().Next();

            tvClient.Connect( server );

            var connect = new ClientConnectPacket();
            connect.ClientChallenge = clientChallenge;

            tvClient.Send( connect );

            // todo: retry logic?
            // assuming fair network conditions for now
        }


        public void RunNetworking()
        {
            var packet = tvClient.Receive();

            if ( packet == null )
                return;

            if ( packet.IsOOB )
            {
                HandleOOBPacket( packet as OutOfBandPacket );
            }
            else
            {
                // todo: we probablty want a NetPacket or something to handle splits and net messages
                // HandlePacket( packet );
            }
        }

        void HandleOOBPacket( OutOfBandPacket packet )
        {
            switch ( packet.Type )
            {
                case OutOfBandPacketType.ServerChallenge:
                    HandleServerChallenge( packet as ServerChallengePacket );
                    break;

                case OutOfBandPacketType.ServerReject:
                    HandleServerReject( packet as ServerRejectPacket );
                    break;

                case OutOfBandPacketType.ServerAccept:
                    HandleServerAccept( packet as ServerAcceptPacket );
                    break;
            }
        }

        void HandleServerChallenge( ServerChallengePacket packet )
        {
            DebugLog.WriteLine( "DotaMatchClient", "Sending client auth..." );

            var clientAuth = new ClientAuthPacket();

            clientAuth.Name = "test";
            clientAuth.Password = password;

            clientAuth.IsLowViolence = false;
            
            // note: we're not sending split screen data, it's not required

            clientAuth.ClientChallenge = clientChallenge;
            clientAuth.ServerChallenge = packet.ServerChallenge;

            clientAuth.Ticket = TicketManager.Instance.CraftTicket().Data;

            tvClient.Send( clientAuth );

            // todo: have gc client auth with steam
        }
        void HandleServerReject( ServerRejectPacket packet )
        {
            DebugLog.WriteLine( "DotaMatchClient", "Rejected from server: {0}", packet.Reason );
        }

        void HandleServerAccept( ServerAcceptPacket packet )
        {
            DebugLog.WriteLine( "DotaMatchClient", "We have been accepted to the server!" );
        }

    }
}
