module proteca {
    
    'use strict';
    export class ProtecaItem {
        constructor(
            private usrUtilisateur: UsrUtilisateur,
            private agence: Agence,
            private profil: Profil,
            private secteur: Secteur,
            private region: Region
        ) { }
    }
}