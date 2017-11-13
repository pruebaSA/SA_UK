﻿namespace System.Collections.Specialized
{
    using MS.Internal;
    using System;
    using System.Collections;
    using System.Windows;

    public class NotifyCollectionChangedEventArgs : EventArgs
    {
        private NotifyCollectionChangedAction _action;
        private IList _newItems;
        private int _newStartingIndex;
        private IList _oldItems;
        private int _oldStartingIndex;

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Reset)
            {
                throw new ArgumentException(System.Windows.SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Reset }), "action");
            }
            this.InitializeAdd(action, null, -1);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException(System.Windows.SR.Get("MustBeResetAddOrRemoveActionForCtor"), "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                {
                    throw new ArgumentException(System.Windows.SR.Get("ResetActionRequiresNullItem"), "action");
                }
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changedItems");
                }
                this.InitializeAddOrRemove(action, changedItems, -1);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException(System.Windows.SR.Get("MustBeResetAddOrRemoveActionForCtor"), "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItem != null)
                {
                    throw new ArgumentException(System.Windows.SR.Get("ResetActionRequiresNullItem"), "action");
                }
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                this.InitializeAddOrRemove(action, new object[] { changedItem }, -1);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException(System.Windows.SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Replace }), "action");
            }
            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("oldItems");
            }
            this.InitializeMoveOrReplace(action, newItems, oldItems, -1, -1);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException(System.Windows.SR.Get("MustBeResetAddOrRemoveActionForCtor"), "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                {
                    throw new ArgumentException(System.Windows.SR.Get("ResetActionRequiresNullItem"), "action");
                }
                if (startingIndex != -1)
                {
                    throw new ArgumentException(System.Windows.SR.Get("ResetActionRequiresIndexMinus1"), "action");
                }
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changedItems");
                }
                if (startingIndex < -1)
                {
                    throw new ArgumentException(System.Windows.SR.Get("IndexCannotBeNegative"), "startingIndex");
                }
                this.InitializeAddOrRemove(action, changedItems, startingIndex);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException(System.Windows.SR.Get("MustBeResetAddOrRemoveActionForCtor"), "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItem != null)
                {
                    throw new ArgumentException(System.Windows.SR.Get("ResetActionRequiresNullItem"), "action");
                }
                if (index != -1)
                {
                    throw new ArgumentException(System.Windows.SR.Get("ResetActionRequiresIndexMinus1"), "action");
                }
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                this.InitializeAddOrRemove(action, new object[] { changedItem }, index);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException(System.Windows.SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Replace }), "action");
            }
            this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, -1, -1);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException(System.Windows.SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Replace }), "action");
            }
            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("oldItems");
            }
            this.InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Move)
            {
                throw new ArgumentException(System.Windows.SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Move }), "action");
            }
            if (index < 0)
            {
                throw new ArgumentException(System.Windows.SR.Get("IndexCannotBeNegative"), "index");
            }
            this.InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Move)
            {
                throw new ArgumentException(System.Windows.SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Move }), "action");
            }
            if (index < 0)
            {
                throw new ArgumentException(System.Windows.SR.Get("IndexCannotBeNegative"), "index");
            }
            object[] newItems = new object[] { changedItem };
            this.InitializeMoveOrReplace(action, newItems, newItems, index, oldIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException(System.Windows.SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Replace }), "action");
            }
            this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, index, index);
        }

        private void InitializeAdd(NotifyCollectionChangedAction action, IList newItems, int newStartingIndex)
        {
            this._action = action;
            this._newItems = (newItems == null) ? null : ArrayList.ReadOnly(newItems);
            this._newStartingIndex = newStartingIndex;
        }

        private void InitializeAddOrRemove(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            if (action == NotifyCollectionChangedAction.Add)
            {
                this.InitializeAdd(action, changedItems, startingIndex);
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                this.InitializeRemove(action, changedItems, startingIndex);
            }
            else
            {
                Invariant.Assert(false, "Unsupported action: {0}", action.ToString());
            }
        }

        private void InitializeMoveOrReplace(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex, int oldStartingIndex)
        {
            this.InitializeAdd(action, newItems, startingIndex);
            this.InitializeRemove(action, oldItems, oldStartingIndex);
        }

        private void InitializeRemove(NotifyCollectionChangedAction action, IList oldItems, int oldStartingIndex)
        {
            this._action = action;
            this._oldItems = (oldItems == null) ? null : ArrayList.ReadOnly(oldItems);
            this._oldStartingIndex = oldStartingIndex;
        }

        public NotifyCollectionChangedAction Action =>
            this._action;

        public IList NewItems =>
            this._newItems;

        public int NewStartingIndex =>
            this._newStartingIndex;

        public IList OldItems =>
            this._oldItems;

        public int OldStartingIndex =>
            this._oldStartingIndex;
    }
}

