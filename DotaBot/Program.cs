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

            Console.Write( "Username: " );
            string user = Console.ReadLine();

            Console.Write( "Password: " );
            string pass = Console.ReadLine();

            DotaClient dc = new DotaClient( user, pass );

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
