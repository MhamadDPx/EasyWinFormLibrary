using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyWinFormLibrary.WinAppNeeds
{
        /// <summary>
        /// Delegate for tasks that can be executed with delay and cancellation support.
        /// </summary>
        public delegate void TaskToDo();

        /// <summary>
        /// Delegate for tasks that can be executed with delay, cancellation support, and parameters.
        /// </summary>
        /// <param name="parameter">Parameter to pass to the task</param>
        public delegate void ParameterizedTaskToDo(object parameter);

        /// <summary>
        /// Delegate for async tasks that can be executed with delay and cancellation support.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the async operation</param>
        /// <returns>Task representing the async operation</returns>
        public delegate Task AsyncTaskToDo(CancellationToken cancellationToken);

        /// <summary>
        /// Provides comprehensive task delay and debouncing utilities with cancellation support,
        /// thread-safe operations, and various execution patterns. Optimized for .NET Framework 4.8
        /// and WinForms applications with proper UI thread marshaling.
        /// </summary>
        public static class TaskDelayUtils
        {
            #region Private Fields

            /// <summary>
            /// Global cancellation token source for simple delay operations
            /// </summary>
            private static CancellationTokenSource _globalTokenSource;

            /// <summary>
            /// Thread-safe dictionary to store named cancellation token sources
            /// </summary>
            private static readonly ConcurrentDictionary<string, CancellationTokenSource> _namedTokenSources
                = new ConcurrentDictionary<string, CancellationTokenSource>();

            /// <summary>
            /// Thread-safe dictionary for debounce operations
            /// </summary>
            private static readonly ConcurrentDictionary<string, CancellationTokenSource> _debounceTokens
                = new ConcurrentDictionary<string, CancellationTokenSource>();

            /// <summary>
            /// Thread-safe dictionary for throttle operations
            /// </summary>
            private static readonly ConcurrentDictionary<string, DateTime> _throttleLastExecuted
                = new ConcurrentDictionary<string, DateTime>();

            /// <summary>
            /// Object for thread-safe operations on global token source
            /// </summary>
            private static readonly object _globalTokenLock = new object();

            #endregion

            #region Enhanced Original Method

            /// <summary>
            /// Executes a task after a specified delay, cancelling any previous pending execution.
            /// Enhanced version of the original method with improved error handling and thread safety.
            /// </summary>
            /// <param name="task">The task to execute after delay</param>
            /// <param name="delayMilliseconds">Delay in milliseconds before executing the task</param>
            /// <param name="executeOnUIThread">Whether to execute the task on the UI thread (default: true for WinForms)</param>
            /// <exception cref="ArgumentNullException">Thrown when task is null</exception>
            /// <exception cref="ArgumentException">Thrown when delayMilliseconds is negative</exception>
            public static async void TaskDelay(TaskToDo task, int delayMilliseconds, bool executeOnUIThread = true)
            {
                if (task == null)
                    throw new ArgumentNullException(nameof(task), "Task cannot be null");
                if (delayMilliseconds < 0)
                    throw new ArgumentException("Delay cannot be negative", nameof(delayMilliseconds));

                CancellationToken token;

                lock (_globalTokenLock)
                {
                    _globalTokenSource?.Cancel();
                    _globalTokenSource?.Dispose();
                    _globalTokenSource = new CancellationTokenSource();
                    token = _globalTokenSource.Token;
                }

                try
                {
                    await Task.Delay(delayMilliseconds, token).ConfigureAwait(false);

                    if (!token.IsCancellationRequested)
                    {
                        if (executeOnUIThread && InvokeRequired())
                        {
                            SafeInvoke(() => task());
                        }
                        else
                        {
                            task();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation occurs - no action needed
                }
                catch (Exception ex)
                {
                    // Log error or handle as needed
                    OnTaskExecutionError?.Invoke(ex, "TaskDelay");
                }
            }

            #endregion

            #region Named Task Delay Operations

            /// <summary>
            /// Executes a named task after a specified delay, allowing multiple independent delay operations.
            /// Each named operation can be cancelled independently.
            /// </summary>
            /// <param name="taskName">Unique name for this task operation</param>
            /// <param name="task">The task to execute after delay</param>
            /// <param name="delayMilliseconds">Delay in milliseconds before executing the task</param>
            /// <param name="executeOnUIThread">Whether to execute the task on the UI thread</param>
            /// <returns>Task representing the delay operation</returns>
            public static async Task TaskDelayNamed(string taskName, TaskToDo task, int delayMilliseconds, bool executeOnUIThread = true)
            {
                if (string.IsNullOrWhiteSpace(taskName))
                    throw new ArgumentException("Task name cannot be null or empty", nameof(taskName));
                if (task == null)
                    throw new ArgumentNullException(nameof(task), "Task cannot be null");
                if (delayMilliseconds < 0)
                    throw new ArgumentException("Delay cannot be negative", nameof(delayMilliseconds));

                // Cancel any existing task with this name
                CancelNamedTask(taskName);

                var tokenSource = new CancellationTokenSource();
                _namedTokenSources.TryAdd(taskName, tokenSource);

                try
                {
                    await Task.Delay(delayMilliseconds, tokenSource.Token).ConfigureAwait(false);

                    if (!tokenSource.Token.IsCancellationRequested)
                    {
                        if (executeOnUIThread && InvokeRequired())
                        {
                            SafeInvoke(() => task());
                        }
                        else
                        {
                            task();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation occurs
                }
                catch (Exception ex)
                {
                    OnTaskExecutionError?.Invoke(ex, $"TaskDelayNamed: {taskName}");
                }
                finally
                {
                    // Cleanup
                    _namedTokenSources.TryRemove(taskName, out var removedSource);
                    removedSource?.Dispose();
                }
            }

            /// <summary>
            /// Cancels a specific named task if it's currently pending.
            /// </summary>
            /// <param name="taskName">Name of the task to cancel</param>
            /// <returns>True if a task was cancelled, false if no task was found</returns>
            public static bool CancelNamedTask(string taskName)
            {
                if (string.IsNullOrWhiteSpace(taskName))
                    return false;

                if (_namedTokenSources.TryRemove(taskName, out var tokenSource))
                {
                    tokenSource.Cancel();
                    tokenSource.Dispose();
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Cancels all named tasks that are currently pending.
            /// </summary>
            /// <returns>Number of tasks that were cancelled</returns>
            public static int CancelAllNamedTasks()
            {
                var count = 0;
                var taskNames = new List<string>(_namedTokenSources.Keys);

                foreach (var taskName in taskNames)
                {
                    if (CancelNamedTask(taskName))
                        count++;
                }

                return count;
            }

            #endregion

            #region Debouncing Operations

            /// <summary>
            /// Debounces task execution - only executes the task after the specified delay has passed
            /// without any new calls to the same debounce key.
            /// </summary>
            /// <param name="debounceKey">Unique key for this debounce operation</param>
            /// <param name="task">The task to execute</param>
            /// <param name="delayMilliseconds">Debounce delay in milliseconds</param>
            /// <param name="executeOnUIThread">Whether to execute on UI thread</param>
            public static void Debounce(string debounceKey, TaskToDo task, int delayMilliseconds, bool executeOnUIThread = true)
            {
                if (string.IsNullOrWhiteSpace(debounceKey))
                    throw new ArgumentException("Debounce key cannot be null or empty", nameof(debounceKey));
                if (task == null)
                    throw new ArgumentNullException(nameof(task));
                if (delayMilliseconds < 0)
                    throw new ArgumentException("Delay cannot be negative", nameof(delayMilliseconds));

                // Cancel any existing debounce operation for this key
                if (_debounceTokens.TryRemove(debounceKey, out var existingTokenSource))
                {
                    existingTokenSource.Cancel();
                    existingTokenSource.Dispose();
                }

                var tokenSource = new CancellationTokenSource();
                _debounceTokens.TryAdd(debounceKey, tokenSource);

                // Start the debounce operation as a fire-and-forget task
                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(delayMilliseconds, tokenSource.Token).ConfigureAwait(false);

                        if (!tokenSource.Token.IsCancellationRequested)
                        {
                            if (executeOnUIThread && InvokeRequired())
                            {
                                SafeInvoke(() => task());
                            }
                            else
                            {
                                task();
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected when debounce is reset
                    }
                    catch (Exception ex)
                    {
                        OnTaskExecutionError?.Invoke(ex, $"Debounce: {debounceKey}");
                    }
                    finally
                    {
                        _debounceTokens.TryRemove(debounceKey, out var removedSource);
                        removedSource?.Dispose();
                    }
                });
            }

            /// <summary>
            /// Debounces task execution asynchronously - only executes the task after the specified delay has passed
            /// without any new calls to the same debounce key. Use this version when you need to await the debounce operation.
            /// </summary>
            /// <param name="debounceKey">Unique key for this debounce operation</param>
            /// <param name="task">The task to execute</param>
            /// <param name="delayMilliseconds">Debounce delay in milliseconds</param>
            /// <param name="executeOnUIThread">Whether to execute on UI thread</param>
            /// <returns>Task representing the debounce operation</returns>
            public static async Task DebounceAsync(string debounceKey, TaskToDo task, int delayMilliseconds, bool executeOnUIThread = true)
            {
                if (string.IsNullOrWhiteSpace(debounceKey))
                    throw new ArgumentException("Debounce key cannot be null or empty", nameof(debounceKey));
                if (task == null)
                    throw new ArgumentNullException(nameof(task));
                if (delayMilliseconds < 0)
                    throw new ArgumentException("Delay cannot be negative", nameof(delayMilliseconds));

                // Cancel any existing debounce operation for this key
                if (_debounceTokens.TryRemove(debounceKey, out var existingTokenSource))
                {
                    existingTokenSource.Cancel();
                    existingTokenSource.Dispose();
                }

                var tokenSource = new CancellationTokenSource();
                _debounceTokens.TryAdd(debounceKey, tokenSource);

                try
                {
                    await Task.Delay(delayMilliseconds, tokenSource.Token).ConfigureAwait(false);

                    if (!tokenSource.Token.IsCancellationRequested)
                    {
                        if (executeOnUIThread && InvokeRequired())
                        {
                            SafeInvoke(() => task());
                        }
                        else
                        {
                            task();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected when debounce is reset
                }
                catch (Exception ex)
                {
                    OnTaskExecutionError?.Invoke(ex, $"DebounceAsync: {debounceKey}");
                }
                finally
                {
                    _debounceTokens.TryRemove(debounceKey, out var removedSource);
                    removedSource?.Dispose();
                }
            }

            /// <summary>
            /// Debounces parameterized task execution.
            /// </summary>
            /// <param name="debounceKey">Unique key for this debounce operation</param>
            /// <param name="task">The parameterized task to execute</param>
            /// <param name="parameter">Parameter to pass to the task</param>
            /// <param name="delayMilliseconds">Debounce delay in milliseconds</param>
            /// <param name="executeOnUIThread">Whether to execute on UI thread</param>
            public static void DebounceParameterized(string debounceKey, ParameterizedTaskToDo task,
                object parameter, int delayMilliseconds, bool executeOnUIThread = true)
            {
                Debounce(debounceKey, () => task(parameter), delayMilliseconds, executeOnUIThread);
            }

            #endregion

            #region Throttling Operations

            /// <summary>
            /// Throttles task execution - ensures the task is not executed more frequently than the specified interval.
            /// </summary>
            /// <param name="throttleKey">Unique key for this throttle operation</param>
            /// <param name="task">The task to execute</param>
            /// <param name="intervalMilliseconds">Minimum interval between executions in milliseconds</param>
            /// <param name="executeOnUIThread">Whether to execute on UI thread</param>
            /// <returns>True if task was executed, false if throttled</returns>
            public static bool Throttle(string throttleKey, TaskToDo task, int intervalMilliseconds, bool executeOnUIThread = true)
            {
                if (string.IsNullOrWhiteSpace(throttleKey))
                    throw new ArgumentException("Throttle key cannot be null or empty", nameof(throttleKey));
                if (task == null)
                    throw new ArgumentNullException(nameof(task));
                if (intervalMilliseconds < 0)
                    throw new ArgumentException("Interval cannot be negative", nameof(intervalMilliseconds));

                var now = DateTime.UtcNow;
                var lastExecuted = _throttleLastExecuted.GetOrAdd(throttleKey, DateTime.MinValue);

                if ((now - lastExecuted).TotalMilliseconds >= intervalMilliseconds)
                {
                    _throttleLastExecuted[throttleKey] = now;

                    try
                    {
                        if (executeOnUIThread && InvokeRequired())
                        {
                            SafeInvoke(() => task());
                        }
                        else
                        {
                            task();
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        OnTaskExecutionError?.Invoke(ex, $"Throttle: {throttleKey}");
                        return false;
                    }
                }

                return false;
            }

            /// <summary>
            /// Resets the throttle timer for a specific key, allowing immediate execution on next call.
            /// </summary>
            /// <param name="throttleKey">Throttle key to reset</param>
            /// <returns>True if throttle was reset, false if key was not found</returns>
            public static bool ResetThrottle(string throttleKey)
            {
                if (string.IsNullOrWhiteSpace(throttleKey))
                    return false;

                return _throttleLastExecuted.TryRemove(throttleKey, out _);
            }

            #endregion

            #region Async Task Support

            /// <summary>
            /// Executes an async task after a specified delay with cancellation support.
            /// </summary>
            /// <param name="asyncTask">The async task to execute</param>
            /// <param name="delayMilliseconds">Delay in milliseconds before executing the task</param>
            /// <param name="executeOnUIThread">Whether to execute on UI thread</param>
            /// <returns>Task representing the delayed async operation</returns>
            public static async Task TaskDelayAsync(AsyncTaskToDo asyncTask, int delayMilliseconds, bool executeOnUIThread = true)
            {
                if (asyncTask == null)
                    throw new ArgumentNullException(nameof(asyncTask));
                if (delayMilliseconds < 0)
                    throw new ArgumentException("Delay cannot be negative", nameof(delayMilliseconds));

                CancellationToken token;

                lock (_globalTokenLock)
                {
                    _globalTokenSource?.Cancel();
                    _globalTokenSource?.Dispose();
                    _globalTokenSource = new CancellationTokenSource();
                    token = _globalTokenSource.Token;
                }

                try
                {
                    await Task.Delay(delayMilliseconds, token).ConfigureAwait(false);

                    if (!token.IsCancellationRequested)
                    {
                        if (executeOnUIThread && InvokeRequired())
                        {
                            await SafeInvokeAsync(async () => await asyncTask(token));
                        }
                        else
                        {
                            await asyncTask(token);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation occurs
                }
                catch (Exception ex)
                {
                    OnTaskExecutionError?.Invoke(ex, "TaskDelayAsync");
                }
            }

            #endregion

            #region Utility Methods

            /// <summary>
            /// Cancels the global task delay operation if one is pending.
            /// </summary>
            /// <returns>True if a task was cancelled, false if no task was pending</returns>
            public static bool CancelGlobalTask()
            {
                lock (_globalTokenLock)
                {
                    if (_globalTokenSource != null && !_globalTokenSource.IsCancellationRequested)
                    {
                        _globalTokenSource.Cancel();
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>
            /// Cancels all pending operations (global, named, and debounce).
            /// </summary>
            /// <returns>Total number of operations cancelled</returns>
            public static int CancelAllOperations()
            {
                var count = 0;

                // Cancel global task
                if (CancelGlobalTask())
                    count++;

                // Cancel all named tasks
                count += CancelAllNamedTasks();

                // Cancel all debounce operations
                var debounceKeys = new List<string>(_debounceTokens.Keys);
                foreach (var key in debounceKeys)
                {
                    if (_debounceTokens.TryRemove(key, out var tokenSource))
                    {
                        tokenSource.Cancel();
                        tokenSource.Dispose();
                        count++;
                    }
                }

                return count;
            }

            /// <summary>
            /// Gets the count of currently pending operations.
            /// </summary>
            /// <returns>Number of pending operations</returns>
            public static int GetPendingOperationsCount()
            {
                var count = 0;

                lock (_globalTokenLock)
                {
                    if (_globalTokenSource != null && !_globalTokenSource.IsCancellationRequested)
                        count++;
                }

                count += _namedTokenSources.Count;
                count += _debounceTokens.Count;

                return count;
            }

            /// <summary>
            /// Checks if invoke is required for UI thread operations.
            /// </summary>
            private static bool InvokeRequired()
            {
                return Application.OpenForms.Count > 0 &&
                       Application.OpenForms[0].InvokeRequired;
            }

            /// <summary>
            /// Safely invokes an action on the UI thread.
            /// </summary>
            private static void SafeInvoke(Action action)
            {
                try
                {
                    if (Application.OpenForms.Count > 0)
                    {
                        var form = Application.OpenForms[0];
                        if (form.IsHandleCreated && !form.IsDisposed)
                        {
                            form.Invoke(action);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnTaskExecutionError?.Invoke(ex, "SafeInvoke");
                }
            }

            /// <summary>
            /// Safely invokes an async action on the UI thread.
            /// </summary>
            private static async Task SafeInvokeAsync(Func<Task> asyncAction)
            {
                try
                {
                    if (Application.OpenForms.Count > 0)
                    {
                        var form = Application.OpenForms[0];
                        if (form.IsHandleCreated && !form.IsDisposed)
                        {
                            await Task.Run(() =>
                            {
                                form.Invoke(new Action(async () => await asyncAction()));
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnTaskExecutionError?.Invoke(ex, "SafeInvokeAsync");
                }
            }

            #endregion

            #region Events

            /// <summary>
            /// Event fired when a task execution error occurs.
            /// </summary>
            public static event Action<Exception, string> OnTaskExecutionError;

            #endregion

            #region Cleanup

            /// <summary>
            /// Disposes all resources and cancels all pending operations.
            /// Call this when shutting down the application.
            /// </summary>
            public static void Cleanup()
            {
                CancelAllOperations();

                lock (_globalTokenLock)
                {
                    _globalTokenSource?.Dispose();
                    _globalTokenSource = null;
                }

                _throttleLastExecuted.Clear();
            }

            #endregion
        }
}