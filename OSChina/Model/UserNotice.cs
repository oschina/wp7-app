/*
 * 原作者: 王俊
 * 联系qq: 113020930
 */

namespace OSChina. Model
{
    /// <summary>
    /// 登陆用户的通知
    /// </summary>
    public sealed class UserNotice
    {
        public int atMeCount;
        public int msgCount;
        public int reviewCount;
        /// <summary>
        /// 第四个参数现在基本没有用上
        /// </summary>
        public int newFansCount;
    }
}
