/*
 * 原作者: 王俊
 * 联系qq: 113020930
 */
using System;
using System. Collections. Generic;
using System. Linq;
using System. Net;
using System. Windows;
using System. Windows. Controls;
using Microsoft. Phone. Controls;
using OSChina. Model;

namespace OSChina. Controls
{
    public partial class BlogListControl : UserControl
    {
        #region Properties
        /// <summary>
        /// ListBox辅助对象
        /// </summary>
        private ListBoxHelper listBoxHelper { get; set; }

        /// <summary>
        /// 作者uid 如果博客列表是用户专页的博客列表 则需要使用作者UID
        /// 如果是最新博客或推荐博客 则本属性无意义
        /// </summary>
        public int authoruid { get; set; }

        #endregion

        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public BlogListControl( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
            {
                this. listBoxHelper = new ListBoxHelper( this.list_Blogs ,false, false, false);
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
        /// 刷新博客列表
        /// </summary>
        public void Refresh( )
        {
            //记住现有的 id 集合
            this. listBoxHelper. ids4Refresh = Tool. GetIDS_ID<BlogUnit>( this. listBoxHelper. datas ). Take( 20 ). ToList( );
            this. listBoxHelper. Refresh( );
        }

        /// <summary>
        /// 继续加载博客列表
        /// </summary>
        public void listBoxHelper_ReloadDelegate( )
        {
            if ( this. listBoxHelper. isLoading == true )
            {
                return;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"authoruid", authoruid.ToString()},
                {"pageIndex", (this. listBoxHelper. allCount / 20).ToString()},
                {"pageSize", "20"},
                {"uid", Config.UID.ToString()},
            };
            WebClient client = Tool. SendWebClient( Config. api_userblog_list, parameters );
            this. listBoxHelper. isLoading = true;
            client. DownloadStringCompleted += (s, e) =>
            {
                this. listBoxHelper. isLoading = false;
                if ( e. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取 UserBlogList 网络错误: {0}", e. Error. Message );
                    return;
                }
                int pageSize;
                BlogUnit[ ] newBlogs = Tool. GetUserBlogList( e. Result, out pageSize );
                if ( newBlogs == null )
                {
                    newBlogs = new BlogUnit[ ] { };
                }
                if ( newBlogs != null )
                {
                    this. listBoxHelper. allCount += pageSize;
                    this. listBoxHelper. isLoadOver = pageSize < 20;
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
                    LoadNextTip tip = this. listBoxHelper. GetLoadTip;
                    this. listBoxHelper. datas. Add( tip );
                }

            };
        }
        #endregion

        #region Private functions
        /// <summary>
        /// 点击选择某篇博客 进入博客详情
        /// </summary>
        private void list_Blogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BlogUnit b = this. list_Blogs. SelectedItem as BlogUnit;
            if ( b != null )
            {
                Tool. ProcessAppLink( b. url );
            }
        }

        /// <summary>
        /// 删除博客
        /// </summary>
        private void menu_Delete_Click(object sender, RoutedEventArgs e)
        {
            BlogUnit b = ( sender as MenuItem ). DataContext as BlogUnit;
            if ( b != null )
            {
                if ( b. authorUID != Config. UID )
                {
                    MessageBox. Show( "不能删除别人的博客", "温馨提示", MessageBoxButton. OK );
                    return;
                }
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    {"authoruid", b.authorUID.ToString()},
                    {"uid", Config.UID.ToString()},
                    {"id", b.id.ToString()},
                };
                WebClient client = Tool. SendWebClient( Config. api_userblog_delete, parameters );
                client. DownloadStringCompleted += (s, e1) =>
                {
                    if ( e1. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "删除个人博客时网络错误: {0}", e1. Error. Message );
                        return;
                    }
                    else
                    {
                        ApiResult result = Tool. GetApiResult( e1. Result );
                        switch ( result. errorCode )
                        {
                            case 1:
                                //删除成功 需要重新刷新
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

        #endregion
    }
}
