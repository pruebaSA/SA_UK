﻿namespace System.Configuration
{
    using System;

    internal class LocationUpdates
    {
        private bool _inheritInChildApps;
        private OverrideModeSetting _overrideMode;
        private System.Configuration.SectionUpdates _sectionUpdates;

        internal LocationUpdates(OverrideModeSetting overrideMode, bool inheritInChildApps)
        {
            this._overrideMode = overrideMode;
            this._inheritInChildApps = inheritInChildApps;
            this._sectionUpdates = new System.Configuration.SectionUpdates(string.Empty);
        }

        internal void CompleteUpdates()
        {
            this._sectionUpdates.CompleteUpdates();
        }

        internal bool InheritInChildApps =>
            this._inheritInChildApps;

        internal bool IsDefault =>
            (this._overrideMode.IsDefaultForLocationTag && this._inheritInChildApps);

        internal OverrideModeSetting OverrideMode =>
            this._overrideMode;

        internal System.Configuration.SectionUpdates SectionUpdates =>
            this._sectionUpdates;
    }
}

