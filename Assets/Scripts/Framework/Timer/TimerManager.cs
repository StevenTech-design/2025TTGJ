using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace TTGJ.Framework.Timer
{
    public class TimerManager : Singleton<TimerManager>
    {
        private Dictionary<int, TimerTask> _timerTasks = new Dictionary<int, TimerTask>();
        private int _nextTimerId = 1;

        #region Public Methods

        public int StartTimer(float duration, Action onComplete, Action<float> onUpdate = null, bool loop = false)
        {
            int timerId = GetNextTimerId();
            var timerTask = new TimerTask(timerId, duration, onComplete, onUpdate, loop);
            _timerTasks[timerId] = timerTask;

            ExecuteTimer(timerTask).Forget();

            return timerId;
        }

        public int DelayedCall(float delay, Action action)
        {
            return StartTimer(delay, action);
        }

        public int StartRepeatingTimer(float interval, Action action)
        {
            return StartTimer(interval, action, null, true);
        }

        public void StopTimer(int timerId)
        {
            if (_timerTasks.TryGetValue(timerId, out TimerTask task))
            {
                task.Cancel();
                _timerTasks.Remove(timerId);
            }
        }

        public void StopAllTimers()
        {
            foreach (var task in _timerTasks.Values)
            {
                task.Cancel();
            }
            _timerTasks.Clear();
        }

        public void PauseTimer(int timerId)
        {
            if (_timerTasks.TryGetValue(timerId, out TimerTask task))
            {
                task.Pause();
            }
        }

        public void ResumeTimer(int timerId)
        {
            if (_timerTasks.TryGetValue(timerId, out TimerTask task))
            {
                task.Resume();
            }
        }

        public bool HasTimer(int timerId)
        {
            return _timerTasks.ContainsKey(timerId);
        }

        public float GetRemainingTime(int timerId)
        {
            if (_timerTasks.TryGetValue(timerId, out TimerTask task))
            {
                return task.GetRemainingTime();
            }
            return 0f;
        }
        public float GetTimerProgress(int timerId)
        {
            if (_timerTasks.TryGetValue(timerId, out TimerTask task))
            {
                return task.GetProgress();
            }
            return 0f;
        }

        #endregion

        #region Private Methods

        private int GetNextTimerId()
        {
            return _nextTimerId++;
        }

        private async UniTaskVoid ExecuteTimer(TimerTask task)
        {
            try
            {
                do
                {
                    await task.Execute();

                    if (task.IsCompleted && !task.Loop)
                    {
                        break;
                    }

                    if (task.Loop)
                    {
                        task.Reset();
                    }

                } while (task.Loop && !task.IsCancelled);

                if (_timerTasks.ContainsKey(task.Id))
                {
                    _timerTasks.Remove(task.Id);
                }
            }
            catch (OperationCanceledException)
            {
                if (_timerTasks.ContainsKey(task.Id))
                {
                    _timerTasks.Remove(task.Id);
                }
            }
        }
    }

    #endregion

    public class TimerTask
    {
        public int Id { get; private set; }
        public float Duration { get; private set; }
        public bool Loop { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsCancelled { get; private set; }
        public bool IsPaused { get; private set; }

        private Action _onComplete;
        private Action<float> _onUpdate;
        private CancellationTokenSource _cancellationTokenSource;
        private float _elapsedTime;
        private float _pauseStartTime;

        public TimerTask(int id, float duration, Action onComplete, Action<float> onUpdate = null, bool loop = false)
        {
            Id = id;
            Duration = duration;
            Loop = loop;
            _onComplete = onComplete;
            _onUpdate = onUpdate;
            _cancellationTokenSource = new CancellationTokenSource();
            Reset();
        }

        public async UniTask Execute()
        {
            _elapsedTime = 0f;
            IsCompleted = false;

            while (_elapsedTime < Duration && !IsCancelled)
            {
                if (!IsPaused)
                {
                    _elapsedTime += Time.deltaTime;

                    _onUpdate?.Invoke(GetRemainingTime());
                }

                await UniTask.Yield(_cancellationTokenSource.Token);
            }

            if (!IsCancelled)
            {
                IsCompleted = true;
                _onComplete?.Invoke();
            }
        }

        public void Cancel()
        {
            IsCancelled = true;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        public void Pause()
        {
            if (!IsPaused)
            {
                IsPaused = true;
                _pauseStartTime = Time.time;
            }
        }

        public void Resume()
        {
            if (IsPaused)
            {
                IsPaused = false;
            }
        }

        public void Reset()
        {
            _elapsedTime = 0f;
            IsCompleted = false;
            IsPaused = false;
        }

        public float GetRemainingTime()
        {
            return Mathf.Max(0f, Duration - _elapsedTime);
        }

        public float GetProgress()
        {
            return Mathf.Clamp01(_elapsedTime / Duration);
        }
    }
}
