var proteca;
(function (proteca) {
    'use strict';
    var ProtecaItem = (function () {
        function ProtecaItem(usrUtilisateur) {
            this.usrUtilisateur = usrUtilisateur;
        }
        return ProtecaItem;
    })();
    proteca.ProtecaItem = ProtecaItem;
})(proteca || (proteca = {}));
