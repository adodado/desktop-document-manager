﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DocumentManager.NET.Utils.Controls.BusyAnimation
{
    //TODO! Transfer this code into viewmodel!

    /// <summary>
    /// Interaction logic for BusyAnimationControl.xaml
    /// </summary>
    public partial class BusyAnimationControl
    {
        /// <summary>
        /// Startup time in miliseconds, default is a second.
        /// </summary>
        public static readonly DependencyProperty StartupDelayProperty =
            DependencyProperty.Register(
                "StartupDelay",
                typeof(int),
                typeof(BusyAnimationControl),
                new PropertyMetadata(1000));

        /// <summary>
        /// Spinning Speed. Default is 60, that's one rotation per second.
        /// </summary>
        public static readonly DependencyProperty RotationsPerMinuteProperty =
            DependencyProperty.Register(
                "RotationsPerMinute",
                typeof(double),
                typeof(BusyAnimationControl),
                new PropertyMetadata(60.0));

        /// <summary>
        /// Timer for the Animation.
        /// </summary>
        private readonly DispatcherTimer _animationTimer;

        /// <summary>
        /// Mouse Cursor.
        /// </summary>
        private Cursor _originalCursor;

        #region Constructor

        public BusyAnimationControl()
        {
            InitializeComponent();
            _animationTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher);
        }

        #endregion

        /// <summary>
        /// Gets or sets the startup time in miliseconds, default is a second.
        /// </summary>
        public int StartupDelay
        {
            get
            {
                return (int)GetValue(StartupDelayProperty);
            }

            set
            {
                SetValue(StartupDelayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the spinning speed. Default is 60, that's one rotation per second.
        /// </summary>
        public double RotationsPerMinute
        {
            get
            {
                return (double)GetValue(RotationsPerMinuteProperty);
            }

            set
            {
                SetValue(RotationsPerMinuteProperty, value);
            }
        }

        /// <summary>
        /// Startup Delay.
        /// </summary>
        private void StartDelay()
        {
            _originalCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;

            // Startup
            _animationTimer.Interval = new TimeSpan(0, 0, 0, 0, StartupDelay);
            _animationTimer.Tick += StartSpinning;
            _animationTimer.Start();
        }

        /// <summary>
        /// Start Spinning.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event Arguments.</param>
        private void StartSpinning(object sender, EventArgs e)
        {
            _animationTimer.Stop();
            _animationTimer.Tick -= StartSpinning;

            // 60 secs per minute, 1000 millisecs per sec, 10 rotations per full circle:
            _animationTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(6000 / RotationsPerMinute));
            _animationTimer.Tick += HandleAnimationTick;
            _animationTimer.Start();
            Opacity = 1;

            Mouse.OverrideCursor = _originalCursor;
        }

        /// <summary>
        /// The control became invisible: stop spinning (animation consumes CPU).
        /// </summary>
        private void StopSpinning()
        {
            _animationTimer.Stop();
            _animationTimer.Tick -= HandleAnimationTick;
            Opacity = 0;
        }

        /// <summary>
        /// Apply a single rotation transformation.
        /// </summary>
        /// <param name="sender">Sender of the Event: the Animation Timer.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleAnimationTick(object sender, EventArgs e)
        {
            SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
        }

        /// <summary>
        /// Control was loaded: distribute circles.
        /// </summary>
        /// <param name="sender">Sender of the Event: I wish I knew.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            SetPosition(C0, 0.0);
            SetPosition(C1, 1.0);
            SetPosition(C2, 2.0);
            SetPosition(C3, 3.0);
            SetPosition(C4, 4.0);
            SetPosition(C5, 5.0);
            SetPosition(C6, 6.0);
            SetPosition(C7, 7.0);
            SetPosition(C8, 8.0);
        }

        /// <summary>
        /// Calculate position of a circle.
        /// </summary>
        /// <param name="ellipse">The circle.</param>
        /// <param name="sequence">Sequence number of the circle.</param>
        private void SetPosition(Ellipse ellipse, double sequence)
        {
            ellipse.SetValue(
                Canvas.LeftProperty,
                50.0 + (Math.Sin(Math.PI * ((0.2 * sequence) + 1)) * 50.0));

            ellipse.SetValue(
                Canvas.TopProperty,
                50 + (Math.Cos(Math.PI * ((0.2 * sequence) + 1)) * 50.0));
        }

        /// <summary>
        /// Control was unloaded: stop spinning.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            StopSpinning();
        }

        /// <summary>
        /// Visibility property was changed: start or stop spinning.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var isVisible = (bool)e.NewValue;
            
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            if (isVisible)
            {
                StartDelay();
            }
            else
            {
                StopSpinning();
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }
    }
}