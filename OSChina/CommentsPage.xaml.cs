using System;
using System. Collections. Generic;
using System. Windows;
using Coding4Fun. Phone. Controls;
using Microsoft. Phone. Controls;
using OSChina. Model;
using OSChina. Model. AppOnly;
using WP7_WebLib. HttpPost;

namespace OSChina
{
    public partial class CommentsPage : PhoneApplicationPage
    {
        #region Properties
        /// <summary>
        /// 表示是否从 PubComment 页面回来
        /// </summary>
        private bool isReturnBackFromPubComment { get; set; }

        #endregion

        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public CommentsPage( )
        {
            InitializeComponent( );
            this. Loaded += (s, e) =>
                {
                    this. commentListControl. listBoxHelper_ReloadDelegate( );
                };
        }

        protected override void OnNavigatedTo(System. Windows. Navigation. NavigationEventArgs e)
        {
            if ( this. NavigationContext. QueryString. ContainsKey( "catalog" ) &&
                  this. NavigationContext. QueryString. ContainsKey( "id" ) )
            {
                this. commentListControl. catalog = this. NavigationContext. QueryString[ "catalog" ]. ToInt32( );
                this. commentListControl. id = this. NavigationContext. QueryString[ "id" ]. ToInt32( );
            }
            if ( this. NavigationContext. QueryString. ContainsKey( "title" ) )
            {
                this. commentListControl. title = this. NavigationContext. QueryString[ "title" ];
            }
            if ( this. NavigationContext. QueryString. ContainsKey( "owneruid" ) )
            {
                this. commentListControl. owneruid = this. NavigationContext. QueryString[ "owneruid" ];
            }
            if ( this. isReturnBackFromPubComment )
            {
                this. commentListControl. listBoxHelper. Refresh( );
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

        /// <summary>
        /// 刷新评论列表
        /// </summary>
        private void icon_Refresh_Click(object sender, EventArgs e)
        {
            this. commentListControl. listBoxHelper. Refresh( );
        }

        /// <summary>
        /// 点击后发表评论
        /// </summary>
        private void icon_Comment2_Click(object sender, EventArgs e)
        {
             if ( this.commentListControl.catalog != (int)CommentType.News && Tool. CheckLogin( ) == false )
            {
                //无法发表评论 并已经提示了登陆
            }
            else
            {
                InputPrompt input = new InputPrompt
                {
                    Title = "发表评论",
                    Message = string.Format("对 {0}", this.commentListControl.title),
                    IsCancelVisible = true,
                    IsSubmitOnEnterKey = false,
                };
                if ( this. commentListControl. catalog == ( int ) CommentType.Tweet)
                {
                    input. Message = string. Empty;
                }
                input. Completed += (s, e1) =>
                {
                    if ( e1.PopUpResult == PopUpResult.Ok )
                    {
                        if ( e1.Result.Length == 0 )
                        {
                            MessageBox. Show( "发表的评论内容不能为空" );
                            return;
                        }
                        Dictionary<string, object> parameters = null;
                        if ( this.commentListControl.catalog != (int)CommentType.Blog )
                        {
                            parameters = new Dictionary<string, object>
                                {
                                    {"catalog", this.commentListControl.catalog},
                                    {"id", this.commentListControl.id},
                                    {"uid", Config.UID},
                                    {"content", e1.Result},
                                    {"isPostToMyZone","1"},
                                };
                        }
                        else
                        {
                            parameters = new Dictionary<string, object>
                                {
                                    {"blog", this.commentListControl.id},
                                    {"uid", Config.UID},
                                    {"content", e1.Result},
                                };
                        }
                        PostClient client = Tool. SendPostClient( this.commentListControl.catalog == (int)CommentType.Blog ? Config. api_blogcomment_pub : Config. api_comment_pub, parameters );
                        client. DownloadStringCompleted += (s1, e2) =>
                        {
                            if ( e2. Error != null )
                            {
                                System. Diagnostics. Debug. WriteLine( "发表评论时网络错误: {0}", e2. Error. Message );
                                return;
                            }
                            ApiResult result = Tool. GetApiResult( e2. Result );
                            if ( result != null )
                            {
                                switch ( result. errorCode )
                                {
                                    case 1:
                                        //进入评论列表页
                                        this.commentListControl.listBoxHelper.Refresh();
                                        break;
                                    case 0:
                                    case -1:
                                    case -2:
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

        #endregion
    }
}