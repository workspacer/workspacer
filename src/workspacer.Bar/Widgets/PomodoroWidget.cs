using System;
using System.Timers;

namespace workspacer.Bar.Widgets
{

    public class PomodoroWidget : BarWidgetBase
    {
        private Timer _timer;
        private Timer _blinkTimer;
        private int _pomodoroMinutes = 0;
        private int _minutesLeft = 0;
        private Boolean _overtimeBackgroundOn;
        private Color _overtimeBackgroundColor;
        private string _incentive;

        private Timer _pauseTimer;
        private int _pauseMinutes = 0;
        private bool _isPaused = false;
        private int _pauseTimeLeft = 0;

    private enum PomodoroState
    {
        Inactive,
        Working,
        Paused
    }

private PomodoroState _currentState = PomodoroState.Inactive;
        public PomodoroWidget(int pomodoroMinutes = 25,int pauseMinutes = 5, Color overtimeBackgroundColor = null, string incentive = "Pomodoro")
        {
            _pomodoroMinutes = pomodoroMinutes;
            _pauseMinutes = pauseMinutes;
            _incentive = incentive;

            if (overtimeBackgroundColor == null) {
                _overtimeBackgroundColor = Color.Red;
            } else {
                _overtimeBackgroundColor = overtimeBackgroundColor;
            }
        }

        public override IBarWidgetPart[] GetParts()
        {
            return Parts(
                Part(
                    text: GetMessage(),
                    back: GetBackgroundColor(),
                    partClicked: () => {
                       if (_currentState == PomodoroState.Inactive)
                       {
                        StartPomodoro();
                       }
                       else
                       {
                        StopPomodoro();
                       }
                    }
                )
            );
        }
        
        private string GetMessage() {
 
            switch (_currentState)
            {
                case PomodoroState.Working:
                    return $"Work {_minutesLeft} mins left";
                case PomodoroState.Paused:
                    return $"Pause {_minutesLeft} mins left";
                case PomodoroState.Inactive:
                    return _incentive;
                default:
                    return "Unknown State";
            }
        }

        private Color GetBackgroundColor() {
            if (_minutesLeft <= 0 && _timer.Enabled && _overtimeBackgroundOn) {
                return _overtimeBackgroundColor;
            } else {
                return null;
            }
        }

        public override void Initialize()
        {
            _timer = new Timer(60000);
            _timer.Elapsed += TimerElapsed;
            _currentState = PomodoroState.Inactive;
        
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _minutesLeft--;

             if (_minutesLeft <= 0)
             {
                if (_currentState == PomodoroState.Working)
                {
                    StartPause();
                }else if (_currentState == PomodoroState.Paused)
                {
                    StartPomodoro();
                }
             }
             MarkDirty();
    
        }
        private void StartPomodoro() 
        {
            _currentState = PomodoroState.Working;
            _minutesLeft = _pomodoroMinutes;
            _timer.Start();
  
        }

        private void StartPause()
        {
            _currentState = PomodoroState.Paused;
            _minutesLeft = _pauseMinutes;
            _timer.Start();

        }
        private void StopPomodoro() 
        {
            _currentState = PomodoroState.Inactive;
            _timer.Stop();
        }
    }

}