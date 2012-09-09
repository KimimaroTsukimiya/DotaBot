using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace DotaBot
{
    class ServerChallengePacket : OutOfBandPacket
    {
        public const int MAGIC = 0x5A4F4933;


        //public int Magic { get; set; }

        public int ServerChallenge { get; set; }
        public int ClientChallenge { get; set; }

        public int AuthProtocol { get; set; }

        public SteamID ServerSteamID { get; set; }

        public bool VACSecure { get; set; }


        public ServerChallengePacket()
        {
            Type = OutOfBandPacketType.ServerChallenge;
        }


        public override void Serialize( Stream stream )
        {
            base.Serialize( stream );

            using ( var bw = new BinaryWriter( stream, Encoding.ASCII, true ) )
            {
                bw.Write( MAGIC );

                bw.Write( ServerChallenge );
                bw.Write( ClientChallenge );

                bw.Write( AuthProtocol );

                bw.Write( ServerSteamID != null ? ServerSteamID.ConvertToUInt64() : 0UL );

                bw.Write( ( byte )( VACSecure ? 1 : 0 ) );
            }
        }

        public override void Deserialize( Stream stream )
        {
            base.Deserialize( stream );

            using ( var br = new BinaryReader( stream, Encoding.ASCII, true ) )
            {
                br.ReadInt32(); // ignore magic

                ServerChallenge = br.ReadInt32();
                ClientChallenge = br.ReadInt32();

                AuthProtocol = br.ReadInt32();

                ServerSteamID = br.ReadUInt64();

                VACSecure = br.ReadByte() > 0;
            }
        }

    }
}
