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
        public static DotaMatchClient TheDotaMatchClient;

        NetClient tvClient;

        string password;
        int clientChallenge;

        public TicketManager ticketManager;


        public DotaMatchClient( TicketManager ticketMgr )
        {
            tvClient = new NetClient();

            ticketManager = ticketMgr;

            System.Diagnostics.Debug.Assert(TheDotaMatchClient == null);
            TheDotaMatchClient = this;
        }


        public void Connect( IPEndPoint server, string password, byte[] appTicket )
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
                    // HandleServerAccept( packet as ServerAcceptPacket ); // todo
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
            
            var splitvars = new CCLCMsg_SplitPlayerConnect();
            splitvars.convars = new CMsg_CVars();

            AddDefaultConVars(splitvars.convars.cvars);

            clientAuth.Players.Add( splitvars ); // todo: do we need to send any userinfo convars?

            clientAuth.ClientChallenge = clientChallenge;
            clientAuth.ServerChallenge = packet.ServerChallenge;

            var gcClient = DotaGCClient.TheDotaGCClient;
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(gcClient.GetSteamUser().SteamID.ConvertToUInt64());
                    bw.Write(ticketManager.CraftTicket().Data);
                    bw.Write(DotaGCClient.TheDotaGCClient.AppTicket.Length);
                    bw.Write(DotaGCClient.TheDotaGCClient.AppTicket);
                }

                clientAuth.Ticket = ms.ToArray();
            }

            tvClient.Send( clientAuth );

            // todo: have gc client auth with steam
        }
        void HandleServerReject( ServerRejectPacket packet )
        {
            DebugLog.WriteLine( "DotaMatchClient", "Rejected from server: {0}", packet.Reason );
        }

        private void AddDefaultConVars(List<CMsg_CVars.CVar> cvars)
        {
            var temp = new CMsg_CVars.CVar();
            temp.name = "tv_nochat";
            temp.value = "1";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "name";
            temp.value = DotaGCClient.TheDotaGCClient.GetSteamFriends().GetPersonaName();
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "tv_listen_voice_indicies";
            temp.value = "0";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "steamworks_sessionid_client";
            temp.value = "0";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "joy_autoaimdampen";
            temp.value = "0";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_interp_ratio";
            temp.value = "2";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_predict";
            temp.value = "0";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_updaterate";
            temp.value = "30";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_showhelp";
            temp.value = "1";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_mouselook";
            temp.value = "1";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "voice_loopback";
            temp.value = "0";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_lagcompensation";
            temp.value = "1";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "closecaption";
            temp.value = "1";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_language";
            temp.value = "english";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_class";
            temp.value = "default";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "snd_voipvolume";
            temp.value = "1";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "snd_musicvoume";
            temp.value = "1";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_cmdrate";
            temp.value = "30";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "net_maxroutable";
            temp.value = "1200";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_team";
            temp.value = "default";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "rate";
            temp.value = "80000";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_predictweapons";
            temp.value = "1";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_interpolate";
            temp.value = "1";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_interp";
            temp.value = "0.05";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "snd_gamevoume";
            temp.value = "1";
            cvars.Add(temp);

            temp = new CMsg_CVars.CVar();
            temp.name = "cl_specmode";
            temp.value = "1";
            cvars.Add(temp);
        }
    }
}
