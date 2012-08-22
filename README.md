wp7-app
=======

OSCHINA 的 Windows Phone 7 客户端源码

直接启动 OSChina.sln，编译即可，当然请确保你的SDK是Windows Phone 7.1或以上
如果出现编译错误，请修改 OSChina\obj\Release 目录下的如下文件

LoginPage.g.i.cs
PubCommentPage.g.i.cs
PubMsgPage.g.i.cs
PubPostPage.g.i.cs
PubTweetPage.g.i.cs

将它们的类设置为派生自 WP7_ControlsLib. Controls. ProgressTrayPage
例如 public partial class LoginPage : Microsoft.Phone.Controls.PhoneApplicationPage

本项目采用 GPL 授权协议，欢迎大家在这个基础上进行改进，并与大家分享。
