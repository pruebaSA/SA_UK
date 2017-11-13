namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(NoBot), "NoBot.NoBot.ico"), DefaultEvent("GenerateChallengeAndResponse"), Designer("AjaxControlToolkit.NoBotDesigner, AjaxControlToolkit")]
    public class NoBot : WebControl, INamingContainer
    {
        private int _cutoffMaximumInstances;
        private int _cutoffWindowSeconds;
        private NoBotExtender _extender;
        private static SortedList<DateTime, string> _pastAddresses = new SortedList<DateTime, string>();
        private int _responseMinimumDelaySeconds;
        private NoBotState _state;

        public event EventHandler<NoBotEventArgs> GenerateChallengeAndResponse;

        public NoBot() : base(HtmlTextWriterTag.Div)
        {
            this._responseMinimumDelaySeconds = 2;
            this._cutoffWindowSeconds = 60;
            this._cutoffMaximumInstances = 5;
            this._state = NoBotState.InvalidUnknown;
        }

        private void CheckResponseAndStoreState()
        {
            if (NoBotState.InvalidUnknown == this._state)
            {
                try
                {
                    if (!this.Page.IsPostBack)
                    {
                        this._state = NoBotState.Valid;
                    }
                    else
                    {
                        DateTime time = (DateTime) this.ViewState[this.ResponseTimeKey];
                        DateTime utcNow = DateTime.UtcNow;
                        if (utcNow < time)
                        {
                            this._state = NoBotState.InvalidResponseTooSoon;
                        }
                        else
                        {
                            lock (_pastAddresses)
                            {
                                string userHostAddress = this.Page.Request.UserHostAddress;
                                DateTime key = utcNow;
                                while (_pastAddresses.ContainsKey(key))
                                {
                                    key = key.AddTicks(1L);
                                }
                                _pastAddresses.Add(key, userHostAddress);
                                DateTime time4 = utcNow.AddSeconds((double) -this._cutoffWindowSeconds);
                                int num = 0;
                                foreach (DateTime time5 in _pastAddresses.Keys)
                                {
                                    if (time5 >= time4)
                                    {
                                        break;
                                    }
                                    num++;
                                }
                                while (0 < num)
                                {
                                    _pastAddresses.RemoveAt(0);
                                    num--;
                                }
                                int num2 = 0;
                                foreach (string str2 in _pastAddresses.Values)
                                {
                                    if (userHostAddress == str2)
                                    {
                                        num2++;
                                    }
                                }
                                if (this._cutoffMaximumInstances < num2)
                                {
                                    this._state = NoBotState.InvalidAddressTooActive;
                                    return;
                                }
                            }
                            string name = (string) this.ViewState[this.SessionKeyKey];
                            string str4 = (string) this.Page.Session[name];
                            this.Page.Session.Remove(name);
                            if (str4 != this._extender.ClientState)
                            {
                                this._state = NoBotState.InvalidBadResponse;
                            }
                            else
                            {
                                this._state = NoBotState.Valid;
                            }
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    this._state = NoBotState.InvalidBadSession;
                }
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            Label child = new Label {
                ID = this.ID + "_NoBotLabel"
            };
            this.Controls.Add(child);
            this._extender = new NoBotExtender();
            this._extender.ID = this.ID + "_NoBotExtender";
            this._extender.TargetControlID = child.ID;
            this.Controls.Add(this._extender);
        }

        private string CreateSessionKey(long ticks) => 
            string.Format(CultureInfo.InvariantCulture, "NoBot_SessionKey_{0}_{1}", new object[] { this.UniqueID, ticks });

        public static void EmptyUserAddressCache()
        {
            lock (_pastAddresses)
            {
                _pastAddresses.Clear();
            }
        }

        public static SortedList<DateTime, string> GetCopyOfUserAddressCache()
        {
            lock (_pastAddresses)
            {
                return new SortedList<DateTime, string>(_pastAddresses);
            }
        }

        public bool IsValid()
        {
            NoBotState state;
            return this.IsValid(out state);
        }

        public bool IsValid(out NoBotState state)
        {
            this.EnsureChildControls();
            this.CheckResponseAndStoreState();
            state = this._state;
            return (NoBotState.Valid == state);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.EnsureChildControls();
            this.CheckResponseAndStoreState();
            NoBotEventArgs args = new NoBotEventArgs();
            DateTime utcNow = DateTime.UtcNow;
            int millisecond = utcNow.Millisecond;
            args.ChallengeScript = string.Format(CultureInfo.InvariantCulture, "~{0}", new object[] { millisecond.ToString(CultureInfo.InvariantCulture) });
            args.RequiredResponse = ~millisecond.ToString(CultureInfo.InvariantCulture);
            if (this.GenerateChallengeAndResponse != null)
            {
                this.GenerateChallengeAndResponse(this, args);
            }
            this._extender.ChallengeScript = args.ChallengeScript;
            this._extender.ClientState = "";
            this.ViewState[this.ResponseTimeKey] = utcNow.AddSeconds((double) this._responseMinimumDelaySeconds);
            string str = this.CreateSessionKey(utcNow.Ticks);
            this.ViewState[this.SessionKeyKey] = str;
            this.Page.Session[str] = args.RequiredResponse;
        }

        public int CutoffMaximumInstances
        {
            get => 
                this._cutoffMaximumInstances;
            set
            {
                this._cutoffMaximumInstances = value;
            }
        }

        public int CutoffWindowSeconds
        {
            get => 
                this._cutoffWindowSeconds;
            set
            {
                this._cutoffWindowSeconds = value;
            }
        }

        public int ResponseMinimumDelaySeconds
        {
            get => 
                this._responseMinimumDelaySeconds;
            set
            {
                this._responseMinimumDelaySeconds = value;
            }
        }

        private string ResponseTimeKey =>
            string.Format(CultureInfo.InvariantCulture, "NoBot_ResponseTimeKey_{0}", new object[] { this.UniqueID });

        private string SessionKeyKey =>
            string.Format(CultureInfo.InvariantCulture, "NoBot_SessionKeyKey_{0}", new object[] { this.UniqueID });
    }
}

