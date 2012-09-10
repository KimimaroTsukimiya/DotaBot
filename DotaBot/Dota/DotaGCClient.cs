using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using SteamKit2.GC;
using SteamKit2.GC.Dota;
using SteamKit2.GC.Dota.Internal;
using SteamKit2.GC.Internal;
using SteamKit2.Internal;

namespace DotaBot
{
    class FoundMatchEventArgs : EventArgs
    {
        public IPEndPoint Server { get; set; }
        public string Password { get; set; }
    }

    class DotaGCClient : GCClient
    {
        const uint APPID = 570;


        uint clientVersion;


        public event Action<object, FoundMatchEventArgs> FoundMatch;


        public DotaGCClient( string user, string pass )
            : base( user, pass )
        {
        }


        protected override void OnLoggedOn( SteamUser.LoggedOnCallback callback )
        {
            base.OnLoggedOn( callback );

            SteamApps.GetAppOwnershipTicket( APPID );
            SteamGames.PlayGame( APPID );
        }

        protected override void OnGCMessage( SteamGameCoordinator.MessageCallback callback )
        {
            base.OnGCMessage( callback );

            var dispatchMap = new Dictionary<uint, Action<IPacketGCMsg>>
            {
                { EGCMsg.ClientWelcome, OnClientWelcome },
                { EGCMsg.SourceTVGamesResponse, OnSourceTVGames },
                { EGCMsg.WatchGameResponse, OnWatchGame },
            };

            Action<IPacketGCMsg> func;
            if ( !dispatchMap.TryGetValue( callback.EMsg, out func ) )
                return;

            func( callback.Message );
        }

        void OnClientWelcome( IPacketGCMsg msg )
        {
            var clientWelcome = new ClientGCMsgProtobuf<CMsgClientWelcome>( msg );

            clientVersion = clientWelcome.Body.version;
            DebugLog.WriteLine( "DotaGCClient", "GC is version {0}", clientWelcome.Body.version );

            var findGames = new ClientGCMsgProtobuf<CMsgFindSourceTVGames>( EGCMsg.FindSourceTVGames );
            findGames.Body.num_games = 10;

            SteamGameCoordinator.Send( findGames, APPID );
        }

        void OnSourceTVGames( IPacketGCMsg msg )
        {
            var gamesList = new ClientGCMsgProtobuf<CMsgSourceTVGamesResponse>( msg );

            DebugLog.WriteLine( "DotaGCClient", "There are currently {0} viewable games", gamesList.Body.num_total_games );

            var game = gamesList.Body.games[ 0 ];

            var watchGame = new ClientGCMsgProtobuf<CMsgWatchGame>( EGCMsg.WatchGame );
            watchGame.Body.client_version = clientVersion;
            watchGame.Body.server_steamid = game.server_steamid;

            SteamGameCoordinator.Send( watchGame, APPID );
        }

        void OnWatchGame( IPacketGCMsg msg )
        {
            var response = new ClientGCMsgProtobuf<CMsgWatchGameResponse>( msg );

            if ( response.Body.watch_game_result == CMsgWatchGameResponse.WatchGameResult.PENDING )
            {
                DebugLog.WriteLine( "DotaGCClient", "STV details pending..." );
                return;
            }

            if ( response.Body.watch_game_result != CMsgWatchGameResponse.WatchGameResult.READY )
            {
                DebugLog.WriteLine( "DotaGCClient", "Unable to get STV details: {0}", response.Body.watch_game_result );
                return;
            }

            IPEndPoint server = new IPEndPoint(
                NetHelpers.GetIPAddress( response.Body.source_tv_public_addr ),
                ( int )response.Body.source_tv_port
            );

            DebugLog.WriteLine( "DotaGCClient", "Got STV details: {0} ({1})", server, response.Body.watch_tv_unique_secret_code );

            if ( FoundMatch != null )
            {
                FoundMatch( this, new FoundMatchEventArgs
                {
                    Server = server,
                    Password = response.Body.watch_tv_unique_secret_code.ToString(),
                } );
            }
        }
    }
}
