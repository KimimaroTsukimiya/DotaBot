using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using SteamKit2.GC.Dota;
using SteamKit2.Internal;

namespace DotaBot
{
    class GCClient
    {
        protected SteamClient SteamClient { get; private set; }

        protected CallbackManager CallbackManager { get; private set; }

        protected SteamUser SteamUser { get; private set; }
        protected SteamGameCoordinator SteamGameCoordinator { get; private set; }

        protected string Username { get; private set; }
        protected string Password { get; private set; }


        public GCClient( string user, string pass )
        {
            Username = user;
            Password = pass;

            SteamClient = new SteamClient();

            CallbackManager = new CallbackManager( SteamClient );

            SteamUser = SteamClient.GetHandler<SteamUser>();
            SteamGameCoordinator = SteamClient.GetHandler<SteamGameCoordinator>();

            new Callback<SteamClient.ConnectedCallback>( OnConnected, CallbackManager );
            new Callback<SteamClient.DisconnectedCallback>( OnDisconnected, CallbackManager );

            new Callback<SteamUser.LoggedOnCallback>( OnLoggedOn, CallbackManager );
            new Callback<SteamUser.LoggedOffCallback>( OnLoggedOff, CallbackManager );

            new Callback<SteamGameCoordinator.MessageCallback>( OnGCMessage, CallbackManager );
        }


        public void Connect()
        {
            SteamClient.Connect();
        }

        public void RunCallbacks()
        {
            CallbackManager.RunWaitCallbacks( TimeSpan.FromMilliseconds( 10 ) );
        }


        protected virtual void OnConnected( SteamClient.ConnectedCallback callback )
        {
            if ( callback.Result != EResult.OK )
            {
                DebugLog.WriteLine( "GCClient", "Unable to connect to steam: {0}", callback.Result );
                return;
            }

            DebugLog.WriteLine( "GCClient", "Connected to steam!" );

            SteamUser.LogOn( new SteamUser.LogOnDetails
            {
                Username = Username,
                Password = Password,
            } );
        }
        protected virtual void OnDisconnected( SteamClient.DisconnectedCallback callback )
        {
            DebugLog.WriteLine( "GCClient", "Disconnected from steam!" );
        }

        protected virtual void OnLoggedOn( SteamUser.LoggedOnCallback callback )
        {
            DebugLog.WriteLine( "GCClient", "Logged on: {0}/{1}", callback.Result, callback.ExtendedResult );
        }
        protected virtual void OnLoggedOff( SteamUser.LoggedOffCallback callback )
        {
            DebugLog.WriteLine( "GCClient", "Logged off of steam: {0}", callback.Result );
        }

        protected virtual void OnGCMessage( SteamGameCoordinator.MessageCallback callback )
        {
            DebugLog.WriteLine( "GCClient", "OnGCMessage: {0}", GetEMsgDisplayString( callback.EMsg ) );
        }


        static string GetEMsgDisplayString( uint eMsg )
        {
            var fields = typeof( EGCMsg ).GetFields( BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy );

            var field = fields.SingleOrDefault( f =>
            {
                uint value = ( uint )f.GetValue( null );
                return value == eMsg;
            } );

            if ( field == null )
                return eMsg.ToString();

            return field.Name;
        }
    }
}
