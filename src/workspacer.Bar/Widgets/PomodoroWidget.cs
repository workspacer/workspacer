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


        public PomodoroWidget(int pomodoroMinutes = 25, Color overtimeBackgroundColor = null, string incentive = "Click to start pomodoro!")
        {
            _pomodoroMinutes = pomodoroMinutes;
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
                        if (_timer.Enabled) {
                            StopPomodoro();
                        } else {
                            StartPomodoro();
                        }
                    }
                )
            );
        }
        
        private string GetMessage() {
            if (_timer.Enabled) {
                return _minutesLeft.ToString() + " minutes left";
            } else {
                return _incentive;
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
            _timer.Elapsed += (s,e) => {
                _minutesLeft -= 1;
                if (_minutesLeft <= 0) {
                    _blinkTimer.Start();
                }
                MarkDirty();
            };
            _timer.Stop();

            _blinkTimer = new Timer(1000);
            _blinkTimer.Elapsed += (s,e) => {
                _overtimeBackgroundOn = !_overtimeBackgroundOn;
                MarkDirty();
            };
            _blinkTimer.Stop();
        }

        private void StartPomodoro() 
        {
            _minutesLeft = _pomodoroMinutes;
            _timer.Start();
            _blinkTimer.Stop();
            MarkDirty();
        }

        private void StopPomodoro() 
        {
            _timer.Stop();
            _blinkTimer.Stop();
            MarkDirty();
        }
    }

}