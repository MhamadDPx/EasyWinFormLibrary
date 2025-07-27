using EasyWinFormLibrary.WinAppNeeds;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyWinFormLibrary.Extension
{
    /// <summary>
    /// Provides extension methods for easy task delay operations on controls.
    /// </summary>
    public static class ControlTaskDelayExtensions
    {
        // <summary>
        /// Executes a task after a delay, using the control's name as the task identifier.
        /// </summary>
        /// <param name="control">The control to associate with the task</param>
        /// <param name="task">The task to execute</param>
        /// <param name="delayMilliseconds">Delay in milliseconds</param>
        /// <returns>Task representing the delay operation</returns>
        public static async Task DelayedExecute(this Control control, TaskToDo task, int delayMilliseconds)
        {
            var taskName = $"{control.GetType().Name}_{control.Name ?? control.GetHashCode().ToString()}";
            await TaskDelayUtils.TaskDelayNamed(taskName, task, delayMilliseconds);
        }

        /// <summary>
        /// Debounces task execution for a control.
        /// </summary>
        /// <param name="control">The control to associate with the debounce</param>
        /// <param name="task">The task to execute</param>
        /// <param name="delayMilliseconds">Debounce delay in milliseconds</param>
        public static void Debounce(this Control control, TaskToDo task, int delayMilliseconds)
        {
            var debounceKey = $"{control.GetType().Name}_{control.Name ?? control.GetHashCode().ToString()}_debounce";
            TaskDelayUtils.Debounce(debounceKey, task, delayMilliseconds);
        }

        /// <summary>
        /// Throttles task execution for a control.
        /// </summary>
        /// <param name="control">The control to associate with the throttle</param>
        /// <param name="task">The task to execute</param>
        /// <param name="intervalMilliseconds">Throttle interval in milliseconds</param>
        /// <returns>True if task was executed, false if throttled</returns>
        public static bool Throttle(this Control control, TaskToDo task, int intervalMilliseconds)
        {
            var throttleKey = $"{control.GetType().Name}_{control.Name ?? control.GetHashCode().ToString()}_throttle";
            return TaskDelayUtils.Throttle(throttleKey, task, intervalMilliseconds);
        }
    }
}
