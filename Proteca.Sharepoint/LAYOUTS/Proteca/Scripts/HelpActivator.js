//// Name spaces
var Proteca = Proteca || {};
Proteca.Help = Proteca.Help || {};

Proteca.Help.Activator = function () {
    //// Private members
    function __showPopup(title, url) {
        var WIN_W = 900;
        var WIN_H = document.getElementById('s4-workspace').offsetHeight - 40; // auto-adjusted
        var options = SP.UI.$create_DialogOptions();
        options.title = title;
        options.width = WIN_W;
        options.height = WIN_H;
        options.resizable = 1;
        options.scroll = 1;
        options.url = url;
        SP.UI.ModalDialog.showModalDialog(options);
    }

    function __showOnlineHelp() {
        var helpContext = document.location.hash.substring(2);
        __showPopup('Proteca: aide en ligne', '/Pages/System/OnlineHelp.aspx?helpContext=' + encodeURIComponent(helpContext));
    }

    function __showDiagnosticHelp() {
        __showPopup('Proteca: aide au diagnostic', '/Pages/System/DiagnosticHelp.aspx');
    }

    function __showGlossary() {
        __showPopup('Proteca: glossaire', '/Pages/System/Glossary.aspx');
    }
    //// Exports
    var __intf =
    {
        showOnlineHelp: __showOnlineHelp,
        showDiagnosticHelp: __showDiagnosticHelp,
        showGlossary: __showGlossary
    };
    return __intf;
} ();