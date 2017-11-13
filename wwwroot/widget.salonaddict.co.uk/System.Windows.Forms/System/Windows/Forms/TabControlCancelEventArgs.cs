﻿namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    public class TabControlCancelEventArgs : CancelEventArgs
    {
        private TabControlAction action;
        private System.Windows.Forms.TabPage tabPage;
        private int tabPageIndex;

        public TabControlCancelEventArgs(System.Windows.Forms.TabPage tabPage, int tabPageIndex, bool cancel, TabControlAction action) : base(cancel)
        {
            this.tabPage = tabPage;
            this.tabPageIndex = tabPageIndex;
            this.action = action;
        }

        public TabControlAction Action =>
            this.action;

        public System.Windows.Forms.TabPage TabPage =>
            this.tabPage;

        public int TabPageIndex =>
            this.tabPageIndex;
    }
}

