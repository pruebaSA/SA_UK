/// <reference name="MicrosoftAjax.js"/>

Type.registerNamespace("SalonAddict");
Type.registerNamespace("SalonAddict.Utilities");

SalonAddict.Utilities.CharacterCounter = function(txt, label)
{
    var maxlength = $(txt).attr("maxLength");
    if(maxlength > 0)
    {
        var count = $(txt).val().length;
        if(count > maxlength)
        {
           $(txt).val($(txt).val().substring(0, maxlength));
           count = maxlength;
        }
        $(label).html(maxlength - count);
    }
}

SalonAddict.Utilities.HtmlEncode = function(value) {
    if (value) {
        return $('<div />').text(value).html();
    } else {
        return '';
    }
}

SalonAddict.Utilities.HtmlDecode = function(value) {
    if (value) {
        return $('<div />').html(value).text();
    } else {
        return '';
    }
}