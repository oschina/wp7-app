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
using System. Windows. Documents;
using System. Windows. Input;
using System. Windows. Media;
using Coding4Fun. Phone. Controls;
using Microsoft. Phone. Controls;
using OSChina. Model;
using OSChina. Model. AppOnly;
using WP7_WebLib. HttpPost;

namespace OSChina. Controls
{
    public partial class CommentListControl : UserControl
    {
        #region Properties
        /// <summary>
        /// 评论所属文章类别
        /// </summary>
        public int catalog { get; set; }

        /// <summary>
        /// 评论所属文章ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 评论所属文章标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 文章作者UID
        /// </summary>
        public string owneruid { get; set; }

        /// <summary>
        /// ListBox辅助对象
        /// </summary>
        public ListBoxHelper listBoxHelper { get; private set; }
        #endregion

        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public CommentListControl( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
            {
                this. listBoxHelper = new ListBoxHelper( this. list_Comment, false, true );
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
        /// 继续加载评论列表
        /// </summary>
        public void listBoxHelper_ReloadDelegate( )
        {
            if ( this. listBoxHelper == null )
            {
                return;
            }
            if ( this. listBoxHelper. isLoading == true )
            {
                return;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"pageIndex",(this.listBoxHelper.allCount/20).ToString()},
                {"pageSize", "20"},
                {"id", this. id. ToString( )},
                {"guid", Guid.NewGuid().ToString()},
            };
            if ( catalog != 5 )
            {
                parameters. Add( "catalog", this. catalog. ToString( ) );
            }
            WebClient client = Tool. SendWebClient( catalog != 5 ? Config. api_comment_list : Config. api_blogcomment_list, parameters );
            this. listBoxHelper. isLoading = true;
            client. DownloadStringCompleted += (s, e) =>
            {
                this. listBoxHelper. isLoading = false;
                if ( e. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取 {0} 网络错误: {1}", catalog == 5 ? "BlogCommentList" : "CommentList", e. Error. Message );
                    return;
                }
                int pageSize;
                CommentUnit[ ] newComments = Tool. GetCommentList( e. Result, out pageSize );
                if ( newComments != null )
                {
                    this. listBoxHelper. allCount += this. catalog != 5 ? pageSize : newComments. Length;
                    this. listBoxHelper. isLoadOver = this. catalog != 5 ? pageSize < 20 : newComments. Length < 20;
                    //筛选
                    int[ ] ids = Tool. GetIDS_ID<CommentUnit>( this. listBoxHelper. datas );
                    newComments = newComments. Where( c => ids. Contains( c. id ) == false ). ToArray( );

                    if ( newComments. Length > 0 )
                    {
                        if ( this. listBoxHelper. datas. Count > 0 )
                        {
                            this. listBoxHelper. datas[ this. listBoxHelper. datas. Count - 1 ] = newComments[ 0 ];
                            for ( int i = 1 ; i < newComments. Length ; i++ )
                            {
                                this. listBoxHelper. datas. Add( newComments[ i ] );
                            }
                        }
                        else
                        {
                            newComments. ForAll( n => this. listBoxHelper. datas. Add( n ) );
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
                        LoadNextTip tip = this. listBoxHelper. GetLoadTip;
                        tip. Foreground = new SolidColorBrush( Colors. Black );
                        this. listBoxHelper. datas. Add( tip );
                    }
                }
            };
        }

        #endregion

        #region Private functions
        /// <summary>
        /// 头像点击 进入个人专页
        /// </summary>
        private void img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CommentUnit c = ( sender as Image ). DataContext as CommentUnit;
            if ( c != null )
            {
                Tool. ToUserPage( c. authorID );
                e. Handled = true;
            }
        }

        /// <summary>
        /// 评论点击 回复该评论
        /// </summary>
        private void list_Comment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //登陆判断
            if ( this. catalog != ( int ) CommentType. News && Tool. CheckLogin( ) == false )
            {
                //无法发表评论
            }
            else
            {
                CommentUnit c = this. list_Comment. SelectedItem as CommentUnit;
                this. list_Comment. SelectedItem = null;
                if ( c != null )
                {
                    InputPrompt input = new InputPrompt
                    {
                        Title = "回复这条评论",
                        Message = c. content,
                        IsCancelVisible = true,
                        IsSubmitOnEnterKey = false,
                        Value = Config. GetCache_CommentReply( this. id, c. id ),
                    };
                    input. OnValueChanged += (s, e1) =>
                    {
                        Config. SaveCache_CommentReply( e1. Text, this. id, c. id );
                    };
                    input. Completed += (s, e1) =>
                    {
                        if ( e1. PopUpResult == PopUpResult. Ok )
                        {
                            Dictionary<string, object> parameters = null;
                            if ( this. catalog != ( int ) CommentType. Blog )
                            {
                                parameters = new Dictionary<string, object>
                            {
                                {"catalog", this.catalog},
                                {"id", this.id},
                                {"replyid",c.id},
                                {"authorid", c.authorID},
                                {"uid",Config.UID},
                                {"content", e1.Result},
                            };
                            }
                            else
                            {
                                parameters = new Dictionary<string, object>
                                    {
                                        {"blog", this.id},
                                        {"uid",Config.UID},
                                        {"content",e1.Result},
                                        {"reply_id",c.id},
                                        {"objuid",c.authorID},
                                    };
                            }
                            PostClient client = Tool. SendPostClient( this. catalog == ( int ) CommentType. Blog ? Config. api_blogcomment_pub : Config. api_comment_reply, parameters );
                            client. DownloadStringCompleted += (s1, e2) =>
                            {
                                if ( e2. Error != null )
                                {
                                    System. Diagnostics. Debug. WriteLine( "回复评论时网络错误: {0}", e2. Error. Message );
                                    return;
                                }
                                ApiResult result = Tool. GetApiResult( e2. Result );
                                if ( result != null )
                                {
                                    switch ( result. errorCode )
                                    {
                                        case 1:
                                            Config. SaveCache_CommentReply( null, this. id, c. id );
                                            this. listBoxHelper. Refresh( );
                                            break;
                                        case -1:
                                        case -2:
                                        case 0:
                                            MessageBox. Show( result. errorMessage, "温馨提示", MessageBoxButton. OK );
                                            break;
                                    }
                                }
                            };
                        }
                    };
                    input. Show( );
                }
            }
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        private void menu_Delete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            CommentUnit c = item. DataContext as CommentUnit;
            if ( c != null )
            {
                //因为博客的评论删除只有文章作者才有此功能
                if ( this. catalog != ( int ) CommentType. Blog )
                {
                    if ( c. authorID != Config. UID )
                    {
                        MessageBox. Show( "不能删除别人的评论", "温馨提示", MessageBoxButton. OK );
                        return;
                    }
                }

                Dictionary<string, string> parameters = null;
                if ( this. catalog != ( int ) CommentType. Blog )
                {
                    parameters = new Dictionary<string, string>
                    {
                        {"catalog",this.catalog.ToString()},
                        {"id",this.id.ToString()},
                        {"replyid",c.id.ToString()},
                        {"authorid", c.authorID.ToString()},
                    };
                }
                else
                {
                    parameters = new Dictionary<string, string>
                    {
                        {"blogid", this.id.ToString()},
                        {"uid", Config.UID.ToString()},
                        {"replyid", c.id.ToString()},
                        {"authorid", c.authorID.ToString()},
                        {"owneruid", this.owneruid},
                    };
                }
                WebClient client = Tool. SendWebClient( this. catalog != ( int ) CommentType. Blog ? Config. api_comment_delete : Config. api_blogcomment_delete, parameters );
                client. DownloadStringCompleted += (s, e1) =>
                {
                    if ( e1. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "删除评论时网络错误: {0}", e1. Error. Message );
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
        /// 添加控件 添加评论的引用与评论的回复 这里可能造成列表上下晃动
        /// </summary>
        private void panel_Loaded(object sender, RoutedEventArgs e)
        {
            StackPanel s = sender as StackPanel;
            CommentUnit c = s. DataContext as CommentUnit;
            if ( c != null )
            {
                //添加 评论的引用
                if ( s. Name == "panel_refer" )
                {
                    if ( c. refers. IsNotNullOrEmpty( ) )
                    {
                        s. Visibility = System. Windows. Visibility. Visible;
                        s. Margin = new Thickness( 0, 0, 0, 7 );
                        s. Background = new SolidColorBrush( Color. FromArgb( 215, 175, 197, 255 ) );
                        s. Children. Clear( );
                        c. refers. ForAll( _c =>
                        {
                            StackPanel newS = new StackPanel { Background = new SolidColorBrush( Color. FromArgb( 255, 239, 239, 179 ) ), Margin = new Thickness( 5, 5, 5, 3 ) };
                            newS. Children. Add( new TextBlock { Text = _c. title, Foreground = new SolidColorBrush( Color. FromArgb( 255, 40, 40, 40 ) ), FontSize = 19, Margin = new Thickness( 3, 2, 0, 2 ), TextWrapping = TextWrapping. Wrap } );
                            s. Children. Add( newS );
                            s. Children. Add( new TextBlock { Text = _c. body, Foreground = new SolidColorBrush( Color. FromArgb( 255, 40, 40, 40 ) ), FontSize = 20, Margin = new Thickness( 7, 0, 0, 0 ), TextWrapping = TextWrapping. Wrap } );
                        } );
                        ( s. Children. LastOrDefault( ) as TextBlock ). Margin = new Thickness( 7, 0, 0, 5 );
                    }
                    else
                    {
                        ( s. Parent as StackPanel ). Children. Remove( s );
                    }
                }
                //添加评论的回复
                else if ( s. Name == "panel_repiles" )
                {
                    if ( c. replies. IsNotNullOrEmpty( ) )
                    {
                        s. Visibility = System. Windows. Visibility. Visible;
                        s. Background = new SolidColorBrush( Color. FromArgb( 215, 239, 239, 179 ) );
                        s. Margin = new Thickness( 0, 0, 0, 7 );
                        s. Children. Clear( );
                        s. Children. Add( new TextBlock { Text = string. Format( "--- 共有 {0} 条评论 ---", c. replies. Length ), Foreground = new SolidColorBrush( Colors. Black ), FontWeight = FontWeights. ExtraBold, FontSize = 21, TextWrapping = TextWrapping. Wrap, Margin = new Thickness( 14, 10, 0, 2 ) } );
                        c. replies. ForAll( r =>
                        {
                            TextBlock t = new TextBlock
                            {
                                TextWrapping = TextWrapping. Wrap,
                                FontSize = 20,
                                Margin = new Thickness( 12, 6, 0, 5 )
                            };
                            t. Inlines. Add( new Run { Text = r. author, FontWeight = FontWeights. Bold, Foreground = new SolidColorBrush( Colors. Black ) } );
                            t. Inlines. Add( new Run { Text = string. Format( "({0}): {1}", Tool. IntervalSinceNow( r. pubDate ), r. content ), Foreground = new SolidColorBrush( Color. FromArgb( 255, 40, 40, 40 ) ) } );
                            s. Children. Add( t );
                        } );
                        ( s. Children. LastOrDefault( ) as TextBlock ). Margin = new Thickness( 12, 6, 0, 10 );
                    }
                    else
                    {
                        ( s. Parent as StackPanel ). Children. Remove( s );
                    }
                }

                this. list_Comment. UpdateLayout( );
            }
        }
        #endregion
    }
}
