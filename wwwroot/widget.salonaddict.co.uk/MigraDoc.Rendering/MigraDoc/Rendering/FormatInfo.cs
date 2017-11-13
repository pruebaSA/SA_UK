namespace MigraDoc.Rendering
{
    using System;

    internal abstract class FormatInfo
    {
        protected FormatInfo()
        {
        }

        internal abstract bool EndingIsComplete { get; }

        internal abstract bool IsComplete { get; }

        internal abstract bool IsEmpty { get; }

        internal abstract bool IsEnding { get; }

        internal abstract bool IsStarting { get; }

        internal abstract bool StartingIsComplete { get; }
    }
}

