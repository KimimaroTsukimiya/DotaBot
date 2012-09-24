using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot
{
    [Flags]
    enum NetMessageFlags : byte
    {
        Reliable    = ( 1 << 0 ),
        Compressed  = ( 1 << 1 ),
        Encrypted   = ( 1 << 2 ),
        Split       = ( 1 << 3 ),
        Choked      = ( 1 << 4 ),
    }

    class NetMessagePacket : Packet // NetPacket
    {

        public int OutSequence { get; set; }
        public int InSequence { get; set; }

        public NetMessageFlags Flags { get; set; }

        public short Checksum { get; set; }

        public byte ReliableState { get; set; }

        public List<NetMessage> NetMessages { get; private set; }


        public NetMessagePacket()
        {
            NetMessages = new List<NetMessage>();
        }


        public override void Deserialize( Stream stream )
        {
            using ( var ms = new BinaryReader( stream, Encoding.UTF8, true ) )
            {
                var bf = new BitBuffer( ms.ReadBytes( ( int )stream.Length ) );

                OutSequence = bf.ReadInt32();
                InSequence = bf.ReadInt32();

                Flags = ( NetMessageFlags )bf.ReadByte();

                Checksum = bf.ReadInt16();

                ReliableState = bf.ReadByte();

                if ( ( Flags & NetMessageFlags.Reliable ) > 0 )
                {
                    // todo: read subchannel
                }
            }
        }

        public override void Serialize( Stream stream )
        {
        }

    }

    class NetMessage
    {
        // todo
    }
}
