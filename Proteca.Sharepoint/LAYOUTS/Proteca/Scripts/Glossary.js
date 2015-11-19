Proteca.Help.Glossary = function (base) {
    //// Private members
    var LIST_NAME = "Glossary";
    var __containerId;

    function __run(containerId) {
        __containerId = containerId;
        __loadTopics();
    }

    function __loadTopics() {
        var queryXml =
            "<View>" +
                "<Query>" +
                    "<OrderBy>" +
                        "<FieldRef Name='TopicKey' Ascending='True' />" +
                    "</OrderBy>" +
                "</Query>" +
            "</View>";

        base.queryList(
            LIST_NAME,
            null, //folderName
            queryXml,
            __processResult,
            __processError
        );
    }

    function __processResult(queryResult) {
        var listEnumerator = queryResult.getEnumerator();
        var html = "<table>";
        while (listEnumerator.moveNext()) {
            var item = listEnumerator.get_current();
            var key = item.get_fieldValues().TopicKey;
            var content = item.get_fieldValues().TopicContent;
            html += "<tr><td class='keycol'>" + key + "</td><td class='contentcol'>" + content + "</td></tr>";
        }
        html += "</table>";
        $("#" + __containerId).html(html);
    }

    function __processError(args, context) {
        base.showFailure(__containerId, "Failed to load glossary: [" + args.get_message() + "]");
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
    Proteca.Help.Glossary.run(containerId);
});