using System;

namespace Minerva.Core
{
    /// <summary>
    /// Describes one isolated callback failure without retaining the exception,
    /// delegate, callback target, or mutable scheduled task.
    /// </summary>
    public sealed class ScheduledTaskCallbackFailure
    {
        internal ScheduledTaskCallbackFailure(
            RuntimeInstant dueTime,
            long insertionSequence,
            Type callbackTargetType,
            string callbackMethodName,
            Type exceptionType,
            string exceptionMessage)
        {
            DueTime = dueTime;
            InsertionSequence = insertionSequence;
            CallbackTargetType = callbackTargetType;
            CallbackMethodName = callbackMethodName;
            ExceptionType = exceptionType;
            ExceptionMessage = exceptionMessage;
        }

        /// <summary>Gets the failed task's exact due time.</summary>
        public RuntimeInstant DueTime { get; private set; }

        /// <summary>Gets the failed task's instance-local insertion sequence.</summary>
        public long InsertionSequence { get; private set; }

        /// <summary>Gets the callback target type, or null for a static callback.</summary>
        public Type CallbackTargetType { get; private set; }

        /// <summary>Gets the callback method name when available.</summary>
        public string CallbackMethodName { get; private set; }

        /// <summary>Gets the captured exception type without retaining the exception.</summary>
        public Type ExceptionType { get; private set; }

        /// <summary>Gets the captured exception message.</summary>
        public string ExceptionMessage { get; private set; }
    }
}
