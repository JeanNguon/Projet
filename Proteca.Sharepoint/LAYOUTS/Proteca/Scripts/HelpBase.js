var Proteca = Proteca || {};
Proteca.Help = Proteca.Help || {};

Proteca.Help.Base = function () {
    //// Private members
    function __queryList(listName, folderName, query, onSuccess, onFailure, context) {
        var ctx = SP.ClientContext.get_current();
        var web = ctx.get_web();
        var list = web.get_lists().getByTitle(listName);

        var camlQuery = new SP.CamlQuery();
        if (query != null)
            camlQuery.set_viewXml(query);
        if (folderName != null)
            camlQuery.set_folderServerRelativeUrl("Lists/" + listName + "/" + folderName);
        var queryResult = list.getItems(camlQuery);
        ctx.load(queryResult);
        ctx.executeQueryAsync(
            function () {
                onSuccess(queryResult, context);
            },
            function (sender, args) {
                onFailure(args, context);
            }
        )
    }

    function __showFailure(containerId, message) {
        $("#" + containerId).html("<p style='color:#FF0000;font-size=140%;'>" + message + "</p>");
    }

    //// Exports
    var __intf =
    {
        queryList: __queryList,
        showFailure: __showFailure

    };
    return __intf;
} ();