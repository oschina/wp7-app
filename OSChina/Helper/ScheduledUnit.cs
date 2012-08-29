/*
 * 原作者: 王俊
 * 联系qq: 113020930
 */
using System;
using System. Windows;
using Microsoft. Phone. Scheduler;

namespace OSChina
{
    public sealed class ScheduledUnit
    {
        #region Singleton
        private static readonly ScheduledUnit instance = new ScheduledUnit( );
        public static ScheduledUnit Instance { get { return instance; } }
        private ScheduledUnit( )
        {
            if ( Config.IsScheduledTask == false )
            {
                return;
            }
            if ( Config.Cookie.IsNotNullOrEmpty() && Config.UID != 0 && Config.UserAgent.IsNotNullOrEmpty())
            {
                this. StartPeriodicAgent( Config.UID );
            }

            EventSingleton. Instance. OnLoginOrLogout += (s, e) =>
                {
                    if ( (bool)e.Tag == true && Config.IsScheduledTask == true)
                    {
                        this. StartPeriodicAgent( Config.UID );
                    }
                };
        }
        #endregion

        private PeriodicTask getUserNoticeTask;
        private string periodicTaskName = "GetUserNotice_OSChina";

        private void StartPeriodicAgent(int uid)
        {
            getUserNoticeTask = ScheduledActionService. Find( periodicTaskName ) as PeriodicTask;
            if ( getUserNoticeTask != null )
            {
                RemoveAgent( periodicTaskName );
            }
            getUserNoticeTask = new PeriodicTask( periodicTaskName );
            string cookie = TakeFromCookie( Config. Cookie );
            if ( cookie. IsNullOrWhitespace( ) )
            {
                return;
            }
            else
            {
                cookie = string. Format( "{0}@{1}", cookie, uid );
            }
            getUserNoticeTask. Description = cookie;
            try
            {
                ScheduledActionService. Add( getUserNoticeTask );
                ScheduledActionService. LaunchForTest( periodicTaskName, TimeSpan. FromMinutes( 20 ) );
            }
            catch ( InvalidOperationException exception )
            {
                if ( exception. Message. Contains( "BNS Error: The action is disabled" ) )
                {
                    MessageBox. Show( "Background agents for this application have been disabled by the user." );
                }
            }
        }

        private string TakeFromCookie(string cookie)
        {
 //oscid=""; Expires=Thu, 01-Jan-1970 00:00:10 GMT; Path=/,  oscid=6U92pEmCn98UmK%2FIDSpOG7UZTJzJXzUWoJ0qb7Luv9QdAQiEWvw9BM%2FeEbrg1%2BgQlF81NCuCaLmrrusCz6Ozr2vjoXFacwSpyN44Ul1I1ktE7cpqUiA%2FGA%3D%3D; Expires=Tue, 30-Jul-2013 05:15:18 GMT; Path=/
            string[ ] parties = cookie. Split( ';' );
            if ( parties. Length >= 4 )
            {
                return string. Format( "{0};{1}", parties[ 2 ], parties[ 3 ] );
            }
            else
            {
                return null;
            }
        }

        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService. Remove( name );
            }
            catch ( Exception ex )
            {
                System. Diagnostics. Debug. WriteLine( ex. Message );
            }
        }
    }
}
