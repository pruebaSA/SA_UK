namespace System.Windows
{
    using MS.Internal;
    using MS.Internal.WindowsBase;
    using MS.Utility;
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public abstract class Freezable : DependencyObject, ISealable
    {
        [ThreadStatic]
        private static EventStorage _eventStorage;
        private DependencyProperty _property;
        private const int INITIAL_EVENTSTORAGE_SIZE = 4;

        public event EventHandler Changed
        {
            add
            {
                this.WritePreamble();
                if (value != null)
                {
                    this.ChangedInternal += value;
                }
            }
            remove
            {
                this.WritePreamble();
                if (value != null)
                {
                    this.ChangedInternal -= value;
                }
            }
        }

        internal event EventHandler ChangedInternal
        {
            add
            {
                this.HandlerAdd(value);
            }
            remove
            {
                this.HandlerRemove(value);
            }
        }

        protected Freezable()
        {
        }

        internal void AddContextInformation(DependencyObject context, DependencyProperty property)
        {
            if (base.Freezable_UsingSingletonContext)
            {
                this.ConvertToContextList();
            }
            if (base.Freezable_UsingContextList)
            {
                this.AddContextToList(context, property);
            }
            else
            {
                this.AddSingletonContext(context, property);
            }
        }

        private void AddContextToList(DependencyObject context, DependencyProperty property)
        {
            FrugalObjectList<FreezableContextPair> contextList = this.ContextList;
            int count = contextList.Count;
            int index = count;
            int numDead = 0;
            DependencyObject obj2 = null;
            bool hasMultipleInheritanceContexts = this.HasMultipleInheritanceContexts;
            bool flag2 = context.CanBeInheritanceContext && !base.IsInheritanceContextSealed;
            for (int i = 0; i < count; i++)
            {
                DependencyObject target = (DependencyObject) contextList[i].Owner.Target;
                if (target != null)
                {
                    if (target == context)
                    {
                        index = i + 1;
                        flag2 = false;
                    }
                    if (flag2 && !hasMultipleInheritanceContexts)
                    {
                        if ((target != obj2) && target.CanBeInheritanceContext)
                        {
                            hasMultipleInheritanceContexts = true;
                            base.Freezable_HasMultipleInheritanceContexts = true;
                        }
                        obj2 = target;
                    }
                }
                else
                {
                    numDead++;
                }
            }
            contextList.Insert(index, new FreezableContextPair(context, property));
            this.PruneContexts(contextList, numDead);
        }

        internal override void AddInheritanceContext(DependencyObject context, DependencyProperty property)
        {
            if (!this.IsFrozenInternal)
            {
                DependencyObject inheritanceContext = this.InheritanceContext;
                this.AddContextInformation(context, property);
                if (inheritanceContext != this.InheritanceContext)
                {
                    base.OnInheritanceContextChanged(EventArgs.Empty);
                }
            }
        }

        private void AddSingletonContext(DependencyObject context, DependencyProperty property)
        {
            if (this.HasHandlers)
            {
                HandlerContextStorage storage = new HandlerContextStorage {
                    _handlerStorage = base._contextStorage,
                    _contextStorage = context
                };
                base._contextStorage = storage;
            }
            else
            {
                base._contextStorage = context;
            }
            this._property = property;
            base.Freezable_UsingSingletonContext = true;
        }

        private void AddSingletonHandler(EventHandler handler)
        {
            if (this.HasContextInformation)
            {
                HandlerContextStorage storage = new HandlerContextStorage {
                    _contextStorage = base._contextStorage,
                    _handlerStorage = handler
                };
                base._contextStorage = storage;
            }
            else
            {
                base._contextStorage = handler;
            }
            base.Freezable_UsingSingletonHandler = true;
        }

        internal void ClearContextAndHandlers()
        {
            base.Freezable_UsingHandlerList = false;
            base.Freezable_UsingContextList = false;
            base.Freezable_UsingSingletonHandler = false;
            base.Freezable_UsingSingletonContext = false;
            base._contextStorage = null;
            this._property = null;
        }

        public Freezable Clone()
        {
            this.ReadPreamble();
            Freezable clone = this.CreateInstance();
            clone.CloneCore(this);
            Debug_VerifyCloneCommon(this, clone, true);
            return clone;
        }

        protected virtual void CloneCore(Freezable sourceFreezable)
        {
            this.CloneCoreCommon(sourceFreezable, false, true);
        }

        private void CloneCoreCommon(Freezable sourceFreezable, bool useCurrentValue, bool cloneFrozenValues)
        {
            EffectiveValueEntry[] effectiveValues = sourceFreezable.EffectiveValues;
            uint effectiveValuesCount = sourceFreezable.EffectiveValuesCount;
            for (uint i = 0; i < effectiveValuesCount; i++)
            {
                EffectiveValueEntry entry = effectiveValues[i];
                DependencyProperty dp = DependencyProperty.RegisteredPropertyList.List[entry.PropertyIndex];
                if ((dp != null) && !dp.ReadOnly)
                {
                    object obj2;
                    EntryIndex entryIndex = new EntryIndex(i);
                    if (useCurrentValue)
                    {
                        obj2 = sourceFreezable.GetValueEntry(entryIndex, dp, null, RequestFlags.FullyResolved).Value;
                    }
                    else
                    {
                        obj2 = sourceFreezable.ReadLocalValueEntry(entryIndex, dp, true);
                        if (obj2 == DependencyProperty.UnsetValue)
                        {
                            continue;
                        }
                        if (entry.IsExpression)
                        {
                            obj2 = ((Expression) obj2).Copy(this, dp);
                        }
                    }
                    Freezable freezable = obj2 as Freezable;
                    if (freezable != null)
                    {
                        Freezable freezable2;
                        if (cloneFrozenValues)
                        {
                            freezable2 = freezable.CreateInstanceCore();
                            if (useCurrentValue)
                            {
                                freezable2.CloneCurrentValueCore(freezable);
                            }
                            else
                            {
                                freezable2.CloneCore(freezable);
                            }
                            obj2 = freezable2;
                            Debug_VerifyCloneCommon(freezable, freezable2, true);
                        }
                        else if (!freezable.IsFrozen)
                        {
                            freezable2 = freezable.CreateInstanceCore();
                            if (useCurrentValue)
                            {
                                freezable2.GetCurrentValueAsFrozenCore(freezable);
                            }
                            else
                            {
                                freezable2.GetAsFrozenCore(freezable);
                            }
                            obj2 = freezable2;
                            Debug_VerifyCloneCommon(freezable, freezable2, false);
                        }
                    }
                    base.SetValue(dp, obj2);
                }
            }
        }

        public Freezable CloneCurrentValue()
        {
            this.ReadPreamble();
            Freezable clone = this.CreateInstance();
            clone.CloneCurrentValueCore(this);
            Debug_VerifyCloneCommon(this, clone, true);
            return clone;
        }

        protected virtual void CloneCurrentValueCore(Freezable sourceFreezable)
        {
            this.CloneCoreCommon(sourceFreezable, true, true);
        }

        private void ConvertToContextList()
        {
            FrugalObjectList<FreezableContextPair> list = new FrugalObjectList<FreezableContextPair>(2);
            list.Add(new FreezableContextPair(this.SingletonContext, this.SingletonContextProperty));
            if (this.HasHandlers)
            {
                ((HandlerContextStorage) base._contextStorage)._contextStorage = list;
            }
            else
            {
                base._contextStorage = list;
            }
            base.Freezable_UsingContextList = true;
            base.Freezable_UsingSingletonContext = false;
            this._property = null;
        }

        private void ConvertToHandlerList()
        {
            EventHandler singletonHandler = this.SingletonHandler;
            FrugalObjectList<EventHandler> list = new FrugalObjectList<EventHandler>(2);
            list.Add(singletonHandler);
            if (this.HasContextInformation)
            {
                ((HandlerContextStorage) base._contextStorage)._handlerStorage = list;
            }
            else
            {
                base._contextStorage = list;
            }
            base.Freezable_UsingHandlerList = true;
            base.Freezable_UsingSingletonHandler = false;
        }

        protected Freezable CreateInstance()
        {
            Freezable newInstance = this.CreateInstanceCore();
            Debug_VerifyInstance("CreateInstance", this, newInstance);
            return newInstance;
        }

        protected abstract Freezable CreateInstanceCore();
        private void Debug_DetectContextLeaks()
        {
            if (Invariant.Strict)
            {
                if (base.Freezable_UsingSingletonContext)
                {
                    this.Debug_VerifyContextIsValid(this.SingletonContext, this.SingletonContextProperty);
                }
                else if (base.Freezable_UsingContextList)
                {
                    FrugalObjectList<FreezableContextPair> contextList = this.ContextList;
                    int num = 0;
                    int count = this.ContextList.Count;
                    while (num < count)
                    {
                        FreezableContextPair pair = this.ContextList[num];
                        DependencyObject target = (DependencyObject) pair.Owner.Target;
                        if (pair.Owner.IsAlive)
                        {
                            this.Debug_VerifyContextIsValid(target, pair.Property);
                        }
                        num++;
                    }
                }
            }
        }

        private static void Debug_VerifyCloneCommon(Freezable original, object clone, bool isDeepClone)
        {
            if (Invariant.Strict)
            {
                Freezable newInstance = (Freezable) clone;
                Debug_VerifyInstance("CloneCore", original, newInstance);
                if (isDeepClone)
                {
                    Invariant.Assert(clone != original, "CloneCore should not return the same instance as the original.");
                }
                Invariant.Assert(!newInstance.HasHandlers, "CloneCore should not have handlers attached on construction.");
                IList list = original as IList;
                if (list != null)
                {
                    IList list2 = clone as IList;
                    Invariant.Assert(list.Count == list2.Count, "CloneCore didn't clone all of the elements in the list.");
                    for (int i = 0; i < list2.Count; i++)
                    {
                        Freezable freezable2 = list[i] as Freezable;
                        Freezable freezable3 = list2[i] as Freezable;
                        if ((isDeepClone && (freezable3 != null)) && (freezable3 != null))
                        {
                            Invariant.Assert(freezable2 != freezable3, "CloneCore didn't clone the elements in the list correctly.");
                        }
                    }
                }
            }
        }

        private void Debug_VerifyContextIsValid(DependencyObject owner, DependencyProperty property)
        {
            if (Invariant.Strict)
            {
                Invariant.Assert(owner != null, "We should not have null owners in the ContextList/SingletonContext.");
                if (property != null)
                {
                    owner.GetValue(property);
                    if ((property.Name == "Visual") && (property.OwnerType.FullName == "System.Windows.Media.VisualBrush"))
                    {
                        bool flag1 = owner.GetType().FullName != "System.Windows.Media.VisualBrush";
                    }
                }
            }
        }

        private static void Debug_VerifyInstance(string methodName, Freezable original, Freezable newInstance)
        {
            if (Invariant.Strict)
            {
                Invariant.Assert(newInstance != null, "{0} should not return null.", methodName);
                Invariant.Assert(newInstance.GetType() == original.GetType(), string.Format(CultureInfo.InvariantCulture, "{0} should return instance of same type. (Expected= '{1}', Actual='{2}')", new object[] { methodName, original.GetType(), newInstance.GetType() }));
                Invariant.Assert(!newInstance.IsFrozen, "{0} should return a mutable instance. Recieved a frozen instance.", methodName);
            }
        }

        private static void EnsureConsistentDispatchers(DependencyObject owner, DependencyObject child)
        {
            if (((owner.Dispatcher != null) && (child.Dispatcher != null)) && (owner.Dispatcher != child.Dispatcher))
            {
                throw new InvalidOperationException(System.Windows.SR.Get("Freezable_AttemptToUseInnerValueWithDifferentThread"));
            }
        }

        internal void FireChanged()
        {
            EventStorage calledHandlers = null;
            this.GetChangeHandlersAndInvalidateSubProperties(ref calledHandlers);
            if (calledHandlers != null)
            {
                int num = 0;
                int count = calledHandlers.Count;
                while (num < count)
                {
                    calledHandlers[num](this, EventArgs.Empty);
                    calledHandlers[num] = null;
                    num++;
                }
                calledHandlers.Clear();
                calledHandlers.InUse = false;
            }
        }

        public void Freeze()
        {
            if (!this.CanFreeze)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("Freezable_CantFreeze"));
            }
            this.Freeze(false);
        }

        internal bool Freeze(bool isChecking)
        {
            if (isChecking)
            {
                this.ReadPreamble();
                return this.FreezeCore(true);
            }
            if (!this.IsFrozenInternal)
            {
                this.WritePreamble();
                this.FreezeCore(false);
                PropertyMetadata.RemoveAllCachedDefaultValues(this);
                DependencyObject.DependentListMapField.ClearValue(this);
                base.Freezable_Frozen = true;
                base.DetachFromDispatcher();
                this.FireChanged();
                this.ClearContextAndHandlers();
                this.WritePostscript();
            }
            return true;
        }

        protected internal static bool Freeze(Freezable freezable, bool isChecking)
        {
            if (freezable != null)
            {
                return freezable.Freeze(isChecking);
            }
            return true;
        }

        protected virtual bool FreezeCore(bool isChecking)
        {
            EffectiveValueEntry[] effectiveValues = base.EffectiveValues;
            uint effectiveValuesCount = base.EffectiveValuesCount;
            for (uint i = 0; i < effectiveValuesCount; i++)
            {
                DependencyProperty dp = DependencyProperty.RegisteredPropertyList.List[effectiveValues[i].PropertyIndex];
                if (dp != null)
                {
                    EntryIndex entryIndex = new EntryIndex(i);
                    PropertyMetadata metadata = dp.GetMetadata(base.DependencyObjectType);
                    if (!metadata.FreezeValueCallback(this, dp, entryIndex, metadata, isChecking))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public Freezable GetAsFrozen()
        {
            this.ReadPreamble();
            if (this.IsFrozenInternal)
            {
                return this;
            }
            Freezable clone = this.CreateInstance();
            clone.GetAsFrozenCore(this);
            Debug_VerifyCloneCommon(this, clone, false);
            clone.Freeze();
            return clone;
        }

        protected virtual void GetAsFrozenCore(Freezable sourceFreezable)
        {
            this.CloneCoreCommon(sourceFreezable, false, false);
        }

        private void GetChangeHandlersAndInvalidateSubProperties(ref EventStorage calledHandlers)
        {
            Freezable freezable;
            this.OnChanged();
            if (base.Freezable_UsingSingletonContext)
            {
                DependencyObject singletonContext = this.SingletonContext;
                freezable = singletonContext as Freezable;
                if (freezable != null)
                {
                    freezable.GetChangeHandlersAndInvalidateSubProperties(ref calledHandlers);
                }
                if (this.SingletonContextProperty != null)
                {
                    singletonContext.InvalidateSubProperty(this.SingletonContextProperty);
                }
            }
            else if (base.Freezable_UsingContextList)
            {
                FrugalObjectList<FreezableContextPair> contextList = this.ContextList;
                DependencyObject obj3 = null;
                int numDead = 0;
                int num2 = 0;
                int count = contextList.Count;
                while (num2 < count)
                {
                    FreezableContextPair pair = contextList[num2];
                    DependencyObject target = (DependencyObject) pair.Owner.Target;
                    if (target != null)
                    {
                        if (target != obj3)
                        {
                            freezable = target as Freezable;
                            if (freezable != null)
                            {
                                freezable.GetChangeHandlersAndInvalidateSubProperties(ref calledHandlers);
                            }
                            obj3 = target;
                        }
                        if (pair.Property != null)
                        {
                            target.InvalidateSubProperty(pair.Property);
                        }
                    }
                    else
                    {
                        numDead++;
                    }
                    num2++;
                }
                this.PruneContexts(contextList, numDead);
            }
            this.GetHandlers(ref calledHandlers);
        }

        public Freezable GetCurrentValueAsFrozen()
        {
            this.ReadPreamble();
            if (this.IsFrozenInternal)
            {
                return this;
            }
            Freezable clone = this.CreateInstance();
            clone.GetCurrentValueAsFrozenCore(this);
            Debug_VerifyCloneCommon(this, clone, false);
            clone.Freeze();
            return clone;
        }

        protected virtual void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            this.CloneCoreCommon(sourceFreezable, true, false);
        }

        private EventStorage GetEventStorage()
        {
            EventStorage cachedEventStorage = this.CachedEventStorage;
            if (cachedEventStorage.InUse)
            {
                cachedEventStorage = new EventStorage(cachedEventStorage.PhysicalSize);
            }
            cachedEventStorage.InUse = true;
            return cachedEventStorage;
        }

        private void GetHandlers(ref EventStorage calledHandlers)
        {
            if (base.Freezable_UsingSingletonHandler)
            {
                if (calledHandlers == null)
                {
                    calledHandlers = this.GetEventStorage();
                }
                calledHandlers.Add(this.SingletonHandler);
            }
            else if (base.Freezable_UsingHandlerList)
            {
                if (calledHandlers == null)
                {
                    calledHandlers = this.GetEventStorage();
                }
                FrugalObjectList<EventHandler> handlerList = this.HandlerList;
                int num = 0;
                int count = handlerList.Count;
                while (num < count)
                {
                    calledHandlers.Add(handlerList[num]);
                    num++;
                }
            }
        }

        private void HandlerAdd(EventHandler handler)
        {
            if (base.Freezable_UsingSingletonHandler)
            {
                this.ConvertToHandlerList();
            }
            if (base.Freezable_UsingHandlerList)
            {
                this.HandlerList.Add(handler);
            }
            else
            {
                this.AddSingletonHandler(handler);
            }
        }

        private void HandlerRemove(EventHandler handler)
        {
            bool flag = true;
            if (base.Freezable_UsingSingletonHandler)
            {
                if (this.SingletonHandler == handler)
                {
                    this.RemoveSingletonHandler();
                    flag = false;
                }
            }
            else if (base.Freezable_UsingHandlerList)
            {
                FrugalObjectList<EventHandler> handlerList = this.HandlerList;
                int index = handlerList.IndexOf(handler);
                if (index >= 0)
                {
                    handlerList.RemoveAt(index);
                    flag = false;
                }
                if (handlerList.Count == 0)
                {
                    this.RemoveHandlerList();
                }
            }
            if (flag)
            {
                throw new ArgumentException(System.Windows.SR.Get("Freezable_UnregisteredHandler"), "handler");
            }
        }

        protected virtual void OnChanged()
        {
        }

        protected void OnFreezablePropertyChanged(DependencyObject oldValue, DependencyObject newValue)
        {
            this.OnFreezablePropertyChanged(oldValue, newValue, null);
        }

        protected void OnFreezablePropertyChanged(DependencyObject oldValue, DependencyObject newValue, DependencyProperty property)
        {
            if (newValue != null)
            {
                EnsureConsistentDispatchers(this, newValue);
            }
            if (oldValue != null)
            {
                base.RemoveSelfAsInheritanceContext(oldValue, property);
            }
            if (newValue != null)
            {
                base.ProvideSelfAsInheritanceContext(newValue, property);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (!e.IsASubPropertyChange || (e.OperationType == OperationType.ChangeMutableDefaultValue))
            {
                this.WritePostscript();
            }
            this.Debug_DetectContextLeaks();
        }

        private void PruneContexts(FrugalObjectList<FreezableContextPair> oldList, int numDead)
        {
            int count = oldList.Count;
            if ((count - numDead) == 0)
            {
                this.RemoveContextList();
            }
            else if (numDead > 0)
            {
                FrugalObjectList<FreezableContextPair> list = new FrugalObjectList<FreezableContextPair>(count - numDead);
                for (int i = 0; i < count; i++)
                {
                    if (oldList[i].Owner.IsAlive)
                    {
                        list.Add(oldList[i]);
                    }
                }
                this.ContextList = list;
            }
        }

        protected void ReadPreamble()
        {
            base.VerifyAccess();
        }

        private void RemoveContextInformation(DependencyObject context, DependencyProperty property)
        {
            bool flag = true;
            if (base.Freezable_UsingSingletonContext)
            {
                if ((this.SingletonContext == context) && (this.SingletonContextProperty == property))
                {
                    this.RemoveSingletonContext();
                    flag = false;
                }
            }
            else if (base.Freezable_UsingContextList)
            {
                FrugalObjectList<FreezableContextPair> contextList = this.ContextList;
                int numDead = 0;
                int index = -1;
                int count = contextList.Count;
                for (int i = 0; i < count; i++)
                {
                    FreezableContextPair pair = contextList[i];
                    object target = pair.Owner.Target;
                    if (target != null)
                    {
                        if ((flag && (pair.Property == property)) && (target == context))
                        {
                            index = i;
                            flag = false;
                        }
                    }
                    else
                    {
                        numDead++;
                    }
                }
                if (index != -1)
                {
                    contextList.RemoveAt(index);
                }
                this.PruneContexts(contextList, numDead);
            }
            if (flag)
            {
                throw new ArgumentException(System.Windows.SR.Get("Freezable_NotAContext"), "context");
            }
        }

        private void RemoveContextList()
        {
            if (this.HasHandlers)
            {
                base._contextStorage = ((HandlerContextStorage) base._contextStorage)._handlerStorage;
            }
            else
            {
                base._contextStorage = null;
            }
            base.Freezable_UsingContextList = false;
        }

        private void RemoveHandlerList()
        {
            if (this.HasContextInformation)
            {
                base._contextStorage = ((HandlerContextStorage) base._contextStorage)._contextStorage;
            }
            else
            {
                base._contextStorage = null;
            }
            base.Freezable_UsingHandlerList = false;
        }

        internal override void RemoveInheritanceContext(DependencyObject context, DependencyProperty property)
        {
            if (!this.IsFrozenInternal)
            {
                DependencyObject inheritanceContext = this.InheritanceContext;
                this.RemoveContextInformation(context, property);
                if (inheritanceContext != this.InheritanceContext)
                {
                    base.OnInheritanceContextChanged(EventArgs.Empty);
                }
            }
        }

        private void RemoveSingletonContext()
        {
            if (this.HasHandlers)
            {
                base._contextStorage = ((HandlerContextStorage) base._contextStorage)._handlerStorage;
            }
            else
            {
                base._contextStorage = null;
            }
            base.Freezable_UsingSingletonContext = false;
        }

        private void RemoveSingletonHandler()
        {
            if (this.HasContextInformation)
            {
                base._contextStorage = ((HandlerContextStorage) base._contextStorage)._contextStorage;
            }
            else
            {
                base._contextStorage = null;
            }
            base.Freezable_UsingSingletonHandler = false;
        }

        internal override void Seal()
        {
            Invariant.Assert(false);
        }

        void ISealable.Seal()
        {
            this.Freeze();
        }

        protected void WritePostscript()
        {
            this.FireChanged();
        }

        protected void WritePreamble()
        {
            base.VerifyAccess();
            if (this.IsFrozenInternal)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("Freezable_CantBeFrozen", new object[] { base.GetType().FullName }));
            }
        }

        private EventStorage CachedEventStorage
        {
            get
            {
                if (_eventStorage == null)
                {
                    _eventStorage = new EventStorage(4);
                }
                return _eventStorage;
            }
        }

        public bool CanFreeze
        {
            get
            {
                if (!this.IsFrozenInternal)
                {
                    return this.FreezeCore(true);
                }
                return true;
            }
        }

        private FrugalObjectList<FreezableContextPair> ContextList
        {
            get
            {
                if (this.HasHandlers)
                {
                    HandlerContextStorage storage = (HandlerContextStorage) base._contextStorage;
                    return (FrugalObjectList<FreezableContextPair>) storage._contextStorage;
                }
                return (FrugalObjectList<FreezableContextPair>) base._contextStorage;
            }
            set
            {
                if (this.HasHandlers)
                {
                    ((HandlerContextStorage) base._contextStorage)._contextStorage = value;
                }
                else
                {
                    base._contextStorage = value;
                }
            }
        }

        private FrugalObjectList<EventHandler> HandlerList
        {
            get
            {
                if (this.HasContextInformation)
                {
                    HandlerContextStorage storage = (HandlerContextStorage) base._contextStorage;
                    return (FrugalObjectList<EventHandler>) storage._handlerStorage;
                }
                return (FrugalObjectList<EventHandler>) base._contextStorage;
            }
        }

        private bool HasContextInformation
        {
            get
            {
                if (!base.Freezable_UsingContextList)
                {
                    return base.Freezable_UsingSingletonContext;
                }
                return true;
            }
        }

        private bool HasHandlers
        {
            get
            {
                if (!base.Freezable_UsingHandlerList)
                {
                    return base.Freezable_UsingSingletonHandler;
                }
                return true;
            }
        }

        internal override bool HasMultipleInheritanceContexts =>
            base.Freezable_HasMultipleInheritanceContexts;

        internal override DependencyObject InheritanceContext
        {
            [FriendAccessAllowed]
            get
            {
                if (!base.Freezable_HasMultipleInheritanceContexts)
                {
                    if (base.Freezable_UsingSingletonContext)
                    {
                        DependencyObject singletonContext = this.SingletonContext;
                        if (singletonContext.CanBeInheritanceContext)
                        {
                            return singletonContext;
                        }
                    }
                    else if (base.Freezable_UsingContextList)
                    {
                        FrugalObjectList<FreezableContextPair> contextList = this.ContextList;
                        int count = contextList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            DependencyObject target = (DependencyObject) contextList[i].Owner.Target;
                            if ((target != null) && target.CanBeInheritanceContext)
                            {
                                return target;
                            }
                        }
                    }
                }
                return null;
            }
        }

        public bool IsFrozen
        {
            get
            {
                this.ReadPreamble();
                return this.IsFrozenInternal;
            }
        }

        internal bool IsFrozenInternal =>
            base.Freezable_Frozen;

        private DependencyObject SingletonContext
        {
            get
            {
                if (this.HasHandlers)
                {
                    HandlerContextStorage storage = (HandlerContextStorage) base._contextStorage;
                    return (DependencyObject) storage._contextStorage;
                }
                return (DependencyObject) base._contextStorage;
            }
        }

        private DependencyProperty SingletonContextProperty =>
            this._property;

        private EventHandler SingletonHandler
        {
            get
            {
                if (this.HasContextInformation)
                {
                    HandlerContextStorage storage = (HandlerContextStorage) base._contextStorage;
                    return (EventHandler) storage._handlerStorage;
                }
                return (EventHandler) base._contextStorage;
            }
        }

        bool ISealable.CanSeal =>
            this.CanFreeze;

        bool ISealable.IsSealed =>
            this.IsFrozen;

        private class EventStorage
        {
            private EventHandler[] _events;
            private bool _inUse;
            private int _logSize;
            private int _physSize;

            public EventStorage(int initialSize)
            {
                if (initialSize <= 0)
                {
                    initialSize = 1;
                }
                this._events = new EventHandler[initialSize];
                this._logSize = 0;
                this._physSize = initialSize;
                this._inUse = false;
            }

            public void Add(EventHandler e)
            {
                if (this._logSize == this._physSize)
                {
                    this._physSize *= 2;
                    EventHandler[] handlerArray = new EventHandler[this._physSize];
                    for (int i = 0; i < this._logSize; i++)
                    {
                        handlerArray[i] = this._events[i];
                    }
                    this._events = handlerArray;
                }
                this._events[this._logSize] = e;
                this._logSize++;
            }

            public void Clear()
            {
                this._logSize = 0;
            }

            public int Count =>
                this._logSize;

            public bool InUse
            {
                get => 
                    this._inUse;
                set
                {
                    this._inUse = value;
                }
            }

            public EventHandler this[int idx]
            {
                get => 
                    this._events[idx];
                set
                {
                    this._events[idx] = value;
                }
            }

            public int PhysicalSize =>
                this._physSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FreezableContextPair
        {
            public readonly WeakReference Owner;
            public readonly DependencyProperty Property;
            public FreezableContextPair(DependencyObject dependObject, DependencyProperty dependProperty)
            {
                this.Owner = new WeakReference(dependObject);
                this.Property = dependProperty;
            }
        }

        private class HandlerContextStorage
        {
            public object _contextStorage;
            public object _handlerStorage;
        }
    }
}

