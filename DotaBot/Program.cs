using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DotaBot
{
    class Program
    {
        static void Main( string[] args )
        {
            // Valve Dota 2 Relay Server #50 (srcds012.iad-1.valve.net) (Dota 2)
            DotaClient dc = new DotaClient( new IPEndPoint( IPAddress.Parse( "204.63.214.58" ), 28069 ) );

            var clientConn = new ClientConnectPacket
            {
                ClientChallenge = 1234,
            };

            dc.Send( clientConn );

            var resp = dc.Receive() as ServerChallengePacket;

            var clientAuth = new ClientAuthPacket
            {
                ClientChallenge = resp.ClientChallenge,
                ServerChallenge = resp.ServerChallenge,

                AuthProtocol = 3,
                Protocol = 40,

                Name = "VoiDeD",
                Password = "test",

                KeyLen = 1,
            };

            dc.Send( clientAuth );

            var resp2 = dc.Receive();
        }
    }

}
