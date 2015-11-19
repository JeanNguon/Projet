var proteca;
(function (proteca) {
    'use strict';
    var ProtecaItem = (function () {
        function ProtecaItem(usrUtilisateur, agence, profil, secteur, region) {
            this.usrUtilisateur = usrUtilisateur;
            this.agence = agence;
            this.profil = profil;
            this.secteur = secteur;
            this.region = region;
        }
        return ProtecaItem;
    })();
    proteca.ProtecaItem = ProtecaItem;
})(proteca || (proteca = {}));
