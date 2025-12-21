using System;
using System.Timers;

namespace DATMOS.WinApp.Models
{
    public class TimerModel : IDisposable
    {
        private System.Timers.Timer _timer;
        private TimeSpan _remainingTime;
        private DateTime _startTime;
        
        public event EventHandler<TimeSpan>? TimeUpdated;
        public event EventHandler? TimeExpired;
        
        public TimeSpan RemainingTime
        {
            get => _remainingTime;
            set
            {
                _remainingTime = value;
                TimeUpdated?.Invoke(this, _remainingTime);
            }
        }
        
        public bool IsRunning { get; private set; }
        public TimeSpan TotalTime { get; set; }
        
        public TimerModel(TimeSpan totalTime)
        {
            TotalTime = totalTime;
            RemainingTime = totalTime;
            _timer = new System.Timers.Timer(1000); // 1 second interval
            _timer.Elapsed += OnTimerElapsed;
        }
        
        public void Start()
        {
            if (!IsRunning)
            {
                _startTime = DateTime.Now;
                _timer.Start();
                IsRunning = true;
            }
        }
        
        public void Pause()
        {
            if (IsRunning)
            {
                _timer.Stop();
                IsRunning = false;
            }
        }
        
        public void Reset()
        {
            _timer.Stop();
            IsRunning = false;
            RemainingTime = TotalTime;
        }
        
        public void AddTime(TimeSpan timeToAdd)
        {
            RemainingTime = RemainingTime.Add(timeToAdd);
        }
        
        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            var elapsed = DateTime.Now - _startTime;
            RemainingTime = TotalTime - elapsed;
            
            if (RemainingTime <= TimeSpan.Zero)
            {
                RemainingTime = TimeSpan.Zero;
                _timer.Stop();
                IsRunning = false;
                TimeExpired?.Invoke(this, EventArgs.Empty);
            }
        }
        
        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }
        
        public string GetFormattedTime()
        {
            return RemainingTime.ToString(@"hh\:mm\:ss");
        }
        
        public bool IsWarningTime => RemainingTime <= TimeSpan.FromMinutes(5);
        public bool IsCriticalTime => RemainingTime <= TimeSpan.FromMinutes(1);
    }
}
