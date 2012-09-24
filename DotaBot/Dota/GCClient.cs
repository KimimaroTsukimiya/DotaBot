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
    abstract class GCClient
    {
        public SteamClient SteamClient { get; private set; }

        protected CallbackManager CallbackManager { get; private set; }

        public SteamUser SteamUser { get; private set; }
        public SteamGameCoordinator SteamGameCoordinator { get; private set; }
        public SteamApps SteamApps { get; private set; }
        public SteamGames SteamGames { get; private set; }
        public SteamFriends SteamFriends { get; private set; }

        public string Username { get; private set; }
        public string Password { get; private set; }


        public byte[] AppTicket { get; private set; }


        DateTime nextConnect;


        public GCClient()
        {
            SteamClient = new SteamClient();
            SteamClient.AddHandler( new SteamGames() );

            CallbackManager = new CallbackManager( SteamClient );

            SteamUser = SteamClient.GetHandler<SteamUser>();
            SteamGameCoordinator = SteamClient.GetHandler<SteamGameCoordinator>();
            SteamApps = SteamClient.GetHandler<SteamApps>();
            SteamGames = SteamClient.GetHandler<SteamGames>();
            SteamFriends = SteamClient.GetHandler<SteamFriends>();

            new Callback<SteamClient.ConnectedCallback>( OnConnected, CallbackManager );
            new Callback<SteamClient.DisconnectedCallback>( OnDisconnected, CallbackManager );

            new Callback<SteamUser.LoggedOnCallback>( OnLoggedOn, CallbackManager );
            new Callback<SteamUser.LoggedOffCallback>( OnLoggedOff, CallbackManager );

            new Callback<SteamApps.LicenseListCallback>( OnLicenseList, CallbackManager );

            new Callback<SteamGameCoordinator.MessageCallback>( OnGCMessage, CallbackManager );

            new JobCallback<SteamApps.AppOwnershipTicketCallback>( OnAppTicket, CallbackManager );
        }



        public void Connect( string user, string pass )
        {
            Username = user;
            Password = pass;

            nextConnect = DateTime.Now;
        }

        public void RunCallbacks()
        {
            if ( DateTime.Now >= nextConnect )
            {
                nextConnect = DateTime.MaxValue;
                SteamClient.Connect();
            }

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

            nextConnect = DateTime.Now + TimeSpan.FromSeconds( 30 );
        }

        protected virtual void OnLoggedOn( SteamUser.LoggedOnCallback callback )
        {
            DebugLog.WriteLine( "GCClient", "Logged on: {0}/{1}", callback.Result, callback.ExtendedResult );
        }
        protected virtual void OnLoggedOff( SteamUser.LoggedOffCallback callback )
        {
            DebugLog.WriteLine( "GCClient", "Logged off of steam: {0}", callback.Result );
        }

        protected virtual void OnLicenseList( SteamApps.LicenseListCallback callback )
        {
            DebugLog.WriteLine( "GCClient", "Got license list" );
        }

        protected virtual void OnGCMessage( SteamGameCoordinator.MessageCallback callback )
        {
            DebugLog.WriteLine( "GCClient", "OnGCMessage: {0}", GetEMsgDisplayString( callback.EMsg ) );
        }

        protected virtual void OnAppTicket( SteamApps.AppOwnershipTicketCallback callback, JobID jobId )
        {
            if ( callback.Result != EResult.OK )
            {
                DebugLog.WriteLine( "GCClient", "Unable to get app ticket!" );
                return;
            }

            DebugLog.WriteLine( "GCClient", "Got {0} appticket", callback.AppID );

            AppTicket = callback.Ticket;
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
