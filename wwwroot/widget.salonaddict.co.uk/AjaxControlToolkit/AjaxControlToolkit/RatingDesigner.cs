namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    internal class RatingDesigner : ControlDesigner
    {
        private DesignerActionListCollection _actionLists;

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (this._actionLists == null)
                {
                    this._actionLists = new DesignerActionListCollection();
                    this._actionLists.AddRange(base.ActionLists);
                    this._actionLists.Add(new ActionList(this));
                }
                return this._actionLists;
            }
        }

        public class ActionList : DesignerActionList
        {
            private DesignerActionItemCollection _items;
            private RatingDesigner _parent;

            public ActionList(RatingDesigner parent) : base(parent.Component)
            {
                this._parent = parent;
            }

            private void Alignment()
            {
                Rating component = (Rating) this._parent.Component;
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(component)["RatingAlign"];
                if (component.RatingAlign == Orientation.Horizontal)
                {
                    descriptor.SetValue(component, Orientation.Vertical);
                }
                else
                {
                    descriptor.SetValue(component, Orientation.Horizontal);
                }
            }

            private void Direction()
            {
                Rating component = (Rating) this._parent.Component;
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(component)["RatingDirection"];
                if (component.RatingDirection == RatingDirection.LeftToRightTopToBottom)
                {
                    descriptor.SetValue(component, RatingDirection.RightToLeftBottomToTop);
                }
                else
                {
                    descriptor.SetValue(component, RatingDirection.LeftToRightTopToBottom);
                }
            }

            public override DesignerActionItemCollection GetSortedActionItems()
            {
                if (this._items == null)
                {
                    this._items = new DesignerActionItemCollection();
                    this._items.Add(new DesignerActionPropertyItem("StartRating", "Initial Rating"));
                    this._items.Add(new DesignerActionPropertyItem("MaxRating", "Maximum Rating"));
                    this._items.Add(new DesignerActionPropertyItem("RealOnly", "Read-only"));
                    this._items.Add(new DesignerActionMethodItem(this, "Alignment", "Switch Align"));
                    this._items.Add(new DesignerActionMethodItem(this, "Direction", "Switch Direction"));
                }
                return this._items;
            }

            public int MaxRating
            {
                get => 
                    ((Rating) this._parent.Component).MaxRating;
                set
                {
                    try
                    {
                        TypeDescriptor.GetProperties(this._parent.Component)["MaxRating"].SetValue(this._parent.Component, value);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            public bool RealOnly
            {
                get => 
                    ((Rating) this._parent.Component).ReadOnly;
                set
                {
                    TypeDescriptor.GetProperties(this._parent.Component)["ReadOnly"].SetValue(this._parent.Component, value);
                }
            }

            public int StartRating
            {
                get => 
                    ((Rating) this._parent.Component).CurrentRating;
                set
                {
                    try
                    {
                        TypeDescriptor.GetProperties(this._parent.Component)["CurrentRating"].SetValue(this._parent.Component, value);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }
    }
}

