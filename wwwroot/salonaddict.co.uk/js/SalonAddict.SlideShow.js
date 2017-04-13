/// <reference name="MicrosoftAjax.js"/>

Type.registerNamespace("SalonAddict");
Type.registerNamespace("SalonAddict.SlideShow");

SalonAddict.SlideShow.Player = function(viewOnePanelID, viewTwoPanelID, captionPanelID, items) 
{
    SalonAddict.SlideShow.Player.initializeBase(this);
    this._SyncRoot = false;
    this._Interval = null;
    this._Index = 0;
    this._Speed = 6000;
    this._Items = items;
    this._ZIndex = -1;
    this._ActiveViewIndex = 1;
    this._ViewOnePanelID = viewOnePanelID;
    this._ViewTwoPanelID = viewTwoPanelID;
    this._CaptionPanelID = captionPanelID;
}

SalonAddict.SlideShow.Player.prototype =
{
    __show: SlideShowPlayer$show,
    
    initialize: SlideShowPlayer$initialize,
   
    set_Speed: SlideShowPlayer$set_Speed,
        
    set_Index: SlideShowPlayer$set_Index,
    
    get_Item: SlideShowPlayer$get_Item,
        
    previous: SlideShowPlayer$previous,
    
    next: SlideShowPlayer$next,
        
    animate: SlideShowPlayer$animate,
    
    dispose: SlideShowPlayer$dispose
}

function SlideShowPlayer$initialize() {
    SalonAddict.SlideShow.Player.callBaseMethod(this,"initialize");
}

function SlideShowPlayer$set_Speed(value) {
    SalonAddict.SlideShow.__SlideSpeed = value;
}

function SlideShowPlayer$get_Item() {
    return this._Items[this._Index];
}

function SlideShowPlayer$set_Index(value) {
    this._Index = value;
}

function SlideShowPlayer$previous() {
    var index = this._Index - 1;
    if(index < 1)
    {
       index = this._Items.length;
    }
    this.__show(index);
}

function SlideShowPlayer$next() {
    var index = this._Index + 1;
    if(index > this._Items.length )
    {
       index = 1;
    }
    this.__show(index);
}

function SlideShowPlayer$show(x) {
   if(this._SyncRoot)
   {
      return;
   }   
   this._SyncRoot = true;
   var item = this._Items[x - 1];
   var index = (this._ActiveViewIndex == 1)? 2 : 1;
   var zindex = this._ZIndex - 1;
   var newslide = ((index == 1)? this._ViewOnePanelID : this._ViewTwoPanelID);

   $("#" + newslide).css({
	    "background-image" : "url(" + item.slide + ")",
		"display" : "block",
		"z-index" : zindex
   });
  
   this._ZIndex = zindex;
   var cpid = this._CaptionPanelID;
   var current = ((this._ActiveViewIndex == 1)? this._ViewOnePanelID : this._ViewTwoPanelID);
   var unlock = Function.createDelegate(this, function() { this._SyncRoot = false; });

   window.setTimeout(function() {
       $("#" + cpid).css({"display" : "none"});
       $("#" + current).fadeOut(1500, function() {
	        setTimeout(function() {
               $("#" + cpid).css({"background-image" : "url(" + item.caption + ")"});
		        $("#" + cpid).fadeIn("slow");
		        unlock();
	        }, 500);
        });
   }, 1000);
   
   this._Index = x;
   this._ActiveViewIndex = index;
}

function SlideShowPlayer$animate(value) {
    if(value == false)
    {
        window.clearInterval(this._Interval);
    }
    else
    {
        var delay = this._Speed;
        var next = Function.createDelegate(this, function() { this.next(); });
        this._Interval = window.setInterval(
            next, 
            delay
        );
    }
}

function SlideShowPlayer$get_ActiveContainer() {
    return this._ActiveContainer;
}

function SlideShowPlayer$dispose() {
    this.animate(false);
    this._Interval = null;
    this._Items = null;
    this._Index = null;
    this._ZIndex = null;
    this._ActiveViewIndex = null;
    this._ViewOnePanelID = null;
    this._ViewTwoPanelID = null;
    this._CaptionPanelID = null;
    SalonAddict.SlideShow.Player.callBaseMethod(this, "dispose");
}

SalonAddict.SlideShow.Player.registerClass("SalonAddict.SlideShow.Player", Sys.Component, Sys.IDisposable);