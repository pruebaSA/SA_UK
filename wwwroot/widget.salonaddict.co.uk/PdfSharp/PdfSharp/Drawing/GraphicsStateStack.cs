namespace PdfSharp.Drawing
{
    using System;
    using System.Collections.Generic;

    internal class GraphicsStateStack
    {
        private InternalGraphicsState current;
        private Stack<InternalGraphicsState> stack = new Stack<InternalGraphicsState>();

        public GraphicsStateStack(XGraphics gfx)
        {
            this.current = new InternalGraphicsState(gfx);
        }

        public void Push(InternalGraphicsState state)
        {
            this.stack.Push(state);
            state.Pushed();
        }

        public int Restore(InternalGraphicsState state)
        {
            if (!this.stack.Contains(state))
            {
                throw new ArgumentException("State not on stack.", "state");
            }
            if (state.invalid)
            {
                throw new ArgumentException("State already restored.", "state");
            }
            int num = 1;
            InternalGraphicsState state2 = this.stack.Pop();
            state2.Popped();
            while (state2 != state)
            {
                num++;
                state.invalid = true;
                this.stack.Pop().Popped();
            }
            state.invalid = true;
            return num;
        }

        public int Count =>
            this.stack.Count;

        public InternalGraphicsState Current
        {
            get
            {
                if (this.stack.Count == 0)
                {
                    return this.current;
                }
                return this.stack.Peek();
            }
        }
    }
}

