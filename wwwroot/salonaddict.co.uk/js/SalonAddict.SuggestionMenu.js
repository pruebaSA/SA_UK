/// <reference name="MicrosoftAjax.js"/>

Type.registerNamespace("SalonAddict");

SalonAddict.SuggestionMenu = function(id, textboxId) {
    SalonAddict.SuggestionMenu.initializeBase(this);
    this._ID = id;
    this._TextBoxID = textboxId;
    this._HtmlTextWriter = new Array();
}

SalonAddict.SuggestionMenu.prototype =
{
    renderControls: SuggestionMenu$renderControls,

    show: SuggestionMenu$show,
    
    message : SuggestionMenu$message,
    
    clear : SuggestionMenu$clear,
    
    close : SuggestionMenu$close
}

function SuggestionMenu$renderControls(suggestions) {
    for (var i = 0; i < suggestions.length; i++) {
        var suggestion = suggestions[i];

        if (suggestion.Keywords.length > 0) {

            var title = document.createElement("div");
            var title_text = document.createTextNode(suggestion.Description);
            title.className = "title";
            title.appendChild(title_text);

            var ul = document.createElement("ul");

            for (var j = 0; j < suggestion.Keywords.length; j++) {
                var keyword = suggestion.Keywords[j];
                var li = document.createElement("li");
                li.onmouseover = function() {
                    this.className = "selected";
                }
                li.onmouseout = function() {
                    this.className = "";
                }

                var instance = this;
                li.onclick = function() {
                    var txtKeyword = document.getElementById(instance._TextBoxID);
                    txtKeyword.value = SalonAddict.Utilities.HtmlDecode(this.childNodes[0].innerHTML);
                    instance.close();
                }

                var item = document.createElement("div");
                var item_text = document.createTextNode(keyword);
                item.appendChild(item_text);
                
                li.appendChild(item);
                ul.appendChild(li);
            }
            
            this._HtmlTextWriter.push(title);
            this._HtmlTextWriter.push(ul);
        }
    }
}

function SuggestionMenu$message(value) {
    var title = document.createElement("div");
    var title_text = document.createTextNode(value);
    title.className = "title";
    title.appendChild(title_text);
    this._HtmlTextWriter.push(title);
}

function SuggestionMenu$show() 
{
    var menu = document.getElementById(this._ID);
    for(var i = 0; i < this._HtmlTextWriter.length; i++)
    {
        var child = this._HtmlTextWriter[i];
        menu.appendChild(child);
    }
    $(menu).fadeIn();
}

function SuggestionMenu$clear() 
{
    var menu = document.getElementById(this._ID);
    menu.innerHTML = "";
}

function SuggestionMenu$close() {
    var menu = document.getElementById(this._ID);
    $(menu).fadeOut();
}