using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot
{
    class NetClient
    {
        UdpClient socket;


        public NetClient( IPEndPoint endPoint )
        {
            socket = new UdpClient();
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
            byte[] data = socket.Receive( ref remoteEp );

            return PacketFactory.GetPacket( data );
        }
    }
}
