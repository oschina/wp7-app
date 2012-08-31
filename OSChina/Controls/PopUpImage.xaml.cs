/*
 * 原作者: 王俊
 * 联系邮箱: wangjuntom360@hotmail.com
 */
using System;
using System. Windows. Controls;
using System. Windows. Controls. Primitives;
using System. Windows. Media. Imaging;

namespace OSChina. Controls
{
    public partial class PopUpImage : UserControl
    {
        #region Properties
        /// <summary>
        /// 弹出对象
        /// </summary>
        public Popup pop { get; private set; }

        #endregion

        #region Construct
        /// <summary>
        /// Construct
        /// </summary>
        public PopUpImage(  )
        {
            InitializeComponent( );
        }

        #endregion

        #region Public functions
        /// <summary>
        /// 创建弹出图片并显示
        /// </summary>
        /// <param name="imgUrl">图像url</param>
        /// <param name="height">图像高度 默认768</param>
        public void Create( string imgUrl, double height = 768)
        {
            this. img. Height = this. grid. Height = height;
            this. img. Source = new BitmapImage( new Uri( imgUrl, UriKind. Absolute ) );
            this. pop = new Popup
            {
                Child = this,
                IsOpen = true,
            };
        }

        #endregion
    }
}
