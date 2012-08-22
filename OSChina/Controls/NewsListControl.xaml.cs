using System;
using System. Collections. Generic;
using System. Linq;
using System. Net;
using System. Windows. Controls;
using System. Windows. Media;
using OSChina. Model;
using OSChina. Model. AppOnly;

namespace OSChina. Controls
{
    public partial class NewsListControl : UserControl
    {
        #region Properties
        /// <summary>
        /// 新闻类型
        /// </summary>
        private NewsType newsType;
        public NewsType NewsType
        {
            get { return newsType; }
            set 
            {
                if ( value != newsType )
                {
                    newsType = value;
                    //开始刷新
                    this. Refresh( );
                }
            }
        }

        /// <summary>
        /// ListBox辅助对象
        /// </summary>
        private ListBoxHelper listBoxHelper { get; set; }
        #endregion

        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public NewsListControl( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
            {

            };
            this. listBoxHelper = new ListBoxHelper( this. list_News, true );
            this. listBoxHelper. ReloadDelegate += new Action( listBoxHelper_ReloadDelegate );
            this. listBoxHelper_ReloadDelegate( );
        }
        #endregion

        #region Public functions
        /// <summary>
        /// 刷新新闻
        /// </summary>
        public void Refresh( )
        {
            //记住现有的 id 集合
            this. listBoxHelper. ids4Refresh = Tool. GetIDS_ID<NewsUnit>( this. listBoxHelper. datas ). Take( 20 ). ToList( );
            this. listBoxHelper. Refresh( );
        }

        #endregion

        #region Private functions
        /// <summary>
        /// 继续加载新闻列表
        /// </summary>
        private void listBoxHelper_ReloadDelegate()
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
            switch ( this.newsType )
            {
                case NewsType. News:
                    parameters. Add( "catalog", "1" );
                    break;
                case NewsType. Blogs:
                    parameters. Add( "type", "latest" );
                    break;
                case NewsType. RecommendBlogs:
                    parameters. Add( "type", "recommend" );
                    break;
            }
            WebClient client = Tool. SendWebClient( NewsType == Controls. NewsType. News ? Config. api_news_list : Config. api_blog_list, parameters );
            this. listBoxHelper. isLoading = true;
            client. DownloadStringCompleted += (s, e) =>
            {
                this. listBoxHelper. isLoading = false;
                if ( e. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取 NewsList 网络错误: {0}", e. Error. Message );
                    return;
                }
                int pageSize;
                #region 获取新闻
                if ( this. NewsType == Controls. NewsType. News )
                {
                    NewsUnit[ ] newNews = Tool. GetNewsList( e. Result, out pageSize );
                    if ( newNews != null )
                    {
                        this. listBoxHelper. allCount += pageSize;
                        this. listBoxHelper. isLoadOver = pageSize < 20;
                        //如果是刷新则提示有数据 注意必须在筛选前就处理好
                        //筛选
                        int[ ] ids = Tool. GetIDS_ID<NewsUnit>( this. listBoxHelper. datas );
                        newNews = newNews. Where( n => ids. Contains( n. id ) == false ). ToArray( );
                        if ( newNews. Length > 0 )
                        {
                            if ( this. listBoxHelper. datas. Count > 0 )
                            {
                                this. listBoxHelper. datas[ this. listBoxHelper. datas. Count - 1 ] = newNews[ 0 ];
                                for ( int i = 1 ; i < newNews. Length ; i++ )
                                {
                                    this. listBoxHelper. datas. Add( newNews[ i ] );
                                }
                            }
                            else
                            {
                                newNews. ForAll( n => this. listBoxHelper. datas. Add( n ) );
                            }
                        }
                        //添加最后的提示文字
                        if ( this. listBoxHelper. isLoadOver == false )
                        {
                            LoadNextTip tip = this. listBoxHelper. GetLoadTip;
                            tip. Foreground = new SolidColorBrush( Colors. White );
                            this. listBoxHelper. datas. Add( tip );
                        }
                    }
                }
                #endregion
                #region 博客
                else
                {
                    BlogUnit[ ] newBlogs = Tool. GetUserBlogList( e. Result, out pageSize );
                    if ( newBlogs != null )
                    {
                        this. listBoxHelper. allCount += pageSize;
                        this. listBoxHelper. isLoadOver = pageSize < 20;
                        //如果是刷新则提示有数据 注意必须在筛选前就处理好
                        //筛选
                        int[ ] ids = Tool. GetIDS_ID<BlogUnit>( this. listBoxHelper. datas );
                        newBlogs = newBlogs. Where( n => ids. Contains( n. id ) == false ). ToArray( );
                        if ( newBlogs. Length > 0 )
                        {
                            if ( this. listBoxHelper. datas. Count > 0 )
                            {
                                this. listBoxHelper. datas[ this. listBoxHelper. datas. Count - 1 ] = newBlogs[ 0 ];
                                for ( int i = 1 ; i < newBlogs. Length ; i++ )
                                {
                                    this. listBoxHelper. datas. Add( newBlogs[ i ] );
                                }
                            }
                            else
                            {
                                newBlogs. ForAll( n => this. listBoxHelper. datas. Add( n ) );
                            }
                        }
                        //添加最后的提示文字
                        if ( this. listBoxHelper. isLoadOver == false )
                        {
                            LoadNextTip tip = this. listBoxHelper. GetLoadTip;
                            tip. Foreground = new SolidColorBrush( Colors. White );
                            this. listBoxHelper. datas. Add( tip );
                        }
                    }
                }
                #endregion
            };
        }

        /// <summary>
        /// 点击查看新闻详情
        /// </summary>
        private void list_News_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ( this. NewsType == Controls. NewsType. News )
            {
                NewsUnit n = this. list_News. SelectedItem as NewsUnit;
                this. list_News. SelectedItem = null;
                if ( n != null )
                {
                    if ( n. url. IsNullOrWhitespace( ) )
                    {
                        DetailType type = DetailType. News;
                        //type:0 新闻 1 软件 2 帖子 3 博客，
                        switch ( n. newsType. type )
                        {
                            case 1:
                                type = DetailType. Software;
                                break;
                            case 2:
                                type = DetailType. Post;
                                break;
                            case 3:
                                type = DetailType. Blog;
                                break;
                        }
                        Tool. ToDetailPage( n. newsType. attachment, type );
                    }
                    else
                    {
                        Tool. ProcessAppLink( n. url );
                    }
                }
            }
            else
            {
                BlogUnit b = this. list_News. SelectedItem as BlogUnit;
                this. list_News. SelectedItem = null;
                if ( b != null )
                {
                    Tool. ProcessAppLink( b. url );
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 新闻类型
    /// </summary>
    public enum NewsType
    {
        /// <summary>
        /// 资讯
        /// </summary>
        News,
        /// <summary>
        /// 博客
        /// </summary>
        Blogs,
        /// <summary>
        /// 推荐阅读
        /// </summary>
        RecommendBlogs,
    }
}
