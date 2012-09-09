using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DotaBot
{
    class Program
    {
        static void Main( string[] args )
        {
            DotaClient dc = new DotaClient();

            dc.ConnectToGC();

            while ( true )
            {
                dc.RunFrame();
            }
        }
    }

}
