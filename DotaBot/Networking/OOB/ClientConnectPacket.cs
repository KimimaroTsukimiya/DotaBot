using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot
{
    class ClientConnectPacket : OutOfBandPacket
    {
        public int ClientChallenge { get; set; }


        public ClientConnectPacket()
        {
            Type = OutOfBandPacketType.ClientConnect;
        }


        public override void Serialize( Stream stream )
        {
            base.Serialize( stream );

            using ( var bw = new BinaryWriter( stream, Encoding.ASCII, true ) )
            {
                bw.Write( ClientChallenge );
                bw.WriteNullTermString( "0000000000" ); // write padding
            }
        }

        public override void Deserialize( Stream stream )
        {
            base.Deserialize( stream );

            using ( var br = new BinaryReader( stream, Encoding.ASCII, true ) )
            {
                ClientChallenge = br.ReadInt32();
                // we don't care about the padding when reading
            }
        }
    }
}
