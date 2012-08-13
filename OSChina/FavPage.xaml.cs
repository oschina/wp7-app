using System;
using System. Collections. Generic;
using System. Linq;
using System. Net;
using System. Windows;
using System. Windows. Controls;
using System. Windows. Documents;
using System. Windows. Input;
using System. Windows. Media;
using System. Windows. Media. Animation;
using System. Windows. Shapes;
using Microsoft. Phone. Controls;
using OSChina. Model;
using OSChina. Model. AppOnly;

namespace OSChina
{
    public partial class FavPage : PhoneApplicationPage
    {
        public FavPage( )
        {
            InitializeComponent( );

            this. fav_Software. FavType = FavType. Software;
            this. fav_Post. FavType = FavType. Post;
            this. fav_Code. FavType = FavType. Code;
            this. fav_Blog. FavType = FavType. Blog;
            this. fav_News. FavType = FavType. News;
            this. Loaded += (s, e) =>
                {
                    this. fav_Software. listBoxHelper_ReloadDelegate( );
                    this. fav_Post. listBoxHelper_ReloadDelegate( );
                    this. fav_Code. listBoxHelper_ReloadDelegate( );
                    this. fav_Blog. listBoxHelper_ReloadDelegate( );
                    this. fav_News. listBoxHelper_ReloadDelegate( );
                };
        }

        protected override void OnNavigatedFrom(System. Windows. Navigation. NavigationEventArgs e)
        {
            GC. Collect( );
            base. OnNavigatedFrom( e );
        }
    }
}