namespace System.ComponentModel.Design
{
    using System;

    public class ActiveDesignSurfaceChangedEventArgs : EventArgs
    {
        private DesignSurface _newSurface;
        private DesignSurface _oldSurface;

        public ActiveDesignSurfaceChangedEventArgs(DesignSurface oldSurface, DesignSurface newSurface)
        {
            this._oldSurface = oldSurface;
            this._newSurface = newSurface;
        }

        public DesignSurface NewSurface =>
            this._newSurface;

        public DesignSurface OldSurface =>
            this._oldSurface;
    }
}

