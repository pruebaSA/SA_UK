Type.registerNamespace("Sys.Extended.UI.HTMLEditor.ToolbarButton");Sys.Extended.UI.HTMLEditor.ToolbarButton.Underline=function(a){Sys.Extended.UI.HTMLEditor.ToolbarButton.Underline.initializeBase(this,[a])};Sys.Extended.UI.HTMLEditor.ToolbarButton.Underline.prototype={callMethod:function(){if(!Sys.Extended.UI.HTMLEditor.ToolbarButton.Underline.callBaseMethod(this,"callMethod"))return false;this._designPanel._execCommand("underline",false,null)},checkState:function(){return!Sys.Extended.UI.HTMLEditor.ToolbarButton.Underline.callBaseMethod(this,"checkState")?false:this._designPanel._queryCommandState("underline")}};Sys.Extended.UI.HTMLEditor.ToolbarButton.Underline.registerClass("Sys.Extended.UI.HTMLEditor.ToolbarButton.Underline",Sys.Extended.UI.HTMLEditor.ToolbarButton.EditorToggleButton);