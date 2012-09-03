/*
 * 原作者: 王俊
 * 
 */
using System;
using System. Collections. Generic;
using System. Linq;
using System. Net;
using System. Windows;
using System. Windows. Controls;
using System. Windows. Input;
using Microsoft. Phone. Controls;
using OSChina. Model;
using OSChina. Model. AppOnly;

namespace OSChina. Controls
{
    public partial class TweetListControl : UserControl
    {
        #region Properties
        /// <summary>
        /// ListBox辅助对象
        /// </summary>
        public ListBoxHelper listBoxHelper { get; private set; }
        
        /// <summary>
        /// 动弹类型
        /// </summary>
        private TweetType _tweetType;
        public TweetType tweetType
        {
            get { return this. _tweetType; }
            set 
            {
                bool isNeedRefresh = ( _tweetType != value );
                _tweetType = value;
                if ( isNeedRefresh )
                {
                    if ( this.listBoxHelper != null )
                    {
                        this. listBoxHelper. Refresh( );
                    }
                }
            }
        }

        /// <summary>
        /// 大图弹出框
        /// </summary>
        private PopUpImage pop;
        #endregion

        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public TweetListControl( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
                {
                    this. listBoxHelper = new ListBoxHelper( this. list_Tweets );
                    this. listBoxHelper. ReloadDelegate += new Action( listBoxHelper_ReloadDelegate );
                };
            
            this. Unloaded += (s, e) =>
            {
                this. listBoxHelper. Clear( );
            };
        }

        #endregion

        #region Public functions
        /// <summary>
        /// 刷新动弹列表
        /// </summary>
        public void Refresh( )
        {
            //记住现有的 id 集合
            this. listBoxHelper. ids4Refresh = Tool. GetIDS_ID<TweetUnit>( this. listBoxHelper. datas ). Take( 20 ). ToList( );
            this. listBoxHelper. Refresh( );
        }

        /// <summary>
        /// 继续获取动弹列表
        /// </summary>
        public void listBoxHelper_ReloadDelegate( )
        {
            if ( this. listBoxHelper. isLoading == true )
            {
                return;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"uid", GetUID( ).ToString()},
                {"pageIndex", (this. listBoxHelper. allCount / 20).ToString()},
                {"pageSize", "20"},
            };
            WebClient client = Tool. SendWebClient( Config. api_tweet_list, parameters );
            this. listBoxHelper. isLoading = true;
            client. DownloadStringCompleted += (s, e) =>
            {
                this. listBoxHelper. isLoading = false;
                if ( e. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取 TweetList 网络错误: {0}", e. Error. Message );
                    return;
                }
                int pageSize, tweetCount;
                TweetUnit[ ] newTweets = Tool. GetTweetList( e. Result, out pageSize, out tweetCount );
                if ( newTweets != null )
                {
                    this. listBoxHelper. allCount += pageSize;
                    //这里需要特殊处理 因为热门动弹总共就20条
                    this. listBoxHelper. isLoadOver = pageSize < 20 || pageSize >= tweetCount;
                    //如果是刷新则提示有数据 注意必须在筛选前就处理好
                    if ( this. listBoxHelper. ids4Refresh. Count > 0 )
                    {
                        int count = newTweets. Where( t => this. listBoxHelper. ids4Refresh. Contains( t. id ) == false ). Count( );
                        if ( count > 0 )
                        {
                            EventSingleton. Instance. ToastMessage( null, string. Format( "{0} 条新动弹", count ) );
                        }
                        this. listBoxHelper. ids4Refresh. Clear( );
                    }
                    //筛选
                    int[ ] ids = Tool. GetIDS_ID<TweetUnit>( this. listBoxHelper. datas );
                    newTweets = newTweets. Where( t => ids. Contains( t. id ) == false ). ToArray( );
                    //去除最后的提示文字
                    //this. listBoxHelper. datas. RemoveAt( this. listBoxHelper. datas. Count - 1 );
                    ////添加新加入的问题列表
                    //newTweets. ForAll( t => this. listBoxHelper. datas. Add( t ) );
                    if ( newTweets. Length > 0 )
                    {
                        if ( this. listBoxHelper. datas. Count > 0 )
                        {
                            this. listBoxHelper. datas[ this. listBoxHelper. datas. Count - 1 ] = newTweets[ 0 ];
                            for ( int i = 1 ; i < newTweets. Length ; i++ )
                            {
                                this. listBoxHelper. datas. Add( newTweets[ i ] );
                            }
                        }
                        else
                        {
                            newTweets. ForAll( n => this. listBoxHelper. datas. Add( n ) );
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

        /// <summary>
        /// 获取动弹url的参数 0 获取最新的所有人动弹信息，大于 0 时表示为获取某个用户的所有动弹 -1表示获取热门动弹
        /// </summary>
        /// <returns>参数</returns>
        private int GetUID( )
        {
            switch ( this. tweetType )
            {
                case TweetType. Latest:
                    return 0;
                case TweetType. My:
                    return Config. UID;
                case TweetType. Hottest:
                    return -1;
            }
            return 0;
        }
        #endregion

        #region Private functions
        /// <summary>
        /// 进入动弹详情页
        /// </summary>
        private void list_Tweets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TweetUnit t = this. list_Tweets. SelectedItem as TweetUnit;
            this. list_Tweets. SelectedItem = null;
            if ( t != null )
            {
                Tool. ToDetailPage( t. id. ToString( ), DetailType. Tweet );
            }
        }

        /// <summary>
        /// 点击头像进入用户专页
        /// </summary>
        private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TweetUnit t = ( sender as Image ). DataContext as TweetUnit;
            if ( t != null)
            {
                //进入用户专页
                Tool. ToUserPage( t. authorID );
                e. Handled = true;
            }
        }

        /// <summary>
        /// 点击动弹图片  打开大图进行查看
        /// </summary>
        private void imgTweet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TweetUnit t = ( sender as Image ). DataContext as TweetUnit;
            if ( t != null )
            {
                e. Handled = true;
                //查看大图
                this. pop = new PopUpImage( );
                this. pop. Create( t. imgBig );
                this. IsEnabled = false;
                this. pop. pop. Closed += (s, e1) =>
                    {
                        this. IsEnabled = true;
                    };
            }
        }

        /// <summary>
        /// 事件检测当前是否为大图显示
        /// </summary>
        public void BackKeyPress(System. ComponentModel. CancelEventArgs e)
        {
            if ( this.pop != null && this.pop.pop.IsOpen )
            {
                this. pop. pop. IsOpen = false;
                this. IsEnabled = true;
                e. Cancel = true;
            }
        }

        /// <summary>
        /// 删除动弹
        /// </summary>
        private void menu_Delete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            TweetUnit t = item. DataContext as TweetUnit;
            if ( t != null )
            {
                if ( t.authorID != Config.UID )
                {
                    MessageBox. Show( "不能删除别人的动弹", "温馨提示", MessageBoxButton. OK );
                    return;
                }
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    {"uid", Config.UID.ToString()},
                    {"tweetid", t.id.ToString()},
                };
                WebClient client = Tool. SendWebClient( Config.api_tweet_delete, parameters );
                client. DownloadStringCompleted += (s, e1) =>
                {
                    if ( e1. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "删除动弹时网络错误: {0}", e1. Error. Message );
                        return;
                    }
                    else
                    {
                        ApiResult result = Tool. GetApiResult( e1. Result );
                        switch ( result. errorCode )
                        {
                            case 1:
                                this. listBoxHelper. Refresh( );
                                break;
                            case 0:
                            case -1:
                            case -2:
                                MessageBox. Show( result. errorMessage, "温馨提示", MessageBoxButton. OK );
                                break;
                            default:
                                break;
                        }
                    }
                };
            }
        }

        /// <summary>
        /// 如果动弹不包含图片 则删除Image控件节约性能
        /// </summary>
        private void imgTweet_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            TweetUnit t = img. DataContext as TweetUnit;
            if ( t != null && t. imgSmall. IsNotNullOrWhitespace( ) )
            {
            }
            else
            {
                Panel panel = img. Parent as Panel;
                if ( panel != null )
                {
                    panel. Children. Remove( img );
                }
            }
        }

        #endregion
    }
}
