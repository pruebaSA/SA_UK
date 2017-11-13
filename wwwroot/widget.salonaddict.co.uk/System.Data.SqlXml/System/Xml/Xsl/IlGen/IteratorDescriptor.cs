namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class IteratorDescriptor
    {
        private BranchingContext brctxt;
        private bool hasNext;
        private GenerateHelper helper;
        private IteratorDescriptor iterParent;
        private Label lblBranch;
        private Label lblNext;
        private LocalBuilder locPos;
        private StorageDescriptor storage;

        public IteratorDescriptor(GenerateHelper helper)
        {
            this.Init(null, helper);
        }

        public IteratorDescriptor(IteratorDescriptor iterParent)
        {
            this.Init(iterParent, iterParent.helper);
        }

        public void CacheCount()
        {
            this.PushValue();
            this.helper.CallCacheCount(this.storage.ItemStorageType);
        }

        public void DiscardStack()
        {
            if (this.storage.Location == ItemLocation.Stack)
            {
                this.helper.Emit(OpCodes.Pop);
                this.storage = StorageDescriptor.None();
            }
        }

        public void EnsureItemStorageType(XmlQueryType xmlType, Type storageTypeDest)
        {
            if (this.storage.ItemStorageType != storageTypeDest)
            {
                if (this.storage.IsCached)
                {
                    if (this.storage.ItemStorageType == typeof(XPathNavigator))
                    {
                        this.EnsureStack();
                        this.helper.Call(XmlILMethods.NavsToItems);
                        goto Label_012F;
                    }
                    if (storageTypeDest == typeof(XPathNavigator))
                    {
                        this.EnsureStack();
                        this.helper.Call(XmlILMethods.ItemsToNavs);
                        goto Label_012F;
                    }
                }
                this.EnsureStackNoCache();
                if (this.storage.ItemStorageType == typeof(XPathItem))
                {
                    if (storageTypeDest == typeof(XPathNavigator))
                    {
                        this.helper.Emit(OpCodes.Castclass, typeof(XPathNavigator));
                    }
                    else
                    {
                        this.helper.CallValueAs(storageTypeDest);
                    }
                }
                else if (this.storage.ItemStorageType != typeof(XPathNavigator))
                {
                    this.helper.LoadInteger(this.helper.StaticData.DeclareXmlType(xmlType));
                    this.helper.LoadQueryRuntime();
                    this.helper.Call(XmlILMethods.StorageMethods[this.storage.ItemStorageType].ToAtomicValue);
                }
            }
        Label_012F:
            this.storage = this.storage.ToStorageType(storageTypeDest);
        }

        public void EnsureLocal(LocalBuilder bldr)
        {
            if (this.storage.LocalLocation != bldr)
            {
                this.EnsureStack();
                this.helper.Emit(OpCodes.Stloc, bldr);
                this.storage = this.storage.ToLocal(bldr);
            }
        }

        public void EnsureLocal(string locName)
        {
            if (this.storage.Location != ItemLocation.Local)
            {
                if (this.storage.IsCached)
                {
                    this.EnsureLocal(this.helper.DeclareLocal(locName, typeof(IList<>).MakeGenericType(new Type[] { this.storage.ItemStorageType })));
                }
                else
                {
                    this.EnsureLocal(this.helper.DeclareLocal(locName, this.storage.ItemStorageType));
                }
            }
        }

        public void EnsureLocalNoCache(LocalBuilder bldr)
        {
            this.EnsureNoCache();
            this.EnsureLocal(bldr);
        }

        public void EnsureLocalNoCache(string locName)
        {
            this.EnsureNoCache();
            this.EnsureLocal(locName);
        }

        public void EnsureNoCache()
        {
            if (this.storage.IsCached)
            {
                if (!this.HasLabelNext)
                {
                    this.EnsureStack();
                    this.helper.LoadInteger(0);
                    this.helper.CallCacheItem(this.storage.ItemStorageType);
                    this.storage = StorageDescriptor.Stack(this.storage.ItemStorageType, false);
                }
                else
                {
                    LocalBuilder locBldr = this.helper.DeclareLocal("$$$idx", typeof(int));
                    this.EnsureNoStack("$$$cache");
                    this.helper.LoadInteger(-1);
                    this.helper.Emit(OpCodes.Stloc, locBldr);
                    Label lbl = this.helper.DefineLabel();
                    this.helper.MarkLabel(lbl);
                    this.helper.Emit(OpCodes.Ldloc, locBldr);
                    this.helper.LoadInteger(1);
                    this.helper.Emit(OpCodes.Add);
                    this.helper.Emit(OpCodes.Stloc, locBldr);
                    this.helper.Emit(OpCodes.Ldloc, locBldr);
                    this.CacheCount();
                    this.helper.Emit(OpCodes.Bge, this.GetLabelNext());
                    this.PushValue();
                    this.helper.Emit(OpCodes.Ldloc, locBldr);
                    this.helper.CallCacheItem(this.storage.ItemStorageType);
                    this.SetIterator(lbl, StorageDescriptor.Stack(this.storage.ItemStorageType, false));
                }
            }
        }

        public void EnsureNoStack(string locName)
        {
            if (this.storage.Location == ItemLocation.Stack)
            {
                this.EnsureLocal(locName);
            }
        }

        public void EnsureNoStackNoCache(string locName)
        {
            this.EnsureNoCache();
            this.EnsureNoStack(locName);
        }

        public void EnsureStack()
        {
            switch (this.storage.Location)
            {
                case ItemLocation.Stack:
                    return;

                case ItemLocation.Parameter:
                case ItemLocation.Local:
                case ItemLocation.Current:
                    this.PushValue();
                    break;

                case ItemLocation.Global:
                    this.helper.LoadQueryRuntime();
                    this.helper.Call(this.storage.GlobalLocation);
                    break;
            }
            this.storage = this.storage.ToStack();
        }

        public void EnsureStackNoCache()
        {
            this.EnsureNoCache();
            this.EnsureStack();
        }

        public Label GetLabelNext() => 
            this.lblNext;

        private void Init(IteratorDescriptor iterParent, GenerateHelper helper)
        {
            this.helper = helper;
            this.iterParent = iterParent;
        }

        public void LoopToEnd(Label lblOnEnd)
        {
            if (this.hasNext)
            {
                this.helper.BranchAndMark(this.lblNext, lblOnEnd);
                this.hasNext = false;
            }
            this.storage = StorageDescriptor.None();
        }

        public void PushValue()
        {
            switch (this.storage.Location)
            {
                case ItemLocation.Stack:
                    this.helper.Emit(OpCodes.Dup);
                    return;

                case ItemLocation.Parameter:
                    this.helper.LoadParameter(this.storage.ParameterLocation);
                    return;

                case ItemLocation.Local:
                    this.helper.Emit(OpCodes.Ldloc, this.storage.LocalLocation);
                    return;

                case ItemLocation.Current:
                    this.helper.Emit(OpCodes.Ldloca, this.storage.CurrentLocation);
                    this.helper.Call(this.storage.CurrentLocation.LocalType.GetMethod("get_Current"));
                    return;
            }
        }

        public void SetBranching(BranchingContext brctxt, Label lblBranch)
        {
            this.brctxt = brctxt;
            this.lblBranch = lblBranch;
        }

        public void SetIterator(IteratorDescriptor iterInfo)
        {
            if (iterInfo.HasLabelNext)
            {
                this.lblNext = iterInfo.GetLabelNext();
                this.hasNext = true;
            }
            this.storage = iterInfo.Storage;
        }

        public void SetIterator(Label lblNext, StorageDescriptor storage)
        {
            this.lblNext = lblNext;
            this.hasNext = true;
            this.storage = storage;
        }

        public BranchingContext CurrentBranchingContext =>
            this.brctxt;

        public bool HasLabelNext =>
            this.hasNext;

        public bool IsBranching =>
            (this.brctxt != BranchingContext.None);

        public Label LabelBranch =>
            this.lblBranch;

        public LocalBuilder LocalPosition
        {
            get => 
                this.locPos;
            set
            {
                this.locPos = value;
            }
        }

        public IteratorDescriptor ParentIterator =>
            this.iterParent;

        public StorageDescriptor Storage
        {
            get => 
                this.storage;
            set
            {
                this.storage = value;
            }
        }
    }
}

