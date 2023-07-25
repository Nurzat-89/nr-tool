using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Antivirus.Controls
{
    //https://csharp.hotexamples.com/ru/examples/System.Windows.Media.Imaging/GifBitmapDecoder/-/php-gifbitmapdecoder-class-examples.html
    //https://github.com/XamlAnimatedGif/WpfAnimatedGif/blob/master/WpfAnimatedGif/ImageBehavior.cs
    //https://stackoverflow.com/questions/210922/how-do-i-get-an-animated-gif-to-work-in-wpf
    sealed class GifImage : Image
    {
        public static readonly DependencyProperty AutoStartProperty =
            DependencyProperty.Register("AutoStart", typeof(bool), typeof(GifImage), new UIPropertyMetadata(false, OnAutoStartPropertyChanged));
        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifImage), new UIPropertyMetadata(0, OnFrameIndexChanged));
        public static readonly DependencyProperty GifSourceProperty =
            DependencyProperty.Register("GifSource", typeof(string), typeof(GifImage), new UIPropertyMetadata(string.Empty, OnGifSourceChanged));

        private bool fIsInitialized;
        private GifBitmapDecoder fGifDecoder;
        private Int32Animation fAnimation;

        static GifImage()
        {
            VisibilityProperty.OverrideMetadata(typeof(GifImage),
                new FrameworkPropertyMetadata(VisibilityPropertyChanged));
        }

        private void Initialize()
        {
            var tmpSrc = this.GifSource;

            //Is Animation Started
            if (fAnimation != null)
            {
                StopAnimation();

                fGifDecoder = null;
                fAnimation = null;

                if (tmpSrc == null)
                    return;
            }

            if (Visibility != Visibility.Visible)
                return;

            fGifDecoder = new GifBitmapDecoder(
                new Uri("pack://application:,,,/" + tmpSrc), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            var tmpFramesCount = fGifDecoder.Frames.Count;
            var tmpFrame = fGifDecoder.Frames[0];
            var tmpFrameData = (BitmapMetadata)tmpFrame.Metadata;
            //Количество сотых (1/100) секунды для ожидания, прежде чем продолжить обработку потока данных.
            //Пример. tmpDelay = 5;//получено из метаданных
            // tmpDelay = (5 * 1000 / 100) = 5 * 10//приведение секунд
            ushort tmpDelay;

            if (tmpFrameData.ContainsQuery("/grctlext/Delay"))
                //NOTE tmpRate can be zero. In this case animation doesn't need
                tmpDelay = (ushort)tmpFrameData.GetQuery("/grctlext/Delay");
            else
                tmpDelay = 5;

            fAnimation = new Int32Animation(
                0, tmpFramesCount - 1,
                new Duration(TimeSpan.FromMilliseconds(tmpDelay * 10 * tmpFramesCount)));
            fAnimation.RepeatBehavior = RepeatBehavior.Forever;
            Source = tmpFrame;
            fIsInitialized = true;
        }

        private static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((Visibility)e.NewValue == Visibility.Visible)
            {
                ((GifImage)sender).StartAnimation();
            }
            else
            {
                ((GifImage)sender).StopAnimation();
            }
        }

        static void OnFrameIndexChanged(DependencyObject aObj, DependencyPropertyChangedEventArgs args)
        {
            var tmpGifImage = (GifImage)aObj;
            tmpGifImage.Source = tmpGifImage.fGifDecoder.Frames[(int)args.NewValue];
            //System.Diagnostics.Debug.Write("; index: " + args.NewValue);
        }

        /// <summary>
        /// Defines whether the animation starts on it's own
        /// </summary>
        public bool AutoStart
        {
            get { return (bool)GetValue(AutoStartProperty); }
            set { SetValue(AutoStartProperty, value); }
        }

        public int FrameIndex
        {
            get { return (int)GetValue(FrameIndexProperty); }
            set { SetValue(FrameIndexProperty, value); }
        }

        private static void OnAutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var tmpImage = (GifImage)sender;

            if ((bool)e.NewValue && tmpImage.Visibility == Visibility.Visible)
                tmpImage.StartAnimation();
        }

        public string GifSource
        {
            get { return (string)GetValue(GifSourceProperty); }
            set { SetValue(GifSourceProperty, value); }
        }

        private static void OnGifSourceChanged(DependencyObject aSender, DependencyPropertyChangedEventArgs e)
        {
            var image = (GifImage)aSender;
            image.Initialize();

            if (image.AutoStart && image.Visibility == Visibility.Visible)
                image.StartAnimation();
        }

        /// <summary>
        /// Starts the animation
        /// </summary>
        public void StartAnimation()
        {
            if (!fIsInitialized)
                this.Initialize();

            if (fAnimation == null)
                return;

            BeginAnimation(FrameIndexProperty, fAnimation);
        }

        /// <summary>
        /// Stops the animation
        /// </summary>
        public void StopAnimation()
        {
            BeginAnimation(FrameIndexProperty, null);
        }
    }
}
