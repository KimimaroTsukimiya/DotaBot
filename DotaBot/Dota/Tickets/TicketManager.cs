using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace DotaBot
{
    class TicketManager
    {
        static TicketManager _instance = new TicketManager();
        public static TicketManager Instance { get { return _instance; } }


        Queue<byte[]> currentTokens;

        int connectCount = 0;
        int timeStart;

        IPAddress ipAddr;


        TicketManager()
        {
            currentTokens = new Queue<byte[]>();
            timeStart = Environment.TickCount;
        }


        public void UpdateTokens( List<byte[]> tokens )
        {
            tokens.ForEach( t => currentTokens.Enqueue( t ) );
        }
        public void UpdateIP( IPAddress ipAddr )
        {
            this.ipAddr = ipAddr;
        }


        public Ticket CraftTicket()
        {
            Debug.Assert( currentTokens.Count() > 0 );

            connectCount++;

            byte[] ticketData;

            using ( var ms = new MemoryStream() )
            using ( var bw = new BinaryWriter( ms ) )
            {
                var gcClient = DotaGCClient.Instance;

                bw.Write( gcClient.SteamID.ConvertToUInt64() );

                byte[] token = currentTokens.Dequeue();

                // gctoken tsection
                bw.Write( token.Length );
                bw.Write( token );

                byte[] sessionHeader = BuildHeader();

                // header tsection
                bw.Write( sessionHeader.Length );
                bw.Write( sessionHeader );

                byte[] appTicket = gcClient.AppTicket;

                // appticket tsection
                bw.Write( appTicket.Length );
                bw.Write( appTicket );

                ticketData = ms.ToArray();
            }

            byte[] crc = CryptoHelper.CRCHash( ticketData );

            return new Ticket
            {
                Data = ticketData,
                CRC = BitConverter.ToUInt32( crc, 0 ),
            };
        }


        byte[] BuildHeader()
        {
            using ( var ms = new MemoryStream() )
            using ( var bw = new BinaryWriter( ms ) )
            {
                bw.Write( 1 );
                bw.Write( 2 );
                bw.Write( GetIP() );
                bw.Write( 0 );
                bw.Write( Environment.TickCount - timeStart );
                bw.Write( connectCount );

                return ms.ToArray();
            }
        }

        uint GetIP()
        {
            return BitConverter.ToUInt32( ipAddr.GetAddressBytes(), 0 );
        }

    }

    class Ticket
    {
        public byte[] Data { get; set; }
        public uint CRC { get; set; }
    }
}
