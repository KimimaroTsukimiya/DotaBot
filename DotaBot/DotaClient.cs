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
    class DotaClient
    {
        UdpClient socket;


        public DotaClient( IPEndPoint endPoint )
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
            IPEndPoint remoteEp = null;
            byte[] data = socket.Receive( ref remoteEp );

            return PacketFactory.GetPacket( data );
        }

    }
}
