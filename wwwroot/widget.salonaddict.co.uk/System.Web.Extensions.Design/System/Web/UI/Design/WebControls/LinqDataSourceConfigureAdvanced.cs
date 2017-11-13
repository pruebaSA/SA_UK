namespace System.Web.UI.Design.WebControls
{
    using System;

    internal class LinqDataSourceConfigureAdvanced : ILinqDataSourceConfigureAdvanced
    {
        private ILinqDataSourceConfigureAdvancedForm _form;
        private ConfigureState _state;

        private LinqDataSourceConfigureAdvanced(ILinqDataSourceConfigureAdvancedForm form)
        {
            this._form = form;
            this._form.Register(this);
        }

        internal LinqDataSourceConfigureAdvanced(ILinqDataSourceConfigureAdvancedForm form, ConfigureState state) : this(form)
        {
            this._state = state;
        }

        internal LinqDataSourceConfigureAdvanced(ILinqDataSourceConfigureAdvancedForm form, System.Web.UI.Design.WebControls.LinqDataSourceState linqDataSourceState) : this(form)
        {
            this._state = new ConfigureState();
            this.LinqDataSourceState = linqDataSourceState;
        }

        public void LoadState()
        {
            this.EnableDelete = this.LinqDataSourceState.EnableDelete;
            this.EnableInsert = this.LinqDataSourceState.EnableInsert;
            this.EnableUpdate = this.LinqDataSourceState.EnableUpdate;
            this._form.SetEnableDelete(this.EnableDelete);
            this._form.SetEnableInsert(this.EnableInsert);
            this._form.SetEnableUpdate(this.EnableUpdate);
        }

        public void SaveState()
        {
            this.LinqDataSourceState.EnableDelete = this.EnableDelete;
            this.LinqDataSourceState.EnableInsert = this.EnableInsert;
            this.LinqDataSourceState.EnableUpdate = this.EnableUpdate;
        }

        public void SetEnableDelete(bool enable)
        {
            this.EnableDelete = enable;
        }

        public void SetEnableInsert(bool enable)
        {
            this.EnableInsert = enable;
        }

        public void SetEnableUpdate(bool enable)
        {
            this.EnableUpdate = enable;
        }

        private bool EnableDelete
        {
            get => 
                this._state.EnableDelete;
            set
            {
                this._state.EnableDelete = value;
            }
        }

        private bool EnableInsert
        {
            get => 
                this._state.EnableInsert;
            set
            {
                this._state.EnableInsert = value;
            }
        }

        private bool EnableUpdate
        {
            get => 
                this._state.EnableUpdate;
            set
            {
                this._state.EnableUpdate = value;
            }
        }

        private System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState
        {
            get => 
                this._state.LinqDataSourceState;
            set
            {
                this._state.LinqDataSourceState = value;
            }
        }

        public class ConfigureState
        {
            public bool EnableDelete;
            public bool EnableInsert;
            public bool EnableUpdate;
            public System.Web.UI.Design.WebControls.LinqDataSourceState LinqDataSourceState;
        }
    }
}

