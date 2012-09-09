using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using SteamKit2.GC;
using SteamKit2.GC.Dota;
using SteamKit2.GC.Internal;
using SteamKit2.Internal;

namespace DotaBot
{
    class DotaGCClient : GCClient
    {
        public DotaGCClient( string user, string pass )
            : base( user, pass )
        {
        }


        protected override void OnLoggedOn( SteamUser.LoggedOnCallback callback )
        {
            base.OnLoggedOn( callback );

            var playDota = new ClientMsgProtobuf<CMsgClientGamesPlayed>( EMsg.ClientGamesPlayedNoDataBlob );

            playDota.Body.games_played.Add( new CMsgClientGamesPlayed.GamePlayed
            {
                game_id = new GameID( 570 ),
            } );

            SteamClient.Send( playDota );
        }

        protected override void OnGCMessage( SteamGameCoordinator.MessageCallback callback )
        {
            base.OnGCMessage( callback );

            var dispatchMap = new Dictionary<uint, Action<IPacketGCMsg>>
            {
                { EGCMsg.ClientWelcome, OnClientWelcome },
            };

            Action<IPacketGCMsg> func;
            if ( !dispatchMap.TryGetValue( callback.EMsg, out func ) )
                return;

            func( callback.Message );
        }

        void OnClientWelcome( IPacketGCMsg msg )
        {
            var clientWelcome = new ClientGCMsgProtobuf<CMsgClientWelcome>( msg );

            DebugLog.WriteLine( "DotaGCClient", "ClientWelcome: {0}", clientWelcome.Body.version );
        }
    }
}
