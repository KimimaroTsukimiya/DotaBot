using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot
{
    class BitReader : IDisposable
    {
        BinaryReader reader;

        readonly uint[] MaskTable =
        {
            0,
            ( 1 << 1 ) - 1,
            ( 1 << 2 ) - 1,
            ( 1 << 3 ) - 1,
            ( 1 << 4 ) - 1,
            ( 1 << 5 ) - 1,
            ( 1 << 6 ) - 1,
            ( 1 << 7 ) - 1,
            ( 1 << 8 ) - 1,
            ( 1 << 9 ) - 1,
            ( 1 << 10 ) - 1,
            ( 1 << 11 ) - 1,
            ( 1 << 12 ) - 1,
            ( 1 << 13 ) - 1,
            ( 1 << 14 ) - 1,
            ( 1 << 15 ) - 1,
            ( 1 << 16 ) - 1,
            ( 1 << 17 ) - 1,
            ( 1 << 18 ) - 1,
            ( 1 << 19 ) - 1,
            ( 1 << 20 ) - 1,
            ( 1 << 21 ) - 1,
            ( 1 << 22 ) - 1,
            ( 1 << 23 ) - 1,
            ( 1 << 24 ) - 1,
            ( 1 << 25 ) - 1,
            ( 1 << 26 ) - 1,
            ( 1 << 27 ) - 1,
            ( 1 << 28 ) - 1,
            ( 1 << 29 ) - 1,
            ( 1 << 30 ) - 1,
            0x7fffffff,
            0xffffffff,
        };

        int bitsInBuffer;
        uint bufferedWord;


        public BitReader( Stream stream, bool leaveOpen = false )
        {
            reader = new BinaryReader( stream, Encoding.UTF8, leaveOpen );
        }


        public byte[] ReadBits( int numBits )
        {
            byte[] output = new byte[ numBits >> 3 ];
            int numBitsLeft = numBits;

            for ( int x = 0 ; x < output.Length  ; ++x )
            {
                output[ x ] = ( byte )ReadUBitLong( 8 );
            }

            return output;
        }

        public byte[] ReadBytes( int numBytes )
        {
            return ReadBits( numBytes << 3 );
        }

        public uint ReadUBitLong( int numBits )
        {
            if ( bitsInBuffer >= numBits )
            {
                // we have enough buffered bits to read this off

                uint nRet = bufferedWord & MaskTable[ numBits ];
                bitsInBuffer -= numBits;

                if ( bitsInBuffer > 0 )
                {
                    // still some bits left
                    bufferedWord >>= numBits;
                }
                else
                {
                    // no bits left, grab 32 more
                    LoadNextDWord();
                }

                return nRet;
            }
            else
            {
                // not enough bits, merge what's buffered with the next dword

                uint nRet = bufferedWord;
                numBits -= bitsInBuffer;

                LoadNextDWord();

                nRet |= ( ( bufferedWord & MaskTable[ numBits ] ) << bitsInBuffer );

                bitsInBuffer = 32 - numBits;
                bufferedWord >>= numBits;

                return nRet;
            }
        }
        public int ReadSBitLong( int numBits )
        {
            uint r = ReadUBitLong( numBits );
            uint s = ( uint )( 1 << ( numBits - 1 ) );

            if ( r >= s )
            {
                r = r - s - s;
            }

            return ( int )r;
        }

        public uint ReadBitLong( int numBits, bool signed )
        {
            if ( signed )
                return ( uint )ReadSBitLong( numBits );
            else
                return ReadUBitLong( numBits );
        }

        public bool ReadOneBit()
        {
            return ReadUBitLong( 1 ) > 0;
        }


        public void Dispose()
        {
            reader.Dispose();
        }


        void LoadNextDWord()
        {
            int remaining = ( int )( reader.BaseStream.Length - reader.BaseStream.Position );

            switch ( remaining )
            {
                case 3:
                    bufferedWord = ( uint )( reader.ReadByte() | ( reader.ReadByte() << 8 ) | ( reader.ReadByte() << 16 ) );
                    break;

                case 2:
                    bufferedWord = reader.ReadUInt16();
                    break;

                case 1:
                    bufferedWord = ( uint )reader.ReadByte();
                    break;

                case 0:
                    throw new Exception( "wtf" );

                default:
                    bufferedWord = reader.ReadUInt32();
                    break;
            }

            bitsInBuffer = 32;
        }
    }
}
