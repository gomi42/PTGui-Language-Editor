using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PTGui_Language_Editor
{
    /// <summary>
    /// Interaction logic for DialogFrame.xaml
    /// </summary>
    public partial class DialogLayer : UserControl
    {
        private const double DialogFadeScale = 0.95;
        private TimeSpan dialogFadeTime = TimeSpan.FromMilliseconds(200);
        private ContentControl parentDialogLayer;

        //////////////////////////////////////////////
        
        public DialogLayer()
        {
            InitializeComponent();
        
            parentDialogLayer = ((MainWindow)Application.Current.MainWindow).DialogLayer;
            parentDialogLayer.Visibility = Visibility.Visible;
        }

        //////////////////////////////////////////////
        
        public void ShowDialog(FrameworkElement dialogBox)
        {
            DialogBoxLayer.Children.Add(dialogBox);

            var storyboard = new Storyboard();
            DoubleAnimation dblAnimation;

            if (DialogBoxLayer.Children.Count == 1)
            {
                parentDialogLayer.Visibility = Visibility.Visible;
                parentDialogLayer.Content = this;

                dblAnimation = new DoubleAnimation(0.0, 1, dialogFadeTime);
                dblAnimation.FillBehavior = FillBehavior.Stop;
                Storyboard.SetTarget(dblAnimation, BackgroundLayer);
                Storyboard.SetTargetProperty(dblAnimation, new PropertyPath(UIElement.OpacityProperty));
                dblAnimation.Freeze();
                storyboard.Children.Add(dblAnimation);
            }

            var scaleTransform = new ScaleTransform(1, 1);
            dialogBox.RenderTransform = scaleTransform;
            dialogBox.RenderTransformOrigin = new Point(0.5, 0.5);

            dblAnimation = new DoubleAnimation(0.0, 1, dialogFadeTime);
            dblAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(dblAnimation, dialogBox);
            Storyboard.SetTargetProperty(dblAnimation, new PropertyPath(UIElement.OpacityProperty));
            dblAnimation.Freeze();
            storyboard.Children.Add(dblAnimation);

            dblAnimation = new DoubleAnimation(DialogFadeScale, 1.0, dialogFadeTime);
            dblAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(dblAnimation, dialogBox);
            var propertyPath = new PropertyPath("(0).(1)", new DependencyProperty[] { FrameworkElement.RenderTransformProperty, ScaleTransform.ScaleXProperty });
            Storyboard.SetTargetProperty(dblAnimation, propertyPath);
            dblAnimation.Freeze();
            storyboard.Children.Add(dblAnimation);

            dblAnimation = new DoubleAnimation(DialogFadeScale, 1.0, dialogFadeTime);
            dblAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(dblAnimation, dialogBox);
            propertyPath = new PropertyPath("(0).(1)", new DependencyProperty[] { FrameworkElement.RenderTransformProperty, ScaleTransform.ScaleYProperty });
            Storyboard.SetTargetProperty(dblAnimation, propertyPath);
            dblAnimation.Freeze();
            storyboard.Children.Add(dblAnimation);

            storyboard.Begin();
        }

        //////////////////////////////////////////////
        
        public void CloseDialog(FrameworkElement dialogBox)
        {
            var storyboard = new Storyboard();
            DoubleAnimation dblAnimation;

            if (DialogBoxLayer.Children.Count == 1)
            {
                dblAnimation = new DoubleAnimation(1, 0.0, dialogFadeTime);
                dblAnimation.FillBehavior = FillBehavior.Stop;
                Storyboard.SetTarget(dblAnimation, BackgroundLayer);
                Storyboard.SetTargetProperty(dblAnimation, new PropertyPath(UIElement.OpacityProperty));
                dblAnimation.Freeze();
                storyboard.Children.Add(dblAnimation);
            }

            dblAnimation = new DoubleAnimation(1, 0.0, dialogFadeTime);
            dblAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(dblAnimation, dialogBox);
            Storyboard.SetTargetProperty(dblAnimation, new PropertyPath(UIElement.OpacityProperty));
            dblAnimation.Freeze();
            storyboard.Children.Add(dblAnimation);

            dblAnimation = new DoubleAnimation(1.0, DialogFadeScale, dialogFadeTime);
            dblAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(dblAnimation, dialogBox);
            var propertyPath = new PropertyPath("(0).(1)", new DependencyProperty[] { FrameworkElement.RenderTransformProperty, ScaleTransform.ScaleXProperty });
            Storyboard.SetTargetProperty(dblAnimation, propertyPath);
            dblAnimation.Freeze();
            storyboard.Children.Add(dblAnimation);

            dblAnimation = new DoubleAnimation(1.0, DialogFadeScale, dialogFadeTime);
            dblAnimation.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(dblAnimation, dialogBox);
            propertyPath = new PropertyPath("(0).(1)", new DependencyProperty[] { FrameworkElement.RenderTransformProperty, ScaleTransform.ScaleYProperty });
            Storyboard.SetTargetProperty(dblAnimation, propertyPath);
            dblAnimation.Freeze();
            storyboard.Children.Add(dblAnimation);

            storyboard.Completed += (s, e) => OnCloseDialogCompleted(dialogBox);
            storyboard.Begin();
        }

        //////////////////////////////////////////////

        private void OnCloseDialogCompleted(FrameworkElement dialogBox)
        {
            DialogBoxLayer.Children.Remove(dialogBox);

            if (DialogBoxLayer.Children.Count == 0)
            {
                parentDialogLayer.Content = null;
                parentDialogLayer.Visibility = Visibility.Collapsed;
            }
        }
    }
}
