using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot
{
    abstract class Packet
    {
        public bool IsOOB { get; set; }


        public abstract void Serialize( Stream stream );
        public abstract void Deserialize( Stream stream );
    }
}
