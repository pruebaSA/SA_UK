// prevents events from bubbling up
function no_propagation(e) 
{
	if (!e)
	{ 
	   e = window.event
	}
	e.cancelBubble = true;
	if (e.stopPropagation) 
	{
	   e.stopPropagation();
	}
}