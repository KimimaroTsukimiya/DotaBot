using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot
{
    class DotaMatchClient
    {
        NetClient tvClient;


        public DotaMatchClient()
        {
            tvClient = new NetClient();
        }


        public void Connect( IPEndPoint server )
        {
            tvClient.Connect( server );

            // todo: send connect packets, etc
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
            // todo
        }
    }
}
