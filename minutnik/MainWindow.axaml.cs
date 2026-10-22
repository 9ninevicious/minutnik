using System;
using Avalonia.Controls;
using System.Collections.Generic;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace minutnik;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer;

    private int _workDurationMinutes = 25;
    private int _breakDurationMinutes = 5;
    private int _remainingSeconds = 25 * 60;
    private int _completedCycles = 0;
    private readonly List<string> _history = new();

    private bool _isWorkSession = true;
    private bool _isRunning = false;

    private readonly IBrush _workBadgeBackground = Brush.Parse("#3B1812");
    private readonly IBrush _workBadgeBorder = Brush.Parse("#F97316");
    private readonly IBrush _workBadgeForeground = Brush.Parse("#FB923C");
    private readonly IBrush _workProgressBarBrush = Brush.Parse("#F97316");

    private readonly IBrush _breakBadgeBackground = Brush.Parse("#064E3B");
    private readonly IBrush _breakBadgeBorder = Brush.Parse("#10B981");
    private readonly IBrush _breakBadgeForeground = Brush.Parse("#34D399");
    private readonly IBrush _breakProgressBarBrush = Brush.Parse("#10B981");

    public MainWindow()
    {
        InitializeComponent();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += OnTimerTick;

        StartPauseButton.Click += OnStartPauseClick;
        ResetButton.Click += OnResetClick;

        WorkDurationInput.ValueChanged += OnWorkDurationChanged;
        BreakDurationInput.ValueChanged += OnBreakDurationChanged;

        UpdateDisplay();
        UpdateSessionUi();
    }

    private void OnStartPauseClick(object? sender, RoutedEventArgs e)
    {
        if (_isRunning)
        {
            PauseTimer();
        }
        else
        {
            StartTimer();
        }
    }

    private void StartTimer()
    {
        _isRunning = true;
        _timer.Start();

        StartPauseButton.Content = "Pauza";
        StartPauseButton.Classes.Clear();
        StartPauseButton.Classes.Add("action-btn");
        StartPauseButton.Classes.Add("pause-btn");

        StatusMessageText.Text = _isWorkSession
            ? "Trwa praca (skupienie)..."
            : "Trwa przerwa (regeneracja)...";
    }

    private void PauseTimer()
    {
        _isRunning = false;
        _timer.Stop();

        StartPauseButton.Content = "Start";
        StartPauseButton.Classes.Clear();
        StartPauseButton.Classes.Add("action-btn");
        StartPauseButton.Classes.Add("start-btn");

        StatusMessageText.Text = "Wstrzymano";
    }

    private void OnResetClick(object? sender, RoutedEventArgs e)
    {
        _isRunning = false;
        _timer.Stop();

        _isWorkSession = true;
        _remainingSeconds = _workDurationMinutes * 60;
        _completedCycles = 0;

        StartPauseButton.Content = "Start";
        StartPauseButton.Classes.Clear();
        StartPauseButton.Classes.Add("action-btn");
        StartPauseButton.Classes.Add("start-btn");

        StatusMessageText.Text = "Zresetowano minutnik – gotowy do startu";

        UpdateDisplay();
        UpdateSessionUi();
    }

    private void OnWorkDurationChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (e.NewValue.HasValue)
        {
            _workDurationMinutes = (int)Math.Clamp(Math.Round(e.NewValue.Value), 1, 180);

            if (_isWorkSession && !_isRunning)
            {
                _remainingSeconds = _workDurationMinutes * 60;
                UpdateDisplay();
            }
        }
    }

    private void OnBreakDurationChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (e.NewValue.HasValue)
        {
            _breakDurationMinutes = (int)Math.Clamp(Math.Round(e.NewValue.Value), 1, 120);

            if (!_isWorkSession && !_isRunning)
            {
                _remainingSeconds = _breakDurationMinutes * 60;
                UpdateDisplay();
            }
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (!_isRunning)
        {
            return;
        }

        if (_remainingSeconds > 0)
        {
            _remainingSeconds--;
            UpdateDisplay();
        }

        if (_remainingSeconds <= 0)
        {
            HandleSessionCompletion();
        }
    }

    private void HandleSessionCompletion()
    {
        try
        {
            Console.Beep();
        }
        catch
        {
            // Ignored if sound hardware or current platform is not supported
        }

        if (_isWorkSession)
        {
            // After work ends, break starts automatically! ("Po skończonej pracy przerwa ma ruszyć sama")
            _isWorkSession = false;
            _remainingSeconds = _breakDurationMinutes * 60;

            UpdateSessionUi();
            UpdateDisplay();

            StatusMessageText.Text = "Praca zakończona! Trwa przerwa...";
            // Timer remains running seamlessly
        }
        else
        {
            // After break ends, cycle is completed
            _completedCycles++;
            _history.Add($"{DateTime.Now:HH:mm} — cykl {_completedCycles}");
    HistoryList.ItemsSource = _history.ToArray();
            _isWorkSession = true;
            _remainingSeconds = _workDurationMinutes * 60;

            _isRunning = false;
            _timer.Stop();

            StartPauseButton.Content = "Start";
            StartPauseButton.Classes.Clear();
            StartPauseButton.Classes.Add("action-btn");
            StartPauseButton.Classes.Add("start-btn");

            StatusMessageText.Text = $"Cykl {_completedCycles} ukończony! Kliknij Start, aby rozpocząć kolejny.";

            UpdateSessionUi();
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        int minutes = _remainingSeconds / 60;
        int seconds = _remainingSeconds % 60;

        TimerDisplay.Text = $"{minutes:D2}:{seconds:D2}";
        CyclesCountDisplay.Text = _completedCycles.ToString();

        int totalSeconds = (_isWorkSession ? _workDurationMinutes : _breakDurationMinutes) * 60;
        if (totalSeconds > 0)
        {
            double progress = 100.0 * (1.0 - (double)_remainingSeconds / totalSeconds);
            SessionProgressBar.Value = Math.Clamp(progress, 0.0, 100.0);
        }
        else
        {
            SessionProgressBar.Value = 0;
        }
    }

    private void UpdateSessionUi()
    {
        if (_isWorkSession)
        {
            SessionBadgeText.Text = "PRACA";
            SessionBadgeBorder.Background = _workBadgeBackground;
            SessionBadgeBorder.BorderBrush = _workBadgeBorder;
            SessionBadgeText.Foreground = _workBadgeForeground;
            SessionProgressBar.Foreground = _workProgressBarBrush;
        }
        else
        {
            SessionBadgeText.Text = "PRZERWA";
            SessionBadgeBorder.Background = _breakBadgeBackground;
            SessionBadgeBorder.BorderBrush = _breakBadgeBorder;
            SessionBadgeText.Foreground = _breakBadgeForeground;
            SessionProgressBar.Foreground = _breakProgressBarBrush;
        }
    }
}