using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Fusao.Controls
{
    public sealed class RedditVoteComment : Control
    {
        public RedditVoteComment()
        {
            this.DefaultStyleKey = typeof(RedditVoteComment);
        }

        public static DependencyProperty UpvoteProperty = DependencyProperty.Register("Upvote", typeof(ICommand), typeof(RedditVoteComment), null);

        public ICommand Upvote
        {
            get
            {
                return (ICommand)GetValue(UpvoteProperty);
            }
            set
            {
                SetValue(UpvoteProperty, value);
            }
        }

        public static DependencyProperty DownvoteProperty = DependencyProperty.Register("Downvote", typeof(ICommand), typeof(RedditVoteComment), null);

        public ICommand Downvote
        {
            get
            {
                return (ICommand)GetValue(DownvoteProperty);
            }
            set
            {
                SetValue(DownvoteProperty, value);
            }
        }        
        
        public static DependencyProperty AddCommentProperty = DependencyProperty.Register("AddComment", typeof(ICommand), typeof(RedditVoteComment), null);

        public ICommand AddComment 
        {
            get
            {
                return (ICommand)GetValue(AddCommentProperty);
            }
            set
            {
                SetValue(AddCommentProperty, value);
            }
        }

        public static DependencyProperty LikedProperty = DependencyProperty.Register("Liked", typeof(bool?), typeof(RedditVoteComment), new PropertyMetadata(null, LikedPropertyChanged));

        private static void LikedPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((RedditVoteComment)sender).LikedPropertyChanged(e);
        }

        private void LikedPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            // Do the animation!
        }

        public bool? Liked
        {
            get
            {
                return (bool?)GetValue(LikedProperty);
            }
            set
            {
                SetValue(LikedProperty, value);
            }
        }

        public static DependencyProperty UpColorProperty = 
            DependencyProperty.Register("UpColor", typeof(Brush), typeof(RedditVoteComment), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush UpColor
        {
            get
            {
                return (Brush)GetValue(UpColorProperty);
            }
            set
            {
                SetValue(UpColorProperty, value);
            }
        }

        public static DependencyProperty DownColorProperty = 
            DependencyProperty.Register("DownColor", typeof(Brush), typeof(RedditVoteComment), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush DownColor
        {
            get
            {
                return (Brush)GetValue(DownColorProperty);
            }
            set
            {
                SetValue(DownColorProperty, value);
            }
        }
    }
}
