using System;
using System. Collections. Generic;
using System. Linq;
using System. Net;
using System. Windows;
using System. Windows. Controls;
using System. Windows. Documents;
using System. Windows. Input;
using System. Windows. Media;
using System. Windows. Media. Animation;
using System. Windows. Shapes;
using Microsoft. Phone. Controls;
using OSChina. Model;
using OSChina. Model. AppOnly;

namespace OSChina
{
    public partial class WordsPage : PhoneApplicationPage
    {
        private int friendid;
        private string friendname;

        private ListBoxHelper listBoxHelper { get; set; }

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

                //listBoxHelper_ReloadDelegate( );
            }
            base. OnNavigatedTo( e );
        }
        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect( );
            base. OnNavigatedFrom( e );
        }
        void listBoxHelper_ReloadDelegate( )
        {
            if ( this. listBoxHelper. isLoading == true )
            {
                return;
            }
//            catalog	 整数	 表示评论列表所属元素的类型， 1 -- 新闻  2 -- 帖子  3 -- 动弹  4 -- 消息中心的消息
//id	 整数	 表示某条新闻，帖子，动弹的id  或者某条消息的 friendid
//pageIndex	 整数	 页面索引
//pageSize	 整数	 每页读取的评论数，例如一次读取10个评论
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
                    //去除最后的提示文字
                    //this. listBoxHelper. datas. RemoveAt( this. listBoxHelper. datas. Count - 1 );
                    ////添加刚才的元素
                    //newWords. ForAll( w => this. listBoxHelper. datas. Add( w ) );
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
    }
}