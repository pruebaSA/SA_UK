namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WizardStepCollection : IList, ICollection, IEnumerable
    {
        private Wizard _wizard;

        internal WizardStepCollection(Wizard wizard)
        {
            this._wizard = wizard;
            wizard.TemplatedSteps.Clear();
        }

        public void Add(WizardStepBase wizardStep)
        {
            if (wizardStep == null)
            {
                throw new ArgumentNullException("wizardStep");
            }
            wizardStep.PreventAutoID();
            this.RemoveIfAlreadyExistsInWizard(wizardStep);
            wizardStep.Owner = this._wizard;
            this.Views.Add(wizardStep);
            if (wizardStep is TemplatedWizardStep)
            {
                this._wizard.TemplatedSteps.Add(wizardStep);
                this._wizard.RegisterCustomNavigationContainers((TemplatedWizardStep) wizardStep);
            }
            this.NotifyWizardStepsChanged();
        }

        public void AddAt(int index, WizardStepBase wizardStep)
        {
            if (wizardStep == null)
            {
                throw new ArgumentNullException("wizardStep");
            }
            this.RemoveIfAlreadyExistsInWizard(wizardStep);
            wizardStep.PreventAutoID();
            wizardStep.Owner = this._wizard;
            this.Views.AddAt(index, wizardStep);
            if (wizardStep is TemplatedWizardStep)
            {
                this._wizard.TemplatedSteps.Add(wizardStep);
                this._wizard.RegisterCustomNavigationContainers((TemplatedWizardStep) wizardStep);
            }
            this.NotifyWizardStepsChanged();
        }

        public void Clear()
        {
            this.Views.Clear();
            this._wizard.TemplatedSteps.Clear();
            this.NotifyWizardStepsChanged();
        }

        public bool Contains(WizardStepBase wizardStep)
        {
            if (wizardStep == null)
            {
                throw new ArgumentNullException("wizardStep");
            }
            return this.Views.Contains(wizardStep);
        }

        public void CopyTo(WizardStepBase[] array, int index)
        {
            this.Views.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator() => 
            this.Views.GetEnumerator();

        private WizardStepBase GetStepAndVerify(object value)
        {
            WizardStepBase base2 = value as WizardStepBase;
            if (base2 == null)
            {
                throw new ArgumentException(System.Web.SR.GetString("Wizard_WizardStepOnly"));
            }
            return base2;
        }

        public int IndexOf(WizardStepBase wizardStep)
        {
            if (wizardStep == null)
            {
                throw new ArgumentNullException("wizardStep");
            }
            return this.Views.IndexOf(wizardStep);
        }

        public void Insert(int index, WizardStepBase wizardStep)
        {
            this.AddAt(index, wizardStep);
        }

        internal void NotifyWizardStepsChanged()
        {
            this._wizard.OnWizardStepsChanged();
        }

        public void Remove(WizardStepBase wizardStep)
        {
            if (wizardStep == null)
            {
                throw new ArgumentNullException("wizardStep");
            }
            this.Views.Remove(wizardStep);
            wizardStep.Owner = null;
            if (wizardStep is TemplatedWizardStep)
            {
                this._wizard.TemplatedSteps.Remove(wizardStep);
            }
            this.NotifyWizardStepsChanged();
        }

        public void RemoveAt(int index)
        {
            WizardStepBase base2 = this.Views[index] as WizardStepBase;
            if (base2 != null)
            {
                base2.Owner = null;
                if (base2 is TemplatedWizardStep)
                {
                    this._wizard.TemplatedSteps.Remove(base2);
                }
            }
            this.Views.RemoveAt(index);
            this.NotifyWizardStepsChanged();
        }

        private void RemoveIfAlreadyExistsInWizard(WizardStepBase wizardStep)
        {
            if (wizardStep.Owner != null)
            {
                wizardStep.Owner.WizardSteps.Remove(wizardStep);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.Views.CopyTo(array, index);
        }

        int IList.Add(object value)
        {
            WizardStepBase stepAndVerify = this.GetStepAndVerify(value);
            stepAndVerify.PreventAutoID();
            this.Add(stepAndVerify);
            return this.IndexOf(stepAndVerify);
        }

        bool IList.Contains(object value) => 
            this.Contains(this.GetStepAndVerify(value));

        int IList.IndexOf(object value) => 
            this.IndexOf(this.GetStepAndVerify(value));

        void IList.Insert(int index, object value)
        {
            this.AddAt(index, this.GetStepAndVerify(value));
        }

        void IList.Remove(object value)
        {
            this.Remove(this.GetStepAndVerify(value));
        }

        public int Count =>
            this.Views.Count;

        public bool IsReadOnly =>
            this.Views.IsReadOnly;

        public bool IsSynchronized =>
            false;

        public WizardStepBase this[int index] =>
            ((WizardStepBase) this.Views[index]);

        public object SyncRoot =>
            this;

        bool IList.IsFixedSize =>
            false;

        object IList.this[int index]
        {
            get => 
                this.Views[index];
            set
            {
                this.RemoveAt(index);
                this.AddAt(index, this.GetStepAndVerify(value));
            }
        }

        private ViewCollection Views =>
            this._wizard.MultiView.Views;
    }
}

