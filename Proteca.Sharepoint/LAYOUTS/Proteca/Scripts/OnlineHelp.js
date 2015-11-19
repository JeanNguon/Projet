Proteca.Help.OnlineHelp = function (base) {
    //// Private members
    var LIST_NAME = "OnlineHelp";
    var __containerId, __helpContext;

    function __run(containerId, helpContext) {
        __containerId = containerId;
        __helpContext = helpContext;
        __loadTopic();
    }

    function __loadTopic() {
        var queryXml =
            "<View>" +
                "<Query>" +
                    "<Where><IsNull>" +
                        "<FieldRef Name='TopicKey'/>" +
                    "</IsNull></Where>" +
                "</Query>" +
            "</View>";

        if (__helpContext) {
            queryXml =
            "<View>" +
                "<Query>" +
                    "<Where><Eq>" +
                        "<FieldRef Name='TopicKey'/>" +
                        "<Value Type='Text'>" + __helpContext + "</Value>" +
                    "</Eq></Where>" +
                "</Query>" +
            "</View>";
        }

        base.queryList(
            LIST_NAME,
            null, //folderName
            queryXml,
            __processResult,
            __processError
        )
    }

    function __processResult(queryResult) {
        if (queryResult.get_count() > 0) {
            var listEnumerator = queryResult.getEnumerator();
            listEnumerator.moveNext();
            var item = listEnumerator.get_current();
            var content = item.get_fieldValues().TopicContent;
            var title = item.get_fieldValues().Title;
            $("#title").html("<h2>Aide en ligne: " + title + "</h2>");
            $("#" + __containerId).html(content);
        }
        else {
            // Try to find the parent topic, if any.
            var lastPosOfSeparator = __helpContext.lastIndexOf("/");
            if (lastPosOfSeparator != -1) {
                __helpContext = __helpContext.slice(0, lastPosOfSeparator);
                __loadTopic();
            }
            else //Not Found, already at the root.
                base.showFailure(__containerId, "Help topic not found: " + __helpContext);
        }
    }

    function __processError() {
        base.showFailure(__containerId, "Failed to load help topic: " + __helpContext);
    }

    //// Exports
    var __intf =
    {
        run: __run
    };
    return __intf;
} (Proteca.Help.Base);

$(document).ready(function () {
    var containerId = "HelpContent";
    var helpContext = decodeURIComponent(Proteca.UrlTool.getQueryParam("helpContext"));
    Proteca.Help.OnlineHelp.run(containerId, helpContext);
});