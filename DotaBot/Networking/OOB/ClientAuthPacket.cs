using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

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

        public List<CCLCMsg_SplitPlayerConnect> Players { get; private set; }

        public bool IsLowViolence { get; set; }

        public short TicketLength { get; set; }
        public byte[] Ticket { get; set; }

        public ClientAuthPacket()
        {
            Type = OutOfBandPacketType.ClientAuth;

            Protocol = 40; // current dota protocol
            AuthProtocol = 3; // steam

            Players = new List<CCLCMsg_SplitPlayerConnect>();
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

                bw.Write( ( byte )Players.Count );

                foreach ( var player in Players )
                {
                    bw.Write( ( byte )CLC_Messages.clc_SplitPlayerConnect );
                    Serializer.SerializeWithLengthPrefix( stream, player, PrefixStyle.Base128 );
                }

                // todo: violence bit, auth ticket
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

                int numPlayers = br.ReadByte();

                Players.Clear();

                for ( int x = 0 ; x < numPlayers ; ++x )
                {
                    CLC_Messages msgType = ( CLC_Messages )br.ReadByte();
                    Players.Add( Serializer.DeserializeWithLengthPrefix<CCLCMsg_SplitPlayerConnect>( stream, PrefixStyle.Base128 ) );
                }

                // todo: violence bit, auth ticket
            }

        }
    }
}
