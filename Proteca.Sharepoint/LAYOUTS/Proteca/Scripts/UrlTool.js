var Proteca = Proteca || {};
Proteca.UrlTool = function () {
    //// Private functions
    function __getQueryParams() {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            if (hash != null) { hash[0] = hash[0].toLowerCase(); }
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    }

    function __getQueryParam(name) {
        if (name != null) { name = name.toLowerCase(); }
        return __getQueryParams()[name];
    }
    //// Exports
    var __intf =
    {
        getQueryParam: __getQueryParam
    };
    return __intf;
} ();