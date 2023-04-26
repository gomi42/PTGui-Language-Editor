//
// Author:
//   Michael Göricke
//
// Copyright (c) 2023
//
// This file is part of PTGui Language Editor.
//
// The PTGui Language Editor is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see<http://www.gnu.org/licenses/>.

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
