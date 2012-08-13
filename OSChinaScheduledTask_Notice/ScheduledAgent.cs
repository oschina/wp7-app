using System. Windows;
using Microsoft. Phone. Scheduler;
using Microsoft. Phone. Shell;
using System. Linq;
using System. Net;
using OSChina. Union;
using WP7_JsonLib;
using System. Xml. Linq;
using System;

namespace OSChinaScheduledTask_Notice
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        #region 固定原始代码
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent 构造函数，初始化 UnhandledException 处理程序
        /// </remarks>
        public ScheduledAgent( )
        {
            if ( !_classInitialized )
            {
                _classInitialized = true;
                // 订阅托管的异常处理程序
                Deployment. Current. Dispatcher. BeginInvoke( delegate
                {
                    Application. Current. UnhandledException += ScheduledAgent_UnhandledException;
                } );
            }
        }

        /// 出现未处理的异常时执行的代码
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if ( System. Diagnostics. Debugger. IsAttached )
            {
                // 出现未处理的异常；强行进入调试器
                System. Diagnostics. Debugger. Break( );
            }
        }
        #endregion

        /// <summary>
        /// 執行排程工作的代理程式
        /// </summary>
        /// <param name="task">
        /// 叫用的工作
        /// </param>
        /// <remarks>
        /// 這個方法的呼叫時機為叫用週期性或耗用大量資料的工作時
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //反序列化
            //Union1 u = JsonUtil. JsonDeSerialize<Union1>( task. Description );
            //if ( u != null )
            //{
            //    this. Process_GetNotice( u );
            //}
            string[ ] parties = task. Description. Split( '@' );
            string cookie = string. Format( "oscid=''; Expires=Thu, 01-Jan-1970 00:00:10 GMT;{0}; Path=/", parties[ 0 ] );
            string uid = parties[ 1 ];
            this. Process_GetNotice( cookie, uid );
        }

        private void Process_GetNotice(string cookie, string uid)
        {
            WebClient client = new WebClient( );
            //client. Headers[ "User-Agent" ] = u. userAgent;
            client. Headers[ "Cookie" ] = cookie;
            client. DownloadStringCompleted += (s, e) =>
                {
                    if ( e.Error == null )
                    {
                        XElement root = XElement. Parse( e.Result );
                        XElement notice = root. Element( "notice" );
                        if ( notice == null )
                        {
                            return;
                        }
                        else
                        {
                            int a = int. Parse( notice. Element( "atmeCount" ). Value );
                            int b = int. Parse( notice. Element( "msgCount" ). Value );
                            int c = int. Parse( notice. Element( "reviewCount" ). Value );
                            int d = int. Parse( notice. Element( "newFansCount" ). Value );
                            UpdateMainTile( a + b + c + d );
                        }
                    }
                };
            string uri = string. Format( "http://www.oschina.net/action/api/user_notice?uid={0}&guid={1}", uid, Guid. NewGuid( ). ToString( ) );
            client. DownloadStringAsync( new System. Uri( uri, System. UriKind. Absolute ) );
        }

        private void UpdateMainTile(int noticeCount)
        {
            var appTile = ShellTile. ActiveTiles. FirstOrDefault( );
            if ( appTile != null )
            {
                StandardTileData firstTile = new StandardTileData
                {
                     Count = noticeCount
                };
                appTile. Update( firstTile );
            }
        }
    }
}