Proteca.Help.DiagnosticHelp = function (base) {
    //// Private members
    var LIST_NAME = "DiagnosticHelp";
    var __containerId;

    function __run(containerId) {
        __containerId = containerId;
        __loadGroups();
    }

    function __loadGroups() {
        var queryXml =
            "<View>" +
                "<Query>" +
                    "<OrderBy>" +
                        "<FieldRef Name='Title' Ascending='True' />" +
                    "</OrderBy>" +
                "</Query>" +
            "</View>";

        base.queryList(
            LIST_NAME,
            null,
            queryXml,
            __processGroups,
            __processError
        );
    }

    function __loadTopics(groupTitle, rootElementId) {
        var queryXml =
            "<View>" +
                "<Query>" +
                    "<OrderBy>" +
                        "<FieldRef Name='Title' Ascending='True' />" +
                    "</OrderBy>" +
                "</Query>" +
            "</View>";

        base.queryList(
            LIST_NAME,
            groupTitle,
            queryXml,
            __processTopics,
            __processError,
            rootElementId
        );
    }


    function __processGroups(items) {
        var html = "<ul class='topic-groups'>";
        var itemEnumerator = items.getEnumerator();
        while (itemEnumerator.moveNext()) {
            var item = itemEnumerator.get_current();
            if (item.get_fileSystemObjectType() == 1) // type is folder
            {
                var groupTitle = item.get_fieldValues().Title;
                var groupDescription = item.get_fieldValues().GroupDescription;
                var groupId = item.get_fieldValues().UniqueId;
                var groupTopicsRootElementId = "topic-" + groupId;
                html += "<li class='topic-group'>";
                html += "   <div class='group-title'>";
                html += "       <div class='left'></div>"
                html += "       <div class='middle'>" + groupTitle + "</div>";
                html += "       <div class='right'></div>"
                html += "   </div>";
                html += "   <div class='group-desc'>" + groupDescription + "</div>";
                html += "   <ul class='topic-items' id='" + groupTopicsRootElementId + "'>";
                html += "   </ul>";
                html += "</li>";
                /* This is not sure enough given the asynchronous nature of the call. Though very unlikely, the element groupTopicsRootElementId
                 * may not have been created by the time of creating child items. See comment below.
                 */
                // __loadTopics(groupTitle, groupTopicsRootElementId); 
            }
        }
        html += "</ul>";
        $("#" + __containerId).html(html);
        //// To ensure that the "pegs" have well been created in the DOM tree, we need to loop again.
        itemEnumerator.reset();
        while (itemEnumerator.moveNext()) {
            var item = itemEnumerator.get_current();
            if (item.get_fileSystemObjectType() == 1) // type is folder
            {
                var groupTitle = item.get_fieldValues().Title; 
                var groupId = item.get_fieldValues().UniqueId;
                var groupTopicsRootElementId = "topic-" + groupId;
                __loadTopics(groupTitle, groupTopicsRootElementId);
            }
        }
    }

    function __processTopics(items, rootElementId) {
        var itemEnumerator = items.getEnumerator();
        var html = ""; // Note that «var html = null;» or «var html;» does NOT work!
        while (itemEnumerator.moveNext()) {
            var item = itemEnumerator.get_current();
            if (item.get_fileSystemObjectType() == 0) // type is item
            {
                var topicTitle = item.get_fieldValues().TopicKey;
                var topicContent = item.get_fieldValues().TopicContent;
                html += "<li class='topic-item'>";
                html += "   <h3 class='topic-title'>" + topicTitle + "</h3>";
                html += "   <div class='topic-content'>" + topicContent + "</div>";
                html += "</li>";
            }
        }
        $("#" + rootElementId).html(html);
    }

    function __processError() {
        base.showFailure(__containerId, "Failed to load help topics");
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
    Proteca.Help.DiagnosticHelp.run(containerId);
});