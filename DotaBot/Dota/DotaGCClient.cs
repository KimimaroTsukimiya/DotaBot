using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace DotaBot
{
    class DotaGCClient
    {
        SteamClient steamClient;

        CallbackManager callbackMgr;

        SteamUser steamUser;
        SteamGameCoordinator gameCoordinator;


        public DotaGCClient()
        {
            steamClient = new SteamClient();

            callbackMgr = new CallbackManager( steamClient );

            steamUser = steamClient.GetHandler<SteamUser>();
            gameCoordinator = steamClient.GetHandler<SteamGameCoordinator>();

            new Callback<SteamClient.ConnectedCallback>( OnConnected, callbackMgr );
            new Callback<SteamClient.DisconnectedCallback>( OnDisconnected, callbackMgr );

            new Callback<SteamUser.LoggedOnCallback>( OnLoggedOn, callbackMgr );
            new Callback<SteamUser.LoggedOffCallback>( OnLoggedOff, callbackMgr );

            new Callback<SteamGameCoordinator.MessageCallback>( OnGCMessage, callbackMgr );
        }


        public void Connect()
        {
            steamClient.Connect();
        }


        public void RunCallbacks()
        {
            callbackMgr.RunWaitCallbacks( TimeSpan.FromSeconds( 1 ) );
        }


        void OnConnected( SteamClient.ConnectedCallback callback )
        {
            if ( callback.Result != EResult.OK )
            {
                DebugLog.WriteLine( "DotaGCClient", "Unable to connect: {0}", callback.Result );
                return;
            }

            // todo: logon
        }
        void OnDisconnected( SteamClient.DisconnectedCallback callback )
        {
            DebugLog.WriteLine( "DotaGCClient", "Disconnected from steam!" );
        }

        void OnLoggedOn( SteamUser.LoggedOnCallback callback )
        {
        }
        void OnLoggedOff( SteamUser.LoggedOffCallback callback )
        {
            DebugLog.WriteLine( "DotaGCClient", "Logged off of steam: {0}", callback.Result );
        }

        void OnGCMessage( SteamGameCoordinator.MessageCallback callback )
        {
        }
    }
}
