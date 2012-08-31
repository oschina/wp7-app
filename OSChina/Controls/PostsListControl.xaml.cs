/*
 * 原作者: 王俊
 * 联系邮箱: wangjuntom360@hotmail.com
 */
using System;
using System. Collections. Generic;
using System. Linq;
using System. Text;
using System. Net;
using System. Windows. Controls;
using System. Windows. Input;
using OSChina. Model;
using OSChina. Model. AppOnly;

namespace OSChina. Controls
{
    public partial class PostsListControl : UserControl
    {
        #region Properties
        /// <summary>
        /// ListBox辅助对象
        /// </summary>
        public ListBoxHelper listBoxHelper { get; private set; }

        /// <summary>
        /// 问答类型 0表示获取所有帖子  1 -- 问答 2 -- 分享 3 -- 综合 4 -- 职业 5 -- 站务
        /// </summary>
        public int catalog { get; set; }

        /// <summary>
        /// 问答标签 如果既有catalog 又有 tag 则优先tag
        /// </summary>
        public string tag { get; set; }
        #endregion

        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public PostsListControl( )
        {
            this. catalog = 1;
            InitializeComponent( );
            this. Loaded += (s, e) =>
                {
                    this. listBoxHelper = new ListBoxHelper( this. list_Posts );
                    this. listBoxHelper. ReloadDelegate += new Action( listBoxHelper_ReloadDelegate );
                    this. listBoxHelper_ReloadDelegate( );
                };
         
            this. Unloaded += (s, e) =>
                {
                    this. listBoxHelper. Clear( );
                };
        }

        #endregion

        #region Public functions
        /// <summary>
        /// 刷新列表
        /// </summary>
        public void Refresh( )
        {
            //记住现有的 id 集合
            this. listBoxHelper. ids4Refresh = Tool. GetIDS_ID<PostUnit>( this. listBoxHelper. datas ). Take( 20 ). ToList( );
            this. listBoxHelper. Refresh( );
        }

        #endregion

        #region Private functions
        /// <summary>
        /// 继续加载问答列表
        /// </summary>
        private void listBoxHelper_ReloadDelegate( )
        {
            if ( this. listBoxHelper. isLoading == true )
            {
                return;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"pageIndex", (this. listBoxHelper. allCount / 20).ToString()},
                {"pageSize", "20"},
            };
            if ( this. tag. IsNotNullOrWhitespace( ) )
            {
                parameters. Add( "tag", System. Net. HttpUtility. UrlEncode( tag ) );
            }
            else 
            {
                parameters. Add( "catalog", this. catalog. ToString( ) );
            }
            WebClient client = Tool. SendWebClient( Config.api_post_list, parameters );
            this. listBoxHelper. isLoading = true;
            client. DownloadStringCompleted += (s, e) =>
            {
                this. listBoxHelper. isLoading = false;
                if ( e. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取 PostList 网络错误: {0}", e. Error. Message );
                    return;
                }
                int pageSize;
                PostUnit[ ] newPosts = Tool. GetPostList( e. Result, out pageSize );
                if ( newPosts != null )
                {
                    this. listBoxHelper. allCount += pageSize;
                    this. listBoxHelper. isLoadOver = pageSize < 20;
                    //如果是刷新则提示有数据 注意必须在筛选前就处理好
                    if ( this. listBoxHelper. ids4Refresh. Count > 0 )
                    {
                        int count = newPosts. Where( p => this. listBoxHelper. ids4Refresh. Contains( p. id ) == false ). Count( );
                        if ( count > 0 )
                        {
                            EventSingleton. Instance. ToastMessage( null, string. Format( "{0} 条新帖子", count ) );
                        }
                        this. listBoxHelper. ids4Refresh. Clear( );
                    }
                    //筛选
                    int[ ] ids = Tool. GetIDS_ID<PostUnit>( this. listBoxHelper. datas );
                    newPosts = newPosts. Where( p => ids. Contains( p. id ) == false ). ToArray( );

                    if ( newPosts. Length > 0 )
                    {
                        if ( this. listBoxHelper. datas. Count > 0 )
                        {
                            this. listBoxHelper. datas[ this. listBoxHelper. datas. Count - 1 ] = newPosts[ 0 ];
                            for ( int i = 1 ; i < newPosts. Length ; i++ )
                            {
                                this. listBoxHelper. datas. Add( newPosts[ i ] );
                            }
                        }
                        else
                        {
                            newPosts. ForAll( n => this. listBoxHelper. datas. Add( n ) );
                        }
                    }

                    //添加最后的提示文字
                    if ( this. listBoxHelper. isLoadOver == false )
                    {
                        this. listBoxHelper. datas. Add( this. listBoxHelper. GetLoadTip );
                    }
                }
            };
        }

        /// <summary>
        /// 点击进入查看问答详情
        /// </summary>
        private void list_Posts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PostUnit p = this. list_Posts. SelectedItem as PostUnit;
            this. list_Posts. SelectedItem = null;
            if ( p != null )
            {
                Tool. ToDetailPage( p. id. ToString( ), DetailType. Post );
            }
        }

        /// <summary>
        /// 用户头像被点击
        /// </summary>
        private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PostUnit p = (sender as Image). DataContext as PostUnit;
            if ( p != null )
            {
                //进入用户专页
                Tool. ToUserPage( p. authorID );
                e. Handled = true;
            }
        }
        #endregion
    }
}
