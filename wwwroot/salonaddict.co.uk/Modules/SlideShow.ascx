<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SlideShow.ascx.cs" Inherits="SalonAddict.Modules.SlideShow" %>
<%@ Register TagPrefix="SA" TagName="Search" Src="~/Templates/SearchPanelOne.ascx" %>

<div class="wrapper-slideshow" >
    <div id="ss-slide1" class="slide" ></div>
    <div id="ss-slide2" class="slide" ></div>
    <div id="ss-caption" class="caption" >
       <style type="text/css" >
          .wrapper-slideshow #ss-slide1 { background-image:url('<%= base.GetLocalResourceObject("Slide1") %>'); }
          .wrapper-slideshow #ss-caption { background-image:url('<%= base.GetLocalResourceObject("Caption1") %>'); }
       </style>
    </div>
    <div class="player-control" >
      <input id="ss-back" type="button" />
      <input id="ss-play" type="button" class="ss-pause" />
      <input id="ss-next" type="button" />
    </div>
    <div class="wrapper-search" >
       <SA:Search ID="Search" runat="server" />
    </div>
</div>
<asp:ScriptManagerProxy ID="smp" runat="server" >
  <Scripts>
      <asp:ScriptReference Path="~/js/SalonAddict.SlideShow.js" />
  </Scripts>
</asp:ScriptManagerProxy>
<script type="text/javascript" language="javascript" >
var slides = [ {
		'slide': '<%= base.GetLocalResourceObject("Slide1") %>',
		'caption': '<%= base.GetLocalResourceObject("Caption1") %>',
		'description': '<%= base.GetLocalResourceObject("Description1") %>'
	}, {
		'slide': '<%= base.GetLocalResourceObject("Slide2") %>',
		'caption': '<%= base.GetLocalResourceObject("Caption2") %>',
		'description': '<%= base.GetLocalResourceObject("Description2") %>'
	}, {
		'slide': '<%= base.GetLocalResourceObject("Slide3") %>',
		'caption': '<%= base.GetLocalResourceObject("Caption3") %>',
		'description': '<%= base.GetLocalResourceObject("Description3") %>'
	}, {
		'slide': '<%= base.GetLocalResourceObject("Slide4") %>',
		'caption': '<%= base.GetLocalResourceObject("Caption4") %>',
		'description': '<%= base.GetLocalResourceObject("Description4") %>'
	}, {
		'slide': '<%= base.GetLocalResourceObject("Slide5") %>',
		'caption': '<%= base.GetLocalResourceObject("Caption5") %>',
		'description': '<%= base.GetLocalResourceObject("Description5") %>'
	}, {
		'slide': '<%= base.GetLocalResourceObject("Slide6") %>',
		'caption': '<%= base.GetLocalResourceObject("Caption6") %>',
		'description': '<%= base.GetLocalResourceObject("Description6") %>'
	}
];

$(document).ready(function() {
    var slideshow = new SalonAddict.SlideShow.Player("ss-slide1","ss-slide2","ss-caption", slides);
    slideshow.set_Index(1);
    
    $("#ss-back").click(function() {
	    slideshow.previous();
	});
	
	$("#ss-next").click(function() {
	    slideshow.next();
	});
	
	$("#ss-play").toggle(function(){
		$(this).attr("class","ss-play");
		slideshow.animate(false);
	}, function() {
		$(this).attr("class","ss-pause");
		slideshow.next();
		slideshow.animate(true);
	});
	
    slideshow.animate(true);
});
</script>