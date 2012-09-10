using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot
{
    static class NetHelpers
    {
        public static IPAddress GetIPAddress( uint ipAddr )
        {
            byte[] addrBytes = BitConverter.GetBytes( ipAddr );
            Array.Reverse( addrBytes );

            return new IPAddress( addrBytes );
        }
    }

    static class BinaryUtils
    {
        public static string ReadNullTermString( this BinaryReader reader, Encoding encoding )
        {
            int characterSize = encoding.GetByteCount( "e" );

            using ( MemoryStream ms = new MemoryStream() )
            {
                while ( true )
                {
                    byte[] data = new byte[ characterSize ];
                    reader.Read( data, 0, characterSize );

                    if ( encoding.GetString( data, 0, characterSize ) == "\0" )
                    {
                        break;
                    }

                    ms.Write( data, 0, data.Length );
                }

                return encoding.GetString( ms.ToArray() );
            }
        }

        public static string ReadNullTermString( this BinaryReader reader )
        {
            return ReadNullTermString( reader, Encoding.ASCII );
        }

        public static void WriteNullTermString( this BinaryWriter writer, Encoding encoding, string value )
        {
            writer.Write( value ?? "" );
            writer.Write( encoding.GetBytes( "\0" ) );
        }

        public static void WriteNullTermString( this BinaryWriter writer, string value )
        {
            WriteNullTermString( writer, Encoding.ASCII, value );
        }

    }
}
