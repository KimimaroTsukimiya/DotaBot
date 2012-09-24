using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using SteamKit2.Internal;

namespace DotaBot
{
    class SteamGames : ClientMsgHandler
    {
        public void PlayGame( uint appId )
        {
            var playMsg = new ClientMsgProtobuf<CMsgClientGamesPlayed>( EMsg.ClientGamesPlayedNoDataBlob );

            playMsg.Body.games_played.Add( new CMsgClientGamesPlayed.GamePlayed
            {
                game_id = new GameID( appId )
            } );

            Client.Send( playMsg );
        }


        public override void HandleMsg( IPacketMsg packetMsg )
        {
            switch ( packetMsg.MsgType )
            {
                case EMsg.ClientGameConnectTokens:
                    HandleGameConnectTokens( packetMsg );
                    break;
            }
        }

        void HandleGameConnectTokens( IPacketMsg packetMsg )
        {
            var gameConnectTokens = new ClientMsgProtobuf<CMsgClientGameConnectTokens>( packetMsg );

            TicketManager.Instance.UpdateTokens( gameConnectTokens.Body.tokens );
        }
    }
}
