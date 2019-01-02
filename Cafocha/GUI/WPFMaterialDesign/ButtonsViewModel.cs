﻿using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using Cafocha.GUI.WPFMaterialDesign.Domain;

namespace Cafocha.GUI.WPFMaterialDesign
{
    public class ButtonsViewModel : INotifyPropertyChanged
    {
        private string _demoRestartCountdownText;
        private double _dismissButtonProgress;
        private bool _showDismissButton;

        public ButtonsViewModel()
        {
            var autoStartingActionCountdownStart = DateTime.Now;
            var demoRestartCountdownComplete = DateTime.Now;
            var dismissRequested = false;
            DismissComand = new AnotherCommandImplementation(_ => dismissRequested = true);
            ShowDismissButton = true;

            #region DISMISS button demo control

            //just some demo code for the DISMISS button...it's up to you to set 
            //up the progress on the button as it would be with a progress bar.
            //and then hide the button, do whatever action you want to do
            new DispatcherTimer(
                TimeSpan.FromMilliseconds(100),
                DispatcherPriority.Normal,
                (o, e) =>
                {
                    bool isComplete;
                    if (dismissRequested)
                    {
                        ShowDismissButton = false;
                        dismissRequested = false;
                        demoRestartCountdownComplete = DateTime.Now.AddSeconds(3);
                        DismissButtonProgress = 0;
                    }

                    if (ShowDismissButton)
                    {
                        var totalDuration = autoStartingActionCountdownStart.AddSeconds(5).Ticks -
                                            autoStartingActionCountdownStart.Ticks;
                        var currentDuration = DateTime.Now.Ticks - autoStartingActionCountdownStart.Ticks;
                        var autoCountdownPercentComplete = 100.0 / totalDuration * currentDuration;
                        DismissButtonProgress = autoCountdownPercentComplete;

                        if (DismissButtonProgress >= 100)
                        {
                            demoRestartCountdownComplete = DateTime.Now.AddSeconds(3);
                            ShowDismissButton = false;
                            UpdateDemoRestartCountdownText(demoRestartCountdownComplete, out isComplete);
                        }
                    }
                    else
                    {
                        UpdateDemoRestartCountdownText(demoRestartCountdownComplete, out isComplete);
                        if (isComplete)
                        {
                            autoStartingActionCountdownStart = DateTime.Now;
                            ShowDismissButton = true;
                        }
                    }
                }, Dispatcher.CurrentDispatcher);

            #endregion

            //just some demo code for the SAVE button
            SaveComand = new AnotherCommandImplementation(_ =>
            {
                if (IsSaveComplete)
                {
                    IsSaveComplete = false;
                    return;
                }

                if (SaveProgress != 0) return;

                var started = DateTime.Now;
                IsSaving = true;

                new DispatcherTimer(
                    TimeSpan.FromMilliseconds(50),
                    DispatcherPriority.Normal,
                    (o, e) =>
                    {
                        var totalDuration = started.AddSeconds(3).Ticks - started.Ticks;
                        var currentProgress = DateTime.Now.Ticks - started.Ticks;
                        var currentProgressPercent = 100.0 / totalDuration * currentProgress;

                        SaveProgress = currentProgressPercent;

                        if (SaveProgress >= 100)
                        {
                            IsSaveComplete = true;
                            IsSaving = false;
                            SaveProgress = 0;
                            ((DispatcherTimer) o).Stop();
                        }
                    }, Dispatcher.CurrentDispatcher);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }

        #region Dismiss button demo

        public ICommand DismissComand { get; }

        public bool ShowDismissButton
        {
            get => _showDismissButton;
            set => this.MutateVerbose(ref _showDismissButton, value, RaisePropertyChanged());
        }

        public double DismissButtonProgress
        {
            get => _dismissButtonProgress;
            set => this.MutateVerbose(ref _dismissButtonProgress, value, RaisePropertyChanged());
        }

        public string DemoRestartCountdownText
        {
            get => _demoRestartCountdownText;
            private set => this.MutateVerbose(ref _demoRestartCountdownText, value, RaisePropertyChanged());
        }

        private void UpdateDemoRestartCountdownText(DateTime endTime, out bool isComplete)
        {
            var span = endTime - DateTime.Now;
            var seconds = Math.Round(span.TotalSeconds < 0 ? 0 : span.TotalSeconds);
            DemoRestartCountdownText = "Demo in " + seconds;
            isComplete = seconds == 0;
        }

        #endregion

        #region floating Save button demo

        public ICommand SaveComand { get; }

        private bool _isSaving;

        public bool IsSaving
        {
            get => _isSaving;
            private set => this.MutateVerbose(ref _isSaving, value, RaisePropertyChanged());
        }

        private bool _isSaveComplete;

        public bool IsSaveComplete
        {
            get => _isSaveComplete;
            private set => this.MutateVerbose(ref _isSaveComplete, value, RaisePropertyChanged());
        }

        private double _saveProgress;

        public double SaveProgress
        {
            get => _saveProgress;
            private set => this.MutateVerbose(ref _saveProgress, value, RaisePropertyChanged());
        }

        #endregion
    }
}