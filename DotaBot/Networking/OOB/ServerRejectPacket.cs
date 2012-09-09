using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot
{
    class ServerRejectPacket : OutOfBandPacket
    {
        public int CilentChallenge { get; set; }

        public string Reason { get; set; }


        public ServerRejectPacket()
        {
            Type = OutOfBandPacketType.ServerReject;
        }


        public override void Serialize( Stream stream )
        {
            base.Serialize( stream );

            using ( var bw = new BinaryWriter( stream, Encoding.ASCII, true ) )
            {
                bw.Write( CilentChallenge );
                bw.WriteNullTermString( Reason );
            }
        }

        public override void Deserialize( Stream stream )
        {
            base.Deserialize( stream );

            using ( var br = new BinaryReader( stream, Encoding.ASCII, true ) )
            {
                CilentChallenge = br.ReadInt32();
                Reason = br.ReadNullTermString();
            }
        }
    }
}
