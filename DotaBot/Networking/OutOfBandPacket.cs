using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot
{
    enum OutOfBandPacketType
    {
        ClientConnect = 'q',
        ServerChallenge = 'A',
        ClientAuth = 'k',
        ServerAccept = 'B',
        ServerReject = '9',
    }

    class OutOfBandPacket : Packet
    {
        public int Channel { get; set; }
        public OutOfBandPacketType Type { get; set; }


        public OutOfBandPacket()
        {
            Channel = -1;
            IsOOB = true;
        }


        public override void Serialize( Stream stream )
        {
            using ( var bw = new BinaryWriter( stream, Encoding.UTF8, true ) )
            {
                bw.Write( Channel );
                bw.Write( ( byte )Type );
            }
        }

        public override void Deserialize( Stream stream )
        {
            using ( var br = new BinaryReader( stream, Encoding.UTF8, true ) )
            {
                Channel = br.ReadInt32();
                Type = ( OutOfBandPacketType )br.ReadByte();
            }
        }
    }
}
