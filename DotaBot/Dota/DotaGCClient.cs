using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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

        string user, pass;


        public DotaGCClient( string user, string pass )
        {
            this.user = user;
            this.pass = pass;

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
            callbackMgr.RunWaitCallbacks( TimeSpan.FromMilliseconds( 10 ) );
        }


        void OnConnected( SteamClient.ConnectedCallback callback )
        {
            if ( callback.Result != EResult.OK )
            {
                DebugLog.WriteLine( "DotaGCClient", "Unable to connect to steam: {0}", callback.Result );
                return;
            }

            DebugLog.WriteLine( "DotaGCClient", "Connected to steam!" );

            steamUser.LogOn( new SteamUser.LogOnDetails
            {
                Username = user,
                Password = pass,
            } );
        }
        void OnDisconnected( SteamClient.DisconnectedCallback callback )
        {
            DebugLog.WriteLine( "DotaGCClient", "Disconnected from steam!" );
        }

        void OnLoggedOn( SteamUser.LoggedOnCallback callback )
        {
            DebugLog.WriteLine( "DotaGCClient", "Logged on: {0}/{1}", callback.Result, callback.ExtendedResult );
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
