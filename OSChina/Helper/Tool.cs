/*
 * 原作者: 王俊
 * 联系qq: 113020930
 */
using System;
using System. Collections. Generic;
using System. Collections. ObjectModel;
using System. Diagnostics;
using System. IO;
using System. Linq;
using System. Net;
using System. Text;
using System. Text. RegularExpressions;
using System. Threading;
using System. Windows;
using System. Windows. Controls;
using System. Windows. Documents;
using System. Windows. Media;
using System. Windows. Media. Imaging;
using System. Xml. Linq;
using HtmlAgilityPack;
using Microsoft. Phone. Controls;
using Microsoft. Phone. Net. NetworkInformation;
using Microsoft. Phone. Shell;
using Microsoft. Phone. Tasks;
using OSChina. Model;
using OSChina. Model. AppOnly;
using WP7_ControlsLib. Controls;
using WP7_WebLib. HttpGet;
using WP7_WebLib. HttpPost;

namespace OSChina
{
    public static class Tool
    {
        #region XML 解析
        public static NewsUnit[ ] GetNewsList( string response, out int pageSize )
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                pageSize = root. Element( "pagesize" ). Value. ToInt32( );
                XElement news = root. Element( "newslist" );
                if ( news == null || news. HasElements == false )
                {
                    return null;
                }
                else
                {
                    NewsUnit[ ] units = news. Elements( "news" ). Select(
                            n => new NewsUnit
                            {
                                id = n. Element( "id" ). Value. ToInt32( ),
                                title = n. Element( "title" ). Value,
                                author = n. Element( "author" ). Value,
                                authorID = n. Element( "authorid" ). Value. ToInt32( ),
                                pubDate = n. Element( "pubDate" ). Value,
                                commentCount = n. Element( "commentCount" ). Value. ToInt32( ),
                                url = n.Element("url") == null ? null : n.Element("url").Value,
                                newsType = new NewsType
                                {
                                    type = n. Element( "newstype" ). Element( "type" ). Value. ToInt32( ),
                                    attachment = n.Element("newstype").Element("attachment") == null ? 
                                                                                    n. Element( "id" ). Value : 
                                                                                    n. Element("newstype"). Element("attachment").Value,
                                    authorID = n. Element( "newstype" ). Element( "authoruid2" ). Value. ToInt32( ),
                                },
                            }
                        ). ToArray( );
                    return units;
                }
            }
            catch ( Exception e)
            {
                pageSize = 0;
                System. Diagnostics. Debug. WriteLine( "NewsList解析错误: {0}", e. Message );
                return null;
            }
        }

        public static NewsDetail GetNewsDetail(string response)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                XElement news = root. Element( "news" );
                return new NewsDetail
                {
                    id = news. Element( "id" ). Value. ToInt32( ),
                    title = news. Element( "title" ). Value,
                    url = news. Element( "url" ). Value,
                    body = news. Element( "body" ). Value,
                    author = news. Element( "author" ). Value,
                    authorID = news. Element( "authorid" ). Value. ToInt32( ),
                    commentCount = news. Element( "commentCount" ). Value. ToInt32( ),
                    pubDate = news. Element( "pubDate" ). Value,
                    softwareLink = news. Element( "softwarelink" ). Value,
                    softwareName = news. Element( "softwarename" ). Value,
                    favorite = news. Element( "favorite" ). Value == "1",
                    relativies = news. Element( "relativies" ) == null ? null :
                    GetRelativeNews( news. Element( "relativies" ) ),
                };
            }
            catch ( Exception e)
            {
                System. Diagnostics. Debug. WriteLine( "NewsDetail 解析错误: {0}", e. Message );
                return null;
            }
        }
        private static RelativeNews[ ] GetRelativeNews(XElement relativies)
        {
            try
            {
                RelativeNews[ ] result = relativies. Elements( "relative" ). Select( r => new RelativeNews
                {
                    url = r. Element( "rurl" ). Value,
                    title = r. Element( "rtitle" ). Value,
                } ). ToArray( );
                return result;
            }
            catch ( Exception e)
            {
                System. Diagnostics. Debug. WriteLine( "RelativeNews解析错误: {0}", e. Message );
                return null;
            }
        }

        public static SoftwareDetail GetSoftwareDetail(string response)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                XElement soft = root. Element( "software" );
                return new SoftwareDetail
                {
                    id = soft. Element( "id" ). Value. ToInt32( ),
                    title = soft. Element( "title" ). Value,
                    url = soft. Element( "url" ). Value,
                    extensionTitle = soft. Element( "extensionTitle" ). Value,
                    license = soft. Element( "license" ). Value,
                    body = soft. Element( "body" ). Value,
                    homePage = soft. Element( "homepage" ). Value,
                    document = soft. Element( "document" ). Value,
                    download = soft. Element( "download" ). Value,
                    logo = soft. Element( "logo" ). Value,
                    language = soft. Element( "language" ). Value,
                    os = soft. Element( "os" ). Value,
                    recordTime = soft. Element( "recordtime" ). Value,
                    favorite = soft. Element( "favorite" ). Value == "1",
                };
            }
            catch ( Exception e)
            {
                System. Diagnostics. Debug. WriteLine( "SoftwareDetail 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static BlogDetail GetBlogDetail(string response)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement blog = XElement. Parse( response ). Element( "blog" );
                return new BlogDetail
                {
                    id = blog. Element( "id" ). Value. ToInt32( ),
                    title = blog. Element( "title" ). Value,
                    url = blog. Element( "url" ). Value,
                    where = blog. Element( "where" ). Value,
                    commentCount = blog. Element( "commentCount" ). Value. ToInt32( ),
                    body = blog. Element( "body" ). Value,
                    author = blog. Element( "author" ). Value,
                    authorID = blog. Element( "authorid" ). Value. ToInt32( ),
                    documentType = blog. Element( "documentType" ). Value. ToInt32( ),
                    pubDate = blog. Element( "pubDate" ). Value,
                    favorite = blog. Element( "favorite" ). Value == "1",
                };
            }
            catch ( Exception e)
            {
                System. Diagnostics. Debug. WriteLine( "BlogDetail 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static PostUnit[ ] GetPostList(string response, out int pageSize)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                pageSize = root. Element( "pagesize" ). Value. ToInt32( );
                XElement posts = root. Element( "posts" );
                if ( posts == null || posts. HasElements == false )
                {
                    return null;
                }
                PostUnit[ ] result = posts. Elements( "post" ). Select(
                        p => new PostUnit
                        {
                            id = p. Element( "id" ). Value. ToInt32( ),
                            portrait = p. Element( "portrait" ). Value,
                            author = p. Element( "author" ). Value,
                            authorID = p. Element( "authorid" ). Value. ToInt32( ),
                            title = p. Element( "title" ). Value,
                            answerCount = p. Element( "answerCount" ). Value. ToInt32( ),
                            viewCount = p. Element( "viewCount" ). Value. ToInt32( ),
                            pubDate = p. Element( "pubDate" ). Value,
                            answer = p. Element( "answer" ) == null ? null :
                             new AnswerUnit
                             {
                                 name = p. Element( "answer" ). Element( "name" ). Value,
                                 time = p. Element( "answer" ). Element( "time" ). Value,
                             },
                        }
                ). ToArray( );
                return result;
            }
            catch ( Exception e )
            {
                pageSize = 0;
                System. Diagnostics. Debug. WriteLine( "PostList 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static PostDetail GetPostDetail(string response)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement post = XElement. Parse( response ). Element( "post" );
                return new PostDetail
                {
                    id = post. Element( "id" ). Value. ToInt32( ),
                    title = post. Element( "title" ). Value,
                    url = post. Element( "url" ). Value,
                    portrait = post. Element( "portrait" ). Value,
                    body = post. Element( "body" ). Value,
                    author = post. Element( "author" ). Value,
                    authorID = post. Element( "authorid" ). Value. ToInt32( ),
                    answerCount = post. Element( "answerCount" ). Value. ToInt32( ),
                    viewCount = post. Element( "viewCount" ). Value. ToInt32( ),
                    pubDate = post. Element( "pubDate" ). Value,
                    favorite = post. Element( "favorite" ). Value == "1",
                    tags = post. Element( "tags" ) == null ? null : post. Element( "tags" ). Elements( "tag" ). Select( t => t. Value ). ToArray( ),
                };
            }
            catch ( Exception e)
            {
                System. Diagnostics. Debug. WriteLine( "PostDetail 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static TweetUnit[ ] GetTweetList(string response, out int pageSize, out int tweetCount)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                pageSize = root. Element( "pagesize" ). Value. ToInt32( );
                tweetCount = root. Element( "tweetCount" ). Value. ToInt32( );
                XElement tweets = root. Element( "tweets" );
                if ( tweets == null || tweets. HasElements == false )
                {
                    return null;
                }
                TweetUnit[ ] result = tweets. Elements( "tweet" ). Select(
                        t => new TweetUnit
                        {
                            id = t. Element( "id" ). Value. ToInt32( ),
                            portrait = t. Element( "portrait" ). Value,
                            author = t. Element( "author" ). Value,
                            authorID = t. Element( "authorid" ). Value. ToInt32( ),
                            body = t. Element( "body" ). Value,
                            commentCount = t. Element( "commentCount" ). Value. ToInt32( ),
                            appClient = t. Element( "appclient" ). Value. ToInt32( ),
                            pubDate = t. Element( "pubDate" ). Value,
                            imgSmall = t. Element( "imgSmall" ). Value,
                            imgBig = t. Element( "imgBig" ). Value,
                        }
                    ). ToArray( );
                return result;
            }
            catch ( Exception e)
            {
                pageSize = tweetCount = 0;
                System. Diagnostics. Debug. WriteLine( "TweetList 解析错误: {0}", e. Message );
                return null;
            }
            
        }

        public static TweetDetail GetTweetDetail(string response)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement t = XElement. Parse( response ). Element( "tweet" );
                return new TweetDetail
                {
                    id = t. Element( "id" ). Value. ToInt32( ),
                    body = t. Element( "body" ). Value,
                    author = t. Element( "author" ). Value,
                    authorID = t. Element( "authorid" ). Value. ToInt32( ),
                    commentCount = t. Element( "commentCount" ). Value. ToInt32( ),
                    portrait = t. Element( "portrait" ). Value,
                    pubDate = t. Element( "pubDate" ). Value,
                    imgSmall = t. Element( "imgSmall" ). Value,
                    imBig = t. Element( "imgBig" ). Value,
                    appClient = ( AppClientType ) t. Element( "appclient" ). Value. ToInt32( ),
                };
            }
            catch ( Exception e)
            {
                System. Diagnostics. Debug. WriteLine( "TweetDetail 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static ActiveUnit[ ] GetActiveList(string response, out int pageSize)
        {

            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                pageSize = root. Element( "pagesize" ). Value. ToInt32( );
                XElement activies = root. Element( "activies" );
                if ( activies == null || activies. HasElements == false )
                {
                    return null;
                }
                else
                {
                    ActiveUnit[ ] result = activies. Elements( "active" ). Select(
                        a => new ActiveUnit
                        {
                            id = a. Element( "id" ). Value. ToInt32( ),
                            portrait = a. Element( "portrait" ). Value,
                            author = a. Element( "author" ). Value,
                            authorID = a. Element( "authorid" ). Value. ToInt32( ),
                            catalog = a. Element( "catalog" ). Value. ToInt32( ),
                            objID = a. Element( "objectID" ). Value. ToInt32( ),
                            objType = a. Element( "objecttype" ). Value. ToInt32( ),
                            objCatalog = a. Element( "objectcatalog" ). Value. ToInt32( ),
                            objTitle = a. Element( "objecttitle" ). Value,
                            objReplyName = a. Element( "objectreply" ) != null ? a. Element( "objectreply" ). Element( "objectname" ). Value : null,
                            objReplyBody = a. Element( "objectreply" ) != null ? a. Element( "objectreply" ). Element( "objectbody" ). Value : null,
                            appClient = a. Element( "appclient" ) == null ? 1 : a. Element( "appclient" ). Value. ToInt32( ),
                            message = a. Element( "message" ). Value,
                            commentCount = a. Element( "commentCount" ). Value. ToInt32( ),
                            pubDate = a. Element( "pubDate" ). Value,
                            tweetImage = a. Element( "tweetimage" ). Value,
                            url = a. Element( "url" ) == null ? null : a. Element( "url" ). Value,
                        } ). ToArray( );
                    return result;
                }
            }
            catch ( Exception e)
            {
                pageSize = 0;
                System. Diagnostics. Debug. WriteLine( "ActiveList 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static MsgUnit[ ] GetMsgList(string response, out int pageSize)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                pageSize = root. Element( "pagesize" ). Value. ToInt32( );
                XElement msgs = root. Element( "messages" );
                if ( msgs == null || msgs. HasElements == false )
                {
                    return null;
                }
                else
                {
                    MsgUnit[ ] result = msgs. Elements( "message" ). Select(
                           m => new MsgUnit
                           {
                               id = m. Element( "id" ). Value. ToInt32( ),
                               portrait = m. Element( "portrait" ). Value,
                               friendID = m. Element( "friendid" ). Value. ToInt32( ),
                               friendName = m. Element( "friendname" ). Value,
                               sender = m. Element( "sender" ). Value,
                               senderID = m. Element( "senderid" ). Value. ToInt32( ),
                               content = m. Element( "content" ). Value,
                               messageCount = m. Element( "messageCount" ). Value. ToInt32( ),
                               pubDate = m. Element( "pubDate" ). Value,
                           } ). ToArray( );
                    return result;
                }
            }
            catch ( Exception e )
            {
                pageSize = 0;
                System. Diagnostics. Debug. WriteLine( "MsgList 解析错误: {0}", e. Message );
                return null;
            }
            
        }

        public static ApiResult GetApiResult(string response)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement result = XElement. Parse( response ). Element( "result" );
                return new ApiResult
                {
                    errorCode = result. Element( "errorCode" ). Value. ToInt32( ),
                    errorMessage = result. Element( "errorMessage" ). Value,
                };
            }
            catch ( Exception e)
            {
                System. Diagnostics. Debug. WriteLine( "ApiResult 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static CommentUnit[ ] GetCommentList(string response, out int pageSize)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                pageSize = root.Element("pagesize") == null ? 0 : root. Element( "pagesize" ). Value. ToInt32( );
                XElement comments = root. Element( "comments" );
                if ( comments == null  )
                {
                    return null;
                }
                else
                {
                    CommentUnit[ ] result = comments. Elements( "comment" ). Select(
                            c => new CommentUnit
                            {
                                id = c. Element( "id" ). Value. ToInt32( ),
                                portrait = c. Element( "portrait" ). Value,
                                author = c. Element( "author" ). Value,
                                authorID = c. Element( "authorid" ). Value. ToInt32( ),
                                content = c. Element( "content" ). Value,
                                pubDate = c. Element( "pubDate" ). Value,
                                refers = GetCommentRefers( c. Element( "refers" ) ),
                                replies = GetCommentReplies( c. Element( "replies" ) ),
                                appClientType = c. Element( "appclient" ) != null ? c. Element( "appclient" ). Value. ToInt32( ) : 1,
                            }
                        ). ToArray( );
                    return result;
                }
            }
            catch ( Exception e)
            {
                pageSize = 0;
                System. Diagnostics. Debug. WriteLine( "CommentList 解析错误: {0}", e. Message );
                return null;
            }
        }
        private static CommentRefer[ ] GetCommentRefers(XElement refers)
        {
            try
            {
                if ( refers == null || refers. HasElements == false )
                {
                    return null;
                }
                else
                {
                    CommentRefer[ ] result = refers. Elements( "refer" ). Select(
                            r => new CommentRefer
                            {
                                body = r. Element( "referbody" ). Value,
                                title = r. Element( "refertitle" ). Value,
                            }
                        ). ToArray( );
                    return result;
                }
            }
            catch ( Exception e)
            {
                System. Diagnostics. Debug. WriteLine( "CommentRefer 解析错误: {0}", e. Message );
                return null;
            }

        }
        private static CommentReply[ ] GetCommentReplies(XElement replies)
        {
            try
            {
                if ( replies == null || replies. HasElements == false )
                {
                    return null;
                }
                else
                {
                    CommentReply[ ] result = replies. Elements( "reply" ). Select(
                            r => new CommentReply
                            {
                                author = r. Element( "rauthor" ). Value,
                                pubDate = r. Element( "rpubDate" ). Value,
                                content = r. Element( "rcontent" ). Value,
                            }
                        ). ToArray( );
                    return result;
                }
            }
            catch ( Exception e)
            {
                System. Diagnostics. Debug. WriteLine( "CommentReply 解析错误: {0}", e. Message );
                return null;
            }
           
        }

        public static ApiResult GetLoginResult(string response, out MyInfo myInfo)
        {
            myInfo = new MyInfo( );
            Tool. GetUserNotice( response );
            try
            {
                ApiResult result = GetApiResult( response );
                XElement root = XElement. Parse( response );
                XElement user = root. Element( "user" );
                if ( user != null )
                {
                    myInfo. uid = user. Element( "uid" ). Value. ToInt32( );
                    myInfo. name = user. Element( "name" ). Value;
                    myInfo. followersCount = user. Element( "followers" ). Value. ToInt32( );
                    myInfo. fansCount = user. Element( "fans" ). Value. ToInt32( );
                    myInfo. portrait = user. Element( "portrait" ). Value;
                }
                else
                {
                    myInfo = null;
                }
                return result;
            }
            catch ( Exception e)
            {
                myInfo = null;
                System. Diagnostics. Debug. WriteLine( "ApiResult 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static ActiveUnit[ ] GetUserInfo(string response, out int pageSize, out UserInfo userInfo)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                pageSize = root. Element( "pagesize" ). Value. ToInt32( );

                XElement user = root. Element( "user" );
                userInfo = new UserInfo
                {
                    name = user. Element( "name" ). Value,
                    uid = user. Element( "uid" ). Value. ToInt32( ),
                    portrait = user. Element( "portrait" ). Value,
                    joinTime = user. Element( "jointime" ). Value,
                    gender = user. Element( "gender" ). Value,
                    from = user. Element( "from" ). Value,
                    devplatform = user. Element( "devplatform" ). Value,
                    expertise = user. Element( "expertise" ). Value,
                    relation = user. Element( "relation" ). Value. ToInt32( ),
                    latestonline = user. Element( "latestonline" ). Value,
                };

                int pageSize2;
                return GetActiveList( response, out pageSize2 );
            }
            catch ( Exception e)
            {
                pageSize = 0;
                userInfo = null;
                Debug. WriteLine( "UserInfo 解析错误: {0}", e. Message );
                return null;
            }
            
        }

        public static BlogUnit[ ] GetUserBlogList(string response, out int pageSize)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                pageSize = root. Element( "pagesize" ). Value. ToInt32( );
                XElement blogs = root. Element( "blogs" );
                if ( blogs == null || blogs. HasElements == false )
                {
                    return null;
                }
                else
                {
                    BlogUnit[ ] result = blogs. Elements( "blog" ). Select(
                           b => new BlogUnit
                           {
                               id = b. Element( "id" ). Value. ToInt32( ),
                               url = b. Element( "url" ). Value,
                               title = b. Element( "title" ). Value,
                               pubDate = b. Element( "pubDate" ). Value,
                               authorUID = b. Element( "authoruid" ). Value. ToInt32( ),
                               authorName = b. Element( "authorname" ). Value,
                               commentCount = b. Element( "commentCount" ). Value. ToInt32( ),
                               documentType = b. Element( "documentType" ). Value == "1",
                           } ). ToArray( );
                    return result;
                }
            }
            catch ( Exception e )
            {
                pageSize = 0;
                Debug. WriteLine( "UserBlogList 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static int UserUpdateRelation( string response, out ApiResult result )
        {
            Tool. GetUserNotice( response );
            try
            {
                result = GetApiResult( response );
                XElement root = XElement. Parse( response );
                return root. Element( "relation" ). Value. ToInt32( );
            }
            catch ( Exception e)
            {
                result = new ApiResult { errorCode = -1, errorMessage = "xml解析错误" };
                System. Diagnostics. Debug. WriteLine( "UserUpdateRelation 解析错误: {0}", e. Message );
                return -1;
            }
        }

        public static FavUnit[ ] GetFavoriteList(string response, out int pageSize)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                pageSize = root. Element( "pagesize" ). Value. ToInt32( );
                XElement favs = root. Element( "favorites" );
                if ( favs == null || favs. HasElements == false )
                {
                    //return null;
                    return new FavUnit[ ] { };
                }
                else
                {
                    FavUnit[ ] results = favs. Elements( "favorite" ). Select(
                            f => new FavUnit
                            {
                                id = f. Element( "objid" ). Value. ToInt32( ),
                                type = f. Element( "type" ). Value. ToInt32( ),
                                title = f. Element( "title" ). Value,
                                url = f. Element( "url" ). Value,
                            }
                        ). ToArray( );
                    return results;
                }
            }
            catch ( Exception e)
            {
                pageSize = 0;
                System. Diagnostics. Debug. WriteLine( "FavoriteList 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static UserNotice GetUserNotice(string response)
        {
            try
            {
                XElement root = XElement. Parse( response );
                XElement notice = root. Element( "notice" );
                if ( notice == null )
                {
                    return null;
                }
                else
                {
                    UserNotice n = new UserNotice
                    {
                        atMeCount = notice. Element( "atmeCount" ). Value. ToInt32( ),
                        msgCount = notice. Element( "msgCount" ). Value. ToInt32( ),
                        reviewCount = notice. Element( "reviewCount" ). Value. ToInt32( ),
                        newFansCount = notice. Element( "newFansCount" ). Value. ToInt32( ),
                    };
                    //内部就直接触发事件
                    EventSingleton. Instance. RaiseGetUserNotice( n );
                    return n;
                }
            }
            catch ( Exception e)
            {
                Debug. WriteLine( "UserNotice 解析错误: {0}", e. Message );
                return null;
            }
            
        }

        public static SearchUnit[ ] GetSearchList(string response, out int pageSize)
        {
            try
            {
                XElement root = XElement. Parse( response );
                pageSize = root. Element( "pagesize" ). Value. ToInt32( );
                XElement results = root. Element( "results" );
                if ( results == null || results. HasElements == false )
                {
                    return null;
                }
                else
                {
                    return results. Elements( "result" ). Select(
                            r => new SearchUnit
                            {
                                id = r. Element( "objid" ). Value. ToInt32( ),
                                type = r. Element( "type" ). Value. ToInt32( ),
                                title = r. Element( "title" ). Value,
                                url = r. Element( "url" ). Value,
                                pubDate = r. Element( "pubDate" ). Value,
                                author = r. Element( "author" ). Value,
                            }
                        ). ToArray( );
                }
            }
            catch ( Exception e)
            {
                pageSize = 0;
                Debug. WriteLine( "SearchList 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static FriendUnit[ ] GetFriendsList(string response)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement friends = XElement. Parse( response ). Element( "friends" );
                if ( friends == null || friends. HasElements == false )
                {
                    return null;
                }
                else
                {
                    FriendUnit[ ] result = friends. Elements( "friend" ). Select(
                            f => new FriendUnit
                            {
                                name = f. Element( "name" ). Value,
                                id = f. Element( "userid" ). Value. ToInt32( ),
                                portrait = f. Element( "portrait" ). Value,
                                expertise = f. Element( "expertise" ). Value,
                                isMale = f. Element( "gender" ). Value == "1",
                            }
                        ). ToArray( );
                    return result;
                }
            }
            catch ( Exception e)
            {
                Debug. WriteLine( "FriendsList 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static SoftwareType[ ] GetSoftwareType(string response, out int count)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                count = root. Element( "softwarecount" ). Value. ToInt32( );
                XElement all = root. Element( "softwareTypes" );
                if ( all == null || all. HasElements == false )
                {
                    return null;
                }
                else
                {
                    return all. Elements( "softwareType" ). Select(
                            s => new SoftwareType
                            {
                                name = s. Element( "name" ). Value,
                                tag = s. Element( "tag" ). Value. ToInt32( ),
                            }
                        ). ToArray( );
                }
            }
            catch ( Exception e)
            {
                count = 0;
                Debug. WriteLine( "SoftwareType 解析错误: {0}", e. Message );
                return null;
            }
            
        }

        public static SoftwareUnit[ ] GetSoftwareUnits(string response, out int count)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement root = XElement. Parse( response );
                count = root. Element( "softwarecount" ). Value. ToInt32( );
                XElement all = root. Element( "softwares" );
                if ( all == null || all. HasElements == false )
                {
                    return null;
                }
                else
                {
                    return all. Elements( "softwares" ). Select(
                            s => new SoftwareUnit
                            {
                                url = s. Element( "url" ). Value,
                                description = s. Element( "description" ). Value,
                                name = s. Element( "name" ). Value,
                            }
                        ). ToArray( );
                }
            }
            catch ( Exception e)
            {
                count = 0;
                Debug. WriteLine( "SoftwareList 解析错误: {0}", e. Message );
                return null;
            }
        }

        public static MyInfo GetMyInfo(string response)
        {
            Tool. GetUserNotice( response );
            try
            {
                XElement user = XElement. Parse( response ). Element( "user" );
                return new MyInfo
                {
                    name = user. Element( "name" ). Value,
                    portrait = user. Element( "portrait" ). Value,
                    joinTime = user. Element( "jointime" ). Value,
                    gender = user. Element( "gender" ). Value,
                    from = user. Element( "from" ). Value,
                    devplatform = user. Element( "devplatform" ). Value,
                    expertise = user. Element( "expertise" ). Value,
                    favCount = user. Element( "favoritecount" ). Value. ToInt32( ),
                    fansCount = user. Element( "fanscount" ). Value. ToInt32( ),
                    followersCount = user. Element( "followerscount" ). Value. ToInt32( ),
                };
            }
            catch ( Exception e)
            {
                Debug. WriteLine( "MyInfo 解析错误: {0}", e. Message );
                return null;
            }
        }
        #endregion

        #region Http操作
        public static WebClient SendWebClient(string urlPrefix, Dictionary<string, string> parameters)
        {
            WebClient client = new WebClient( );
            client. Headers[ "User-Agent" ] = Config. UserAgent;
            if ( Config.Cookie.IsNotNullOrWhitespace() )
            {
                client. Headers[ "Cookie" ] = Config. Cookie;
            }
            /*
             * WP7 会缓存相同url 的返回结果 所以这里需要添加 guid 参数
             */
            if ( parameters != null && parameters.ContainsKey("guid") == false)
            {
                parameters. Add( "guid", Guid. NewGuid( ). ToString( ) );
            }
            string uri = HttpGetHelper. GetQueryStringByParameters( urlPrefix, parameters );
            client. DownloadStringAsync( new Uri( uri, UriKind. Absolute ) );
            return client;
        }

        public static PostClient SendPostClient(string urlPrefix, Dictionary<string, object> parameters)
        {
            /*
             * WP7 会缓存相同url 的返回结果 所以这里需要添加 guid 参数
             */
            if ( parameters != null && parameters.ContainsKey("guid") == false)
            {
                parameters. Add( "guid", Guid. NewGuid( ). ToString( ) );
            }
            PostClient client = new PostClient( parameters )
            {
                UserAgent = Config. UserAgent,
            };
            //抓取到 Cookie
            client. OnGetCookie += (cookie) =>
            {
                Config. Cookie = cookie;
            };
            client. DownloadStringAsync( new Uri( urlPrefix, UriKind. Absolute ), Config. Cookie.EnsureNotNull() );
            return client;
        }

        /// <summary>
        /// 主动呼叫获取 UserNotice
        /// </summary>
        public static void AsyncGetUserNotice( )
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"uid", Config.UID.ToString()},
            };
            WebClient client = SendWebClient( Config. api_user_notice, parameters );
            client. DownloadStringCompleted += (s, e) =>
                {
                    if ( e. Error != null )
                    {
                        return;
                    }
                    else
                    {
                        Tool. GetUserNotice( e. Result );
                    }
                };
        }

        private static Thread thread;
        /// <summary>
        /// 开始启动线程执行 线程获取 UserNotice
        /// </summary>
        public static void StartUserNoticeThread( )
        {
            if ( thread != null && thread.IsAlive )
            {
                return;
            }
            thread = new Thread( new ThreadStart(
                ( ) => {
                    while ( true )
                    {
                        System. Threading. Thread. Sleep( TimeSpan. FromMinutes( 4 ) );
                        Tool. AsyncGetUserNotice( );
                    }
                }
                ) )
            {
                IsBackground = true,
            };
            thread. Start( );
        }

        /// <summary>
        /// 清空这些消息 在进入 ActivePage 或者 MsgPage 时需要执行
        /// </summary>
        /// <param name="type"></param>
        public static void ClearUserNotice( int type )
        {
            UserNotice notice = Config. UserNoticeInPhone;
            if ( type == 1 && notice.atMeCount <= 0)
            {
                return;
            }
            else if ( type == 2 && notice.msgCount <= 0 )
            {
                return;
            }
            else if ( type == 3 && notice.reviewCount <= 0 )
            {
                return;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"uid", Config.UID.ToString()},
                {"type", type.ToString()},
            };
            WebClient client = Tool. SendWebClient( Config. api_notice_clear, parameters );
            client. DownloadStringCompleted += (s, e) =>
                {
                    if ( e. Error != null )
                    {
                        System. Diagnostics. Debug. WriteLine( "清空UserNotice时网络错误: {0}", e. Error. Message );
                        return;
                    }
                    else
                    {
                        ApiResult result = Tool. GetApiResult( e. Result );
                    }
                };
        }

        #endregion

        #region Html组装以及站内链接处理以及软件更新
        /// <summary>
        /// HTML组装新闻页面
        /// </summary>
        /// <param name="n">新闻详情</param>
        /// <returns>Html字符串</returns>
        public static string HtmlProcess_News(NewsDetail n)
        {
            string author_str = string. Format( "<a style='text-decoration:none' href=http://my.oschina.net/u/{0}>{1}</a> 发布于 {2}  &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style='font-weight:bold;font-size:52px;text-align:right'><a href='http://www.wangjuntom.com' style='text-decoration:none'>资讯首页</a></span>", n. authorID, n. author, Tool. IntervalSinceNow( n. pubDate ) );
            string software = string. Empty;
            if ( n.softwareName.IsNotNullOrWhitespace() )
            {
                software = string. Format( "<div id='oschina_software' style='margin-top:16px;color:#FF0000;font-size:17px;font-weight:bold'>更多关于:&nbsp;<a href='{0}'>{1}</a>&nbsp;的详细信息</div>", n. softwareLink, n. softwareName );
            }
            string html = string. Format( "<body style='background-color:#FFFFFF'>{0}<div id='oschina_title'>{1}</div><div id='oschina_outline'>{2}</div><hr/><div id='oschina_body'>{3}</div>{4}{5}{6}</body>", Config. HTML_Style, n. title, author_str, n. body, software, GenerateRelativeNewsString( n ), Config. HTML_Bottom );
            
            html  = ControlHelper. ConvertExtendedASCII( html );
            return html;
        }
        /// <summary>
        /// HTML组装相关新闻
        /// </summary>
        /// <param name="n">新闻详情</param>
        /// <returns>Html字符串</returns>
        private static string GenerateRelativeNewsString(NewsDetail n)
        {
            if ( n. relativies == null || n. relativies. Length == 0 )
            {
                return string. Empty;
            }
            else
            {
                StringBuilder builder = new StringBuilder( 600 );
                n. relativies. ForAll( r => builder. AppendFormat( "<a href={0} style='text-decoration:none;line-height:30px'>{1}</a><p/>", r. url, r. title ) );
                string result = string. Format( "<hr style='line-height:30px'/><p/><div style='line-height:40px;font-size:22px;font-weight:bold;'>相关文章</div><div style='font-size:16px'><p/>{0}</div>", builder. ToString( ) );
                return result;
            }
        }

        /// <summary>
        /// HTML组装问答页面
        /// </summary>
        /// <param name="p">问答详情</param>
        /// <returns>Html字符串</returns>
        public static string HtmlProcess_Post(PostDetail p)
        {
            string author_str = string. Format( "<a href=http://my.oschina.net/u/{0}>{1}</a> 发布于 {2}", p. authorID, p. author, Tool. IntervalSinceNow( p. pubDate ) );
            string html = string. Format( "<body style='background-color:#FFFFFF;'>{0}<div id='oschina_title'>{1}</div><div id='oschina_outline'>{2}</div><hr/><div id='oschina_body'>{3}</div>{4}{5}</body>", Config. HTML_Style, p. title, author_str, p. body, GetTags( p. tags ), Config. HTML_Bottom );
            html = ControlHelper. ConvertExtendedASCII( html );
            return html;
        }
        /// <summary>
        /// 组装 Tag 标签
        /// </summary>
        /// <returns>组装好的字符串</returns>
        /// <param name="type">文章类型</param>
        /// <param name="tags">标签集合</param>
        private static string GetTags(string[ ] tags)
        {
            if (  tags. IsNotNullOrEmpty( ) )
            {
                StringBuilder sb = new StringBuilder( 256 );
                tags. ForAll( t => sb. AppendFormat( "<a style='background-color: #bbd6f3;border-bottom: 2px solid #3E6D8E;border-right: 2px solid #7F9FB6;color: #284a7b;font-size: 14pt;-webkit-text-size-adjust: none;line-height: 2.4;margin: 2px 2px 2px 0;padding: 2px 4px;text-decoration: none;white-space: nowrap;' href='http://www.oschina.net/question/tag/{0}' >&nbsp;{0}&nbsp;</a>&nbsp;&nbsp;", t ) );
                return string. Format( "<div>{0}</div>", sb. ToString( ) );
            }
            else
                return string. Empty;
        }

        /// <summary>
        /// HTML组装博客页面
        /// </summary>
        /// <param name="b">博客详情</param>
        /// <returns>Html字符串</returns>
        public static string HtmlProcess_Blog(BlogDetail b)
        {
            string author_str = string. Format( "<a href=http://my.oschina.net/u/{0}>{1}</a> 发布于 {2}", b. authorID, b. author, Tool. IntervalSinceNow( b. pubDate ) );
            string html = string. Format( "<body style='background-color:#ffffff'>{0}<div id='oschina_title'>{1}</div><div id='oschina_outline'>{2}</div><hr/><div id='oschina_body'>{3}</div>{4}</body>", Config. HTML_Style, b. title, author_str, b. body, Config. HTML_Bottom );
            html = ControlHelper. ConvertExtendedASCII( html );
            return html;
        }

        /// <summary>
        /// HTML组装软件页面
        /// </summary>
        /// <param name="s">软件详情</param>
        /// <returns>Html字符串</returns>
        public static string HtmlProcess_Software(SoftwareDetail s)
        {
            string str_title = string. Format( "{0} {1}", s. extensionTitle, s. title );
            string tail = string. Format( "<p/><p/><div style='margin-top:80px'><hr/><div style='margin-top:20px'/><span style='font-weight:bold'>授权协议:</span> {0}</div><p/><div><span style='font-weight:bold'>开发语言:</span> {1}</div><p/><div><span style='font-weight:bold'>操作系统:</span> {2}</div><p/><div  style='margin-bottom:80px'><span style='font-weight:bold'>收录时间:</span> {3}<div style='margin-top:20px'/><hr/></div><p/><p/>", s. license, s. language, s. os, s. recordTime );

            string strHomePage = string. Empty;
            string strDocument = string. Empty;
            string strDownload = string. Empty;
            if ( s.homePage.IsNotNullOrWhitespace() )
            {
                strHomePage = string. Format( "<a href={0}><input type='button' value='软件首页' style='font-size:48px;'/></a>", s. homePage );
            }
            if ( s.document.IsNotNullOrWhitespace() )
            {
                strDocument = string. Format( "<a href={0}><input type='button' value='软件文档' style='font-size:48px;'/></a>", s. document );
            }
            if ( s.download.IsNotNullOrWhitespace() )
            {
                strDownload = string. Format( "<a href={0}><input type='button' value='软件下载' style='font-size:48px;'/></a>", s. download );
            }
            string buttonString = string. Format( "<p>{0}&nbsp;&nbsp;&nbsp;&nbsp;{1}&nbsp;&nbsp;&nbsp;&nbsp;{2}</p>", strHomePage, strDocument, strDownload );

            string html = string. Format( "<body style='background-color:#FFFFFF'><div style='margin-top:40px'/>{0}<div id='oschina_title'><img src='{1}' width='68' height='68'/>{2}</div><div style='margin-top:20px'/><hr/><div id='oschina_body'>{3}</div><div>{4}</div>{5}{6}</body>", Config. HTML_Style, s. logo, str_title, s. body, tail, buttonString, Config. HTML_Bottom );
            html = ControlHelper. ConvertExtendedASCII( html );
            return html;
        }

        /// <summary>
        /// HTML组装动弹页面
        /// </summary>
        /// <param name="t">动弹详情</param>
        /// <returns>Html字符串</returns>
        public static string HtmlProcess_Tweet(TweetDetail t)
        {
            string pubTime = string. Format( "在{0} 更新了动态", Tool. IntervalSinceNow( t. pubDate ) );
            if ( t.appClient != AppClientType.Web )
            {
                pubTime = string. Format( "{0}<p/><span style='font-size:16px;line-height:18px;'>{1}</span>", pubTime, GetAppClientString( t. appClient ) );
            }
            string imgHtml = string. Empty;
            if ( t.imBig.IsNotNullOrWhitespace() )
            {
                //注意这里的处理
                imgHtml = string. Format( "<br/><a href='http://www.wangjuntomtweetimg.com/'><img style='max-width:470px;' src='{0}'/></a>", t. imBig );
            }
            string html = string. Format( "{0}<body style='background-color:#FFFFFF'><div id='oschina_title'><a href='http://my.oschina.net/u/{1}' style='font-size:80px;line-height:82px'>{2}</a></div><div style='margin-top:40px' /><div id='oschina_outline' style='font-size:58px;line-height:60px'>{3}</div><br/><hr/><br/><div id='oschina_body' style='font-size:19px;line-height:27px;'>{4}</div>{5}{6}</body>", Config. HTML_Style, t. authorID, t. author, pubTime, t. body, imgHtml, Config. HTML_Bottom );
            html = ControlHelper. ConvertExtendedASCII( html );
            return html;
        }

        /// <summary>
        /// 检查是否需要更新版本
        /// </summary>
        /// <param name="url">xml文件的url访问地址</param>
        public static void CheckVersionNeedUpdate( string url )
        {
            WebClient client = Tool. SendWebClient( url, null );
            client. DownloadStringCompleted += (s, e) =>
                {
                    if ( e.Error != null )
                    {
                        return;
                    }
                    try
                    {
                        XElement root = XElement. Parse( e. Result );
                        if ( root != null )
                        {
                            XElement wp7 = root. Element( "update" ). Element( "wp7" );
                            if ( wp7 != null )
                            {
                                if ( Config. AppVersion != wp7. Value && Config.AppVersion.ToFloat() < wp7.Value.ToFloat())
                                {
                                    if ( MessageBox. Show( "有新版本可用，您确定要升级本客户端吗?", "温馨提示", MessageBoxButton. OKCancel ) == MessageBoxResult. OK )
                                    {
                                        MarketplaceDetailTask task = new MarketplaceDetailTask { };
                                        task. Show( );
                                    }
                                }
                                else
                                {
                                    MessageBox. Show( "您使用的版本已经是最新的" );
                                }
                            }
                        }
                    }
                    catch ( Exception )
                    {
                    }
                };
        }

        #region  浏览器缩放图片处理
        public static string ProcessHTMLString(string sourceHTML, bool isImgsHidden = false, int imgWidth = 0)
        {
            var info = NetworkInterface. NetworkInterfaceType;
            switch ( info )
            {
                case NetworkInterfaceType. MobileBroadbandCdma:
                case NetworkInterfaceType. MobileBroadbandGsm:
                    isImgsHidden = !Config. IsImgsVisible;
                    break;
            }  
            //然后转码
            HtmlDocument document = new HtmlDocument( );
            document. LoadHtml( sourceHTML );
            if ( document == null )
            {
                return sourceHTML;
            }
            var images = document. DocumentNode. SelectNodes( "//img" );
            if ( images == null )
            {
                return sourceHTML;
            }
            foreach ( HtmlNode image in images )
            {
                if ( isImgsHidden )
                {
                    if ( image. Attributes[ "src" ] != null )
                    {
                        image. Attributes[ "src" ]. Value = string. Empty;
                    }
                    continue;
                }
                if ( imgWidth != 0 )
                {
                    image. Attributes. Add( "width", string.Format("{0}px", imgWidth) );
                }
                if ( image. Attributes[ "width" ] != null )
                {
                    //修改高宽
                    int width = GetNumFromString( image. Attributes[ "width" ]. Value );
                    if ( width > 0 )
                    {
                        image. Attributes[ "width" ]. Value = width. ToString( );
                    }
                    else
                    {
                        image. Attributes[ "width" ]. Remove( );
                    }
                }
                if ( image. Attributes[ "height" ] != null )
                {
                    image. Attributes[ "height" ]. Remove( );
                }
            }
            return document. DocumentNode. InnerHtml;
        }
        private static int GetNumFromString(string originStr)
        {
            string num_str = Regex. Replace( originStr, @"[^\d]*", "" );
            int num;
            if ( int. TryParse( num_str, out num ) )
            {
                return num <= 480 ? num * 2 : 960;
            }
            else
            {
                return -1;
            }
        }
        #endregion

        /// <summary>
        /// 处理站内链接地址的跳转 这里需要分析  oschina.net 的url关系
        /// </summary>
        /// <param name="link">站内链接</param>
        public static void ProcessAppLink( string link )
        {
            link = link. EnsureNotNull( );

            #region 如果是动弹图片的链接点击
            if ( link == "http://wangjuntom" )
            {
                return;
            }
            #endregion
        //http://www.oschina.net/question/tag/{1}
            //对 link 进行分析
            string search = "oschina.net";
            if ( link.IndexOf(search) >= 0 )
            {
                link = link. Substring( 7 );
                string prefix = link. Substring( 0, 3 );
                switch ( prefix )
                {
    #region 此情况为博客，动弹，个人专页
                    case "my.":
                        {
                            string[ ] array = link. Split( '/' );
                            //个人专页 用户名形式
                            if ( array.Length <= 2 )
                            {
                                //进入用户专页 其中 array[1] 表示用户名
                                ToUserPage( -1, array[ 1 ] );
                                return;
                            }
                            else if ( array.Length <= 3 )
                            {
                                if ( array[1] == "u" )
                                {
                                    //进入用户专页 其中 array[2] 表示用户 uid
                                    ToUserPage( array[ 2 ]. ToInt32( ), null );
                                    return;
                                }
                            }
                            else if ( array.Length <= 4 )
                            {
                                switch ( array[2] )
                                {
                                        //进入博客专页
                                    case "blog":
                                        //博客id 是 array[3]
                                        Tool. ToDetailPage( array[ 3 ], DetailType. Blog );
                                        return;
                                        //进入动弹专页
                                    case "tweet":
                                        Tool. ToDetailPage( array[ 3 ], DetailType. Tweet );
                                        //动弹id 是 array[3]
                                        return;
                                }
                            }
                            else if ( array.Length <= 5 )
                            {
                                if ( array[3] == "blog" )
                                {
                                    //进入博客专页 博客id 是 array[4]
                                    Tool. ToDetailPage( array[ 4 ], DetailType. Blog );
                                    return;
                                }
                            }
                        }
                        break;
    #endregion

    #region 此情况为 新闻，软件，问答，标签
                    case "www":
                        {
                            string[ ] array = link. Split( '/' );
                            if ( array.Length >= 3 )
                            {
                                switch ( array[1] )
                                {
                                    case "news":
                                        //进入新闻专页 id 为 array[2]
                                        Tool. ToDetailPage( array[ 2 ], DetailType. News );
                                        return;
                                    case "p":
                                        //进入软件专页 软件ident 为 array[2]
                                        Tool. ToDetailPage( array[ 2 ], DetailType. Software );
                                        return;
                                    case "question":
                                        if (array.Length == 3)
	                                    {
		                                      string[ ] array2 = array[ 2 ]. Split( '_' );
                                              if ( array2.Length >= 2 )
                                              {
                                                  //进入问答专页 id为 array2[1]
                                                  Tool. ToDetailPage( array2[ 1 ], DetailType. Post );
                                                  return;
                                              }
	                                    }
                                        else if (array.Length >=  4)
	                                    {
                                            string tag = "";
                                            if ( array. Length == 4 )
                                            {
                                                tag = array[ 3 ];
                                            }
                                            else
                                            {
                                                array = array. Skip( 3 ). ToArray( );
                                                tag = array. Aggregate((i, j) => string. Format( @"{0}/{1}", i, j ) );
                                            }
                                            tag = System. Net. HttpUtility. UrlEncode( tag );
                                            //string _url =System.Net.HttpUtility.UrlEncode( );
                                            Tool. To( string. Format( "/PostsPage.xaml?tag={0}", tag ) );
                                            return;
	                                    }
                                        break;
                                }
                            }
                        }
   #endregion
                        break;
                }
                //非站内链接将使用IE打开
                WebBrowserTask task = new WebBrowserTask { Uri = new Uri( link.Substring(0,4)=="http" ? link: string.Format("http://{0}", link), UriKind. Absolute ) };
                task. Show( );
            }
         }

        /// <summary>
        /// 跳转到某页
        /// </summary>
        /// <param name="appPageUrl">跳转目的地的查询字符串</param>
        public static void To(string appPageUrl)
        {
            ( Application. Current. RootVisual as PhoneApplicationFrame ). Navigate( new Uri( appPageUrl, UriKind. Relative ) );
        }

        /// <summary>
        /// 跳转到文章详情页
        /// </summary>
        /// <param name="id">文章详情ID</param>
        /// <param name="detailType">文章详情类型</param>
        public static void ToDetailPage( string id, DetailType detailType )
        {
            //Tool. To( string. Format( "/DetailPage.xaml?id={0}&type={1}", id, ( int ) detailType ) );
            Tool. To( string. Format( "/DetailPage2.xaml?id={0}&type={1}", id, ( int ) detailType ) );
        }

        /// <summary>
        /// 进入用户专页 优先uid 如果使用用户名 则将uid = -1 即可
        /// </summary>
        /// <param name="uid">用户uid</param>
        /// <param name="userName">用户姓名</param>
        public static void ToUserPage(int uid, string userName = "")
        {
            //考虑uid
            if ( uid > 0 )
            {
                Tool. To( string. Format( "/UserPage.xaml?uid={0}", uid ) );
            }
            //考虑userName
            else
            {
                Tool. To( string. Format( "/UserPage.xaml?name={0}", userName ) );
            }
        }

        #endregion

        #region 字符串转换
        /// <summary>
        /// 时间显示字符串转换
        /// </summary>
        /// <param name="sourceTime">原始时间</param>
        /// <returns>转换后的时间</returns>
        public static string IntervalSinceNow(string sourceTime)
        {
            DateTime source = DateTime. Now;
            DateTime. TryParse( sourceTime, out source );
            TimeSpan span = DateTime. Now - source;
            long seconds = (long)span. TotalSeconds;
            string timeString = string. Empty;
            //一小时内
            if ( seconds / 3600 < 1 )
            {
                if ( seconds / 60 < 1 )
                {
                    timeString = "1";
                }
                else
                {
                    timeString = ( seconds / 60 ). ToString( );
                }
                return string. Format( "{0}分钟前", timeString );
            }
                //一天内
            else if ( seconds / 3600 >= 1 && seconds / 86400 < 1 )
            {
                return string. Format( "{0}小时前", seconds / 3600 );
            }
                //十天内
            else if ( seconds / 86400 >= 1 && seconds / 864000 < 1 )
            {
                return string. Format( "{0}天前", seconds / 86400 );
            }
                //十天以上
            else
            {
                return source. ToString( "yyyy-MM-dd" );
            }
        }

        /// <summary>
        /// 动弹的客户端平台显示
        /// </summary>
        public static string GetAppClientString(AppClientType type)
        {
            switch ( type )
            {
                case AppClientType.None:
                case AppClientType. Web:
                    return string. Empty;
                case AppClientType. Mobile:
                    return "来自手机";
                case AppClientType. Android:
                    return "来自Android";
                case AppClientType. iOS:
                    return "来自iPhone";
                case AppClientType. WP7:
                    return "来自Windows Phone";
                default:
                    return string. Empty;
            }
        }

        //个人动态的字符串组装
        private static Run blueText(string text)
        {
            return new Run
            {
                Text = text,
                FontSize = 24,
                //Foreground = new SolidColorBrush( Color. FromArgb( 255, 13, 109, 168 ) ),
                Foreground = Application. Current. Resources[ "PhoneAccentBrush" ] as SolidColorBrush,
            };
        }
        private static Run grayText(string text)
        {
            return new Run
            {
                Text = text,
                FontSize = 22,
                Foreground = new SolidColorBrush( Color. FromArgb( 255, 120, 120, 120 ) ),
            };
        }
        private static Run orangeText(string text)
        {
            return new Run
            {
                Text = text,
                FontSize = 21,
                FontStyle = FontStyles.Italic,
                //Foreground = new SolidColorBrush( Color. FromArgb( 195, 255, 80, 0 ) ),
                //Foreground = new SolidColorBrush( (Application. Current. Resources[ "PhoneAccentBrush" ] as SolidColorBrush).Color == Colors.Orange ? Colors.Magenta : Colors.Orange ),
                Foreground = new SolidColorBrush(Colors.Orange),
            };
        }
        private static Run themeText(string text)
        {
            return new Run
            {
                Text = text,
                FontSize = 24,
                Foreground = Application. Current. Resources[ "PhoneAccentBrush" ] as SolidColorBrush,
            };
        }
        private static Run blackText(string text)
        {
            return new Run
            {
                Text = text,
                FontSize = 24,
            };
        }
        private static Run smallGrayText(string text)
        {
            return new Run
            {
                Text = text,
                FontSize = 20,
                Foreground = new SolidColorBrush( Color. FromArgb( 255, 153, 153, 153 ) ),
            };
        }

        /// <summary>
        /// 动态列表单元处理
        /// </summary>
        /// <param name="active">动态对象</param>
        /// <param name="txt">TextBlock控件引用</param>
        public static void ProcessActiveUnit(ActiveUnit active, TextBlock txt)
        {
            txt. Inlines. Clear( );
            Run author = blueText( active. author );

            //注意 pubDate前必须有LineBreak
            Run pubDate = smallGrayText( string. Format( "{0} {1}", Tool. IntervalSinceNow( active. pubDate ), Tool. GetAppClientString( ( AppClientType ) active. appClient ) ) );

            //注意 reply 前必须有LineBreak
            Run reply = null;
            if ( active.objReplyBody.IsNotNullOrWhitespace( ) )
            {
                reply = orangeText( string. Format( "{0}:{1}", active. objReplyName, active. objReplyBody ) );
            }

            List<Inline> msgs = new List<Inline>( );
            #region 针对 msgs 的处理
            switch ( active.objType )
            {
                case 6:
                    if ( active.objCatalog == 0 )
                    {
                        msgs. Add( grayText( " 发布了一个职位 " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
                case 20:
                    if ( active.objCatalog == 0 )
                    {
                        msgs. Add( grayText( " 在职位 " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( grayText( " 发表评论" ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
                case 32:
                    if ( active.objCatalog == 0 )
                    {
                        msgs. Add( grayText( " 加入了开源中国" ) );
                    }
                    break;
                case 1:
                    if ( active.objCatalog == 0 )
                    {
                        msgs. Add( grayText( " 添加了开源项目 " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
                case 2:
                    if ( active. objCatalog == 1 )
                    {
                        msgs. Add( grayText( " 在讨论区提问: " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    else if(active.objCatalog == 2)
                    {
                        msgs. Add( grayText( " 发表了新话题: " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
                case 3:
                    if ( active.objCatalog == 0 )
                    {
                        msgs. Add( grayText( " 发表了博客 " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
                case 4:
                    if ( active. objCatalog == 0 )
                    {
                        msgs. Add( grayText( " 发表一篇新闻 " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
                case 5:
                    if ( active. objCatalog == 0 )
                    {
                        msgs. Add( grayText( " 分享了一段代码 " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
                case 16:
                    if ( active. objCatalog == 0 )
                    {
                        msgs. Add( grayText( " 在新闻 " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( grayText( " 发表评论" ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
                case 17:
                    if ( active.objCatalog == 1 )
                    {
                        msgs. Add( grayText( " 回答了问题: " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    else if ( active.objCatalog == 2 )
                    {
                        msgs. Add( grayText( " 回复了话题: " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    else if ( active.objCatalog == 3 )
                    {
                        msgs. Add( grayText( " 在 " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( grayText( " 对回帖发表评论" ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
                case 18:
                    if ( active. objCatalog == 0 )
                    {
                        msgs. Add( grayText( " 在博客 " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( grayText( " 发表评论" ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
                case 19:
                    if ( active. objCatalog == 0 )
                    {
                        msgs. Add( grayText( " 在代码 " ) );
                        msgs. Add( themeText( active. objTitle ) );
                        msgs. Add( grayText( " 发表评论" ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
                case 100:
                    if ( active. objCatalog == 0 )
                    {
                        msgs. Add( grayText( " 更新了动态" ) );
                        msgs. Add( new LineBreak { FontSize = 6 } );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak () );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
                case 101:
                    if ( active. objCatalog == 0 )
                    {
                        msgs. Add( grayText( " 回复了动态" ) );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( new Run { Text = " ", FontSize = 6 } );
                        msgs. Add( new LineBreak( ) );
                        msgs. Add( blackText( active. message ) );
                    }
                    break;
            }
            #endregion
            //开始添加
            txt. Inlines. Add( author );
            msgs. ForEach( m => txt. Inlines. Add( m ) );
            if ( reply != null )
            {
                txt. Inlines. Add( new LineBreak( ) );
                txt. Inlines. Add( new Run { Text = " ", FontSize = 8 } );
                txt. Inlines. Add( new LineBreak( ) );
                txt. Inlines. Add( reply );
            }
            txt. Inlines. Add( new LineBreak( ) );
            txt. Inlines. Add( new Run { Text = " ", FontSize = 12 } );
            txt. Inlines. Add( new LineBreak( ) );
            txt. Inlines. Add( pubDate );
        }
        #endregion

        #region 订到桌面
        /// <summary>
        /// 将某篇文章订到桌面上
        /// </summary>
        /// <param name="id">文章ID</param>
        /// <param name="type">文章类型</param>
        /// <param name="title">文章标题</param>
        /// <param name="author">作者</param>
        /// <param name="commentCount">评论个数</param>
        public static void CreateTile(string id, DetailType type, string title, string author, int commentCount = 0)
        {
            string queryString = string. Format( "id={0}&type={1}", id, ( int ) type );
            ShellTile tile = ShellTile. ActiveTiles. FirstOrDefault( t => t. NavigationUri. ToString( ). Contains( queryString ) );
            if ( tile == null )
            {
                StandardTileData tileData = new StandardTileData
                {
                    Title = title. EnsureNotNull( ),
                    Count = commentCount,
                    BackTitle = "开源中国",
                    BackContent = title,
                };
                ShellTile. Create( new Uri( string. Format( "/DetailPage2.xaml?{0}", queryString ), UriKind. Relative ), tileData );
            }
            else
            {
                string notice = string.Empty;
                switch ( type )
                {
                    case DetailType. News:
                        notice = "此资讯已经被订到桌面上";
                        break;
                    case DetailType. Post:
                        notice = "此问答已经被订到桌面上";
                        break;
                    case DetailType. Blog:
                        notice = "此博客已经被订到桌面上";
                        break;
                    case DetailType. Software:
                        notice = "此软件已经被订到桌面上";
                        break;
                }
                MessageBox. Show( notice );
            }
        }

        #endregion

        #region 集合重复元素筛选
        public static int[ ] GetIDS_ID<T>(ObservableCollection<object> datas) where T : BaseUnit
        {
            return datas. Select(
                    d =>
                    {
                        BaseUnit u = d as BaseUnit;
                        return u == null ? -1 : u. id;
                    }
                ). ToArray( );
        }
        public static string[ ] GetIDS_ID_Type<T>(ObservableCollection<object> datas) where T : BaseUnitWithType
        {
            return datas. Select(
                    d =>
                    {
                        BaseUnitWithType u = d as BaseUnitWithType;
                        return u == null ? null : string. Format( "{0}_{0}", u. id. ToString( ), u. type. ToString( ) );
                    }
                ). ToArray( );
        }
        public static string[ ] GetIDS_Name<T>(ObservableCollection<object> datas) where T : BaseUnitWithName
        {
            return datas. Select(
                    d =>
                    {
                        BaseUnitWithName u = d as BaseUnitWithName;
                        return u == null ? null : u. name;
                    }
                ). ToArray( );
        }
        #endregion

        #region 登陆检测
        /// <summary>
        /// 检测是否登陆 如果没有登陆则弹出登陆提示框
        /// </summary>
        /// <param name="message">提示语句</param>
        /// <param name="title">提示标题</param>
        /// <returns>true: 表示已经登陆 false: 表示没有登陆</returns>
        public static bool CheckLogin( string message = "请先登录", string title = null )
        {
            if ( Config. IsLogin )
            {
                return true;
            }
            else
            {
                if ( MessageBox.Show(message, title == null ? "温馨提示" : title, MessageBoxButton.OKCancel) == MessageBoxResult.OK )
                {
                    Tool. To( "/LoginPage.xaml?tag=needback" );
                }
                return false;
            }
        }

        #endregion

        #region 后台线程
        /// <summary>
        /// 动弹后台发送中，当动弹中包含图片时 将100%采用这个方法发送
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="gStream">图像数据流</param>
        /// <param name="parameters">参数</param>
        public static void AsyncPubTweetWithImage(string fileName, Stream gStream, Dictionary<string, object> parameters)
        {
            StandardPostClient client = new StandardPostClient { UserAgent = Config. UserAgent };
            client. DownloadStringCompleted += (s, e1) =>
            {
                if ( e1. Error != null )
                {
                    EventSingleton. Instance. ToastMessage( "请注意", "带图片的动弹发送失败" );
                    return;
                }
                else
                {
                    ApiResult result = Tool. GetApiResult( e1. Result );
                    if ( result != null )
                    {
                        switch ( result. errorCode )
                        {
                            case 1:
                                Config. ClearCacheTweet( );
                                EventSingleton. Instance. ToastMessage( "温馨提示", "带图片的动弹发送成功" );
                                break;
                            case 0:
                            case -1:
                            case -2:
                                EventSingleton. Instance. ToastMessage( "温馨提示", result. errorMessage );
                                break;
                        }
                    }
                }
            };
            //开始发送
            client. UploadFilesToRemoteUrl( Config. api_tweet_pub,
                                                                new string[ ] { fileName },
                                                                new Stream[ ] { gStream },
                                                                parameters,
                                                                Config. Cookie,
                                                                true );
        }
        /// <summary>
        /// 后台更新头像
        /// </summary>
        /// <param name="gStream">头像数据流</param>
        /// <param name="parameters">http post 参数</param>
        public static void AsyncUserUpdatePortrait( Stream gStream, Dictionary<string, object> parameters )
        {
            StandardPostClient client = new StandardPostClient { UserAgent = Config. UserAgent };
            client. DownloadStringCompleted += (s, e1) =>
            {
                if ( e1. Error != null )
                {
                    EventSingleton. Instance. ToastMessage( "请注意", "用户更新头像失败" );
                    return;
                }
                else
                {
                    ApiResult result = Tool. GetApiResult( e1. Result );
                    if ( result != null )
                    {
                        switch ( result. errorCode )
                        {
                            case 1:
                                EventSingleton. Instance. ToastMessage( "温馨提示", "您的头像更新成功" );
                                ReGetMyInfoOnUpdatePortrait( );
                                break;
                            case 0:
                            case -1:
                            case -2:
                                EventSingleton. Instance. ToastMessage( "温馨提示", result. errorMessage );
                                break;
                        }
                    }
                }
            };
            //开始发送
            client. UploadFilesToRemoteUrl( Config.api_userinfo_update,
                                                                new string[ ] { "avatar.jpg" },
                                                                new Stream[ ] { gStream },
                                                                parameters,
                                                                Config. Cookie,
                                                                true, "portrait" );
        }

        private static void ReGetMyInfoOnUpdatePortrait( )
        {
            //然后网络获取
            Dictionary<string, string> parameters = new Dictionary<string, string>
                    {
                        {"uid", Config.UID.ToString()},
                    };
            WebClient client = Tool. SendWebClient( Config. api_my_info, parameters );
            client. DownloadStringCompleted += (s1, e1) =>
            {
                if ( e1. Error != null )
                {
                    System. Diagnostics. Debug. WriteLine( "获取my-info 时网络错误: {0}", e1. Error. Message );
                    return;
                }
                else
                {
                    MyInfo info = Tool. GetMyInfo( e1. Result );
                    if ( info != null )
                    {
                        Config. MyInfo = info;
                        EventSingleton. Instance. RaiseUpdatePortrait( );
                    }
                }
            };
        }
        #endregion

        #region 图像处理
        public static Stream ReduceSize(BitmapImage g_bmp)
        {
            WriteableBitmap wb = new WriteableBitmap( g_bmp );
            MemoryStream g_MS = new MemoryStream( );
            System. Windows. Media. Imaging. Extensions. SaveJpeg( wb, g_MS, 800, 640, 0, 82 );
            g_MS. Seek( 0, SeekOrigin. Begin );
            return g_MS;
        }
        #endregion
    }
}