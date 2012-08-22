wp7-app
=======
#开源中国社区 Windows Phone 客户端项目简析

直接启动 OSChina.sln，编译即可，当然请确保你的SDK是Windows Phone 7.1或以上
如果出现编译错误，请修改 OSChina\obj\Release 目录下的如下文件

>LoginPage.g.i.cs
>PubCommentPage.g.i.cs
>PubMsgPage.g.i.cs
>PubPostPage.g.i.cs
>PubTweetPage.g.i.cs

将它们的类设置为派生自 WP7_ControlsLib. Controls. ProgressTrayPage
例如 public partial class LoginPage : Microsoft.Phone.Controls.PhoneApplicationPage

####本项目采用 GPL 授权协议，欢迎大家在这个基础上进行改进，并与大家分享。

App/Config.cs
>包含应用的配置信息

Control/ActiveListControl.xaml
>动态列表控件

Control/BlogListControl.xaml
>博客列表控件

Control/CommentListControl.xaml
>评论列表控件

Control/FaceControl.xaml
>表情控件

Control/FavListControl.xaml
>收藏列表控件

Control/FriendListControl.xaml
>粉丝列表控件

Control/LoadNextTip.xaml
>列表加载更多项控件

Control/MsgListControl.xaml
>留言列表控件

Control/NewsListControl.xaml
>新闻列表控件

Control/PopUpImage.xaml
>查看动弹大图控件

Control/PostsListControl.xaml
>问答列表控件

Control/TweetListControl.xaml
>动弹列表控件

Control/TweetPortal.xaml
>用在首页的动弹页入口控件

Helper/EventSingleton.cs
>负责在整个程序中消息通知

Helper/Tool.cs
>项目辅助类

####Model 文件夹的类代表网络传输的实体对象

ActivesPage.xaml
>动态页

App.xaml
>应用程序入口页

DetailPage2.xaml
>新闻,问答,动弹,博客,软件详细内容页

FavPage.xaml
>收藏页

FeedbackPage.xaml
>关于我们页

LoginPage.xaml
>登录页

MainPage.xaml
>应用程序首页

MsgsPage.xaml
>留言列表页

PostsPage.xaml
>问答列表页

PubPostPage.xaml
>发表问答页

PubTweetPage.xaml
>发表动弹页

SearchPage.xaml
>搜索页

SettingPage.xaml
>应用设置页

TweetsPage.xaml
>动弹列表页

UserPage.xaml
>用户个人专页

WordsPage.xaml
>与某用户的会话页

####项目 OSChinaScheduledTask_Notice 
>为应用不启动时的后台线程，可以轮询获取用户的最新通知数