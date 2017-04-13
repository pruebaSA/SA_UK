//------------------------------------------------------------------------------
// SalonAddict
//------------------------------------------------------------------------------

var SAMessageTokens_command = function(name) {
    this.Name = name;
}

SAMessageTokens_command.prototype.Execute = function(itemText, itemLabel) {
    if (itemText != "")
        FCK.InsertHtml(itemText);
}

SAMessageTokens_command.prototype.GetState = function() {
    return;
}

FCKCommands.RegisterCommand('messagetokenscommand', new SAMessageTokens_command('any_name'));

var FCKToolbarSAMessageTokens = function(tooltip, style) {
    this.Command = FCKCommands.GetCommand('messagetokenscommand');
    this.CommandName = 'messagetokenscommand';
    this.Label = this.GetLabel();
    this.Tooltip = tooltip ? tooltip : this.Label;
    this.Style = style;
    this.FieldWidth = 200;
    this.PanelWidth = 200;
};

FCKToolbarSAMessageTokens.prototype = new FCKToolbarSpecialCombo;

//Label to appear in the FCK toolbar
FCKToolbarSAMessageTokens.prototype.GetLabel = function() {
    return "Message&nbsp;Tokens";
};

//Retrieve tokens from xml and add to combo
FCKToolbarSAMessageTokens.prototype.CreateItems = function() {
    var tokensXmlPath = FCKConfig.SATokensXmlPath;

    if(tokensXmlPath == null)
    {
        return;
    }
    
    // Load the XML file into a FCKXml object.
    var xml = new FCKXml();
    xml.LoadUrl(tokensXmlPath);

    var tokensXmlObj = FCKXml.TransformToObject(xml.SelectSingleNode('Tokens'));

    // Get the "Token" nodes defined in the XML file.
    var tokenNodes = tokensXmlObj.$Token;

    // Add each style to our "Tokens" collection.
    for (var i = 0; i < tokenNodes.length; i++) {
        var tokenNode = tokenNodes[i];

        var tokenValue = (tokenNode.value || '').toLowerCase();

        if (tokenValue.length == 0)
            throw ('The element name is required. Error loading "' + tokensXmlPath + '"');

        // Set styles and labels to the dropdown
        this._Combo.AddItem(tokenNode.value, '<span style="color:#000000;font-weight: normal; font-size: 12px;">' + tokenNode.name + '</span>');
    }
}

//Register the combo with the FCKEditor
FCKToolbarItems.RegisterItem('SAMessageTokens', new FCKToolbarSAMessageTokens('SA Message Tokens', FCK_TOOLBARITEM_ICONTEXT)); //or FCK_TOOLBARITEM_ONLYTEXT


