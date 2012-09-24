using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot
{
    class ServerAcceptPacket : OutOfBandPacket
    {
        public int CilentChallenge { get; set; }


        public ServerAcceptPacket()
        {
            Type = OutOfBandPacketType.ServerAccept;
        }


        public override void Serialize( Stream stream )
        {
            base.Serialize( stream );

            using ( var bw = new BinaryWriter( stream, Encoding.UTF8, true ) )
            {
                bw.Write( CilentChallenge );
            }
        }

        public override void Deserialize( Stream stream )
        {
            base.Deserialize( stream );

            using ( var br = new BinaryReader( stream, Encoding.UTF8, true ) )
            {
                CilentChallenge = br.ReadInt32();
            }
        }
    }
}
