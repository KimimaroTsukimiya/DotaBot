using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using SteamKit2;

namespace DotaBot
{
    class Program
    {
        static void Main( string[] args )
        {
            DebugLog.AddListener( new ConsoleListener() );
            DebugLog.Enabled = true; // force debuglog in release

            DotaClient dc = new DotaClient( args[ 0 ], args[ 1 ] );

            dc.ConnectToGC();

            while ( true )
            {
                dc.RunFrame();
            }
        }
    }

    class ConsoleListener : IDebugListener
    {
        public void WriteLine( string category, string msg )
        {
            if ( category == "CMClient" )
                return;

            Console.WriteLine( "{0}: {1}", category, msg );
        }

    }

}
