using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot
{
    class ClientAuthPacket : OutOfBandPacket
    {
        public int Protocol { get; set; }
        public int AuthProtocol { get; set; }

        public int ServerChallenge { get; set; }
        public int ClientChallenge { get; set; }

        public string Name { get; set; }
        public string Password { get; set; }


        public ClientAuthPacket()
        {
            Type = OutOfBandPacketType.ClientAuth;

            Protocol = 40; // current dota protocol
            AuthProtocol = 3; // steam
        }


        public override void Serialize( Stream stream )
        {
            base.Serialize( stream );

            using ( var bw = new BinaryWriter( stream, Encoding.ASCII, true ) )
            {
                bw.Write( Protocol );
                bw.Write( AuthProtocol );

                bw.Write( ServerChallenge );
                bw.Write( ClientChallenge );

                bw.WriteNullTermString( Name );
                bw.WriteNullTermString( Password );
            }
        }

        public override void Deserialize( Stream stream )
        {
            base.Deserialize( stream );

            using ( var br = new BinaryReader( stream, Encoding.ASCII, true ) )
            {
                Protocol = br.ReadInt32();
                AuthProtocol = br.ReadInt32();

                ServerChallenge = br.ReadInt32();
                ClientChallenge = br.ReadInt32();

                Name = br.ReadNullTermString();
                Password = br.ReadNullTermString();
            }
        }
    }
}
