/*
 * 原作者: 王俊
 * 
 */

namespace OSChina. Model
{
    /// <summary>
    /// 好友列表单元
    /// </summary>
    public sealed class FriendUnit : BaseUnit
    {
        public string name { get; set; }
        private string _portrait;
        public string portrait 
        {
            get { return _portrait. IsNullOrWhitespace( ) ? (isMale ?  "/Resource/big_avatar.png" : "/Resource/woman1.png") : _portrait; }
            set { _portrait = value; }
        }
        public string expertise { get; set; }
        public bool isMale { get; set; }

        public string genderImg
        {
            get { return string. Format( "/Resource/{0}", isMale ? "man.png" : "woman.png" ); }
            set { ; }
        }
    }
}
