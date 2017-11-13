namespace Microsoft.Contracts
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    internal static class Contract
    {
        [Conditional("DEBUG"), Pure]
        public static void Assert(bool b)
        {
        }

        [Pure, Conditional("DEBUG")]
        public static void Assert(bool b, string message)
        {
        }

        [Conditional("USE_SPECSHARP_ASSEMBLY_REWRITER"), Pure]
        public static void AssertOnException<E>(bool b) where E: Exception
        {
            string text1 = "This method will be modified to the following after rewriting:" + "if (!b) throw new AssertionException();";
        }

        [Conditional("USE_SPECSHARP_ASSEMBLY_REWRITER"), Pure]
        public static void AssertOnReturn(bool b)
        {
            string text1 = "This method will be modified to the following after rewriting:" + "if (!b) throw new AssertionException();";
        }

        [Conditional("DEBUG"), Pure]
        public static void Assume(bool b)
        {
        }

        [Pure, Conditional("DEBUG")]
        public static void Assume(bool b, string message)
        {
            if (!b)
            {
                throw new AssumptionException(message);
            }
        }

        [Conditional("DEBUG"), Pure]
        public static void DebugRequires(bool b)
        {
        }

        [Pure, Conditional("USE_SPECSHARP_ASSEMBLY_REWRITER")]
        public static void Ensures(bool b)
        {
            string text1 = "This method will be modified to the following after rewriting:" + "if (!b) throw new PostConditionException();";
        }

        public static bool Exists(int lo, int hi, Predicate<int> p)
        {
            Requires(lo <= hi);
            Requires(p != null);
            for (int i = lo; i < hi; i++)
            {
                if (p(i))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ForAll(int lo, int hi, Predicate<int> p)
        {
            Requires(lo <= hi);
            Requires(p != null);
            for (int i = lo; i < hi; i++)
            {
                if (!p(i))
                {
                    return false;
                }
            }
            return true;
        }

        [Pure, Conditional("USE_SPECSHARP_ASSEMBLY_REWRITER")]
        public static void Invariant(bool b)
        {
            string text1 = "This method will be modified to the following after rewriting:" + "if (!b) throw new InvariantException();";
        }

        [Pure]
        public static T Old<T>(T t) => 
            t;

        [Pure]
        public static T Parameter<T>(out T t)
        {
            t = default(T);
            return t;
        }

        [Pure]
        public static void Requires(bool b)
        {
            if (!b)
            {
                throw new PreconditionException();
            }
        }

        [Pure]
        public static void Requires(Exception x)
        {
            if (x != null)
            {
                throw x;
            }
        }

        [Pure]
        public static T Result<T>() => 
            default(T);

        [Pure]
        public static void RewriterEnsures(bool b)
        {
            if (!b)
            {
                throw new PostconditionException();
            }
        }

        [Pure]
        public static void RewriterInvariant(bool b)
        {
            if (!b)
            {
                throw new InvariantException();
            }
        }

        [Pure, Conditional("USE_SPECSHARP_ASSEMBLY_REWRITER")]
        public static void Throws<E>() where E: Exception
        {
        }

        [Pure, Conditional("USE_SPECSHARP_ASSEMBLY_REWRITER")]
        public static void ThrowsEnsures<E>(bool b) where E: Exception
        {
            string text1 = "This method will be modified to the following after rewriting:" + "if (!b) throw new PostconditionException();";
        }

        [Serializable]
        public sealed class AssertionException : Exception
        {
            public AssertionException()
            {
            }

            public AssertionException(string s) : base(s)
            {
            }

            private AssertionException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public AssertionException(string s, Exception inner) : base(s, inner)
            {
            }
        }

        [Serializable]
        public sealed class AssumptionException : Exception
        {
            public AssumptionException()
            {
            }

            public AssumptionException(string s) : base(s)
            {
            }

            private AssumptionException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public AssumptionException(string s, Exception inner) : base(s, inner)
            {
            }
        }

        [Serializable]
        public sealed class InvariantException : Exception
        {
            public InvariantException()
            {
            }

            public InvariantException(string s) : base(s)
            {
            }

            private InvariantException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public InvariantException(string s, Exception inner) : base(s, inner)
            {
            }
        }

        [Serializable]
        public sealed class PostconditionException : Exception
        {
            public PostconditionException() : this("Postcondition failed.")
            {
            }

            public PostconditionException(string s) : base(s)
            {
            }

            private PostconditionException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public PostconditionException(string s, Exception inner) : base(s, inner)
            {
            }
        }

        [Serializable]
        public sealed class PreconditionException : Exception
        {
            public PreconditionException() : this("Precondition failed.")
            {
            }

            public PreconditionException(string s) : base(s)
            {
            }

            private PreconditionException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public PreconditionException(string s, Exception inner) : base(s, inner)
            {
            }
        }
    }
}

