/*
 * 原作者: 王俊
 * 联系qq: 113020930
 */
using System;
using System. Collections. Generic;
using System. Linq;
using System. Net;
using Microsoft. Phone. Controls;
using OSChina. Model;

namespace OSChina
{
    public partial class WordsPage : PhoneApplicationPage
    {
        #region Fields
        private int friendid;
        private string friendname;
        #endregion

        #region Properties
        private ListBoxHelper listBoxHelper { get; set; }
        #endregion

        #region Construct
        public WordsPage( )
        {
            InitializeComponent( );

            this. Loaded += (s, e) =>
                {
                    this. listBoxHelper = new ListBoxHelper( this. list_Words );
                    this. listBoxHelper. ReloadDelegate += new Action( listBoxHelper_ReloadDelegate );
                    this. listBoxHelper. Refresh( );
                };
            this. Unloaded += (s, e) =>
            {
                this. listBoxHelper. Clear( );
            };
        }

        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            if ( this. NavigationContext. QueryString. ContainsKey( "friendid" ) &&
                   this. NavigationContext. QueryString. ContainsKey( "friendname" ) )
            {
                this. friendid = this. NavigationContext. QueryString[ "friendid" ]. ToInt32( );
                this. friendname = this. NavigationContext. QueryString[ "friendname" ];
                this. lblTitle. Text = string. Format( "与 {0} 的对话", this. friendname );
            }
            base. OnNavigatedTo( e );
        }
        #endregion

        #region Private functions
        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect();
            base. OnNavigatedFrom( e );
        }
        void listBoxHelper_ReloadDelegate( )
        {
            if ( this. listBoxHelper. isLoading == true )
            {
                return;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"catalog", "4"},
                {"pageIndex", (this.listBoxHelper.allCount / 20).ToString()},
                {"pageSize","20"},
                {"id", this.friendid.ToString()},
            };
            WebClient client = Tool. SendWebClient( Config. api_comment_list, parameters );
            this. listBoxHelper. isLoading = true;
            client. DownloadStringCompleted += (s, e) =>
            {
                this. listBoxHelper. isLoading = false;
                if ( e. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取 消息中心的对话 CommentList 网络错误: {0}", e. Error. Message );
                    return;
                }
                int pageSize;
                CommentUnit[ ] newWords = Tool. GetCommentList( e. Result, out pageSize ); 
                if ( newWords != null )
                {
                    this. listBoxHelper. allCount += pageSize;
                    this. listBoxHelper. isLoadOver = pageSize < 20;
                    //筛选
                    int[ ] ids = Tool. GetIDS_ID<CommentUnit>( this. listBoxHelper. datas );
                    newWords = newWords. Where( w => ids. Contains( w. id ) == false ). ToArray( );

                    if ( newWords. Length > 0 )
                    {
                        if ( this. listBoxHelper. datas. Count > 0 )
                        {
                            this. listBoxHelper. datas[ this. listBoxHelper. datas. Count - 1 ] = newWords[ 0 ];
                            for ( int i = 1 ; i < newWords. Length ; i++ )
                            {
                                this. listBoxHelper. datas. Add( newWords[ i ] );
                            }
                        }
                        else
                        {
                            newWords. ForAll( n => this. listBoxHelper. datas. Add( n ) );
                        }
                    }
                    else
                    {
                        //去除最后项
                        if ( this. listBoxHelper. datas. Count > 0 )
                        {
                            this. listBoxHelper. datas. RemoveAt( this. listBoxHelper. datas. Count - 1 );
                        }
                    }

                    //添加最后的提示文字
                    if ( this. listBoxHelper. isLoadOver == false || this. listBoxHelper. datas. Count <= 0 )
                    {
                        this. listBoxHelper. datas. Add( this. listBoxHelper. GetLoadTip );
                    }
                }
            };
        }

        private void icon_Msg_Click(object sender, EventArgs e)
        {
            Tool. To( string. Format( "/PubMsgPage.xaml?receiverID={0}&receiver={1}", this. friendid, this. friendname ) );
        }
        #endregion
    }
}