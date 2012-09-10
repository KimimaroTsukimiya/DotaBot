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
    class NetClient
    {
        UdpClient socket;


        public NetClient()
        {
            socket = new UdpClient();
        }


        public void Connect( IPEndPoint endPoint )
        {
            socket.Connect( endPoint );
        }

        public void Send( Packet packet )
        {
            using ( var ms = new MemoryStream() )
            {
                packet.Serialize( ms );

                byte[] data = ms.ToArray();
                socket.Send( data, data.Length );
            }
        }

        public Packet Receive()
        {
            if ( socket.Available == 0 )
                return null; // no packet yet

            IPEndPoint remoteEp = null;
            byte[] data = null;

            try
            {
                data = socket.Receive( ref remoteEp );
            }
            catch ( SocketException ex )
            {
                DebugLog.WriteLine( "NetClient", "Unable to receive packet: {0}", ex );
                return null;
            }

            return PacketFactory.GetPacket( data );
        }
    }
}
