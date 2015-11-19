module proteca {
    'use strict';

    export class UsrUtilisateur {
        identifiant: string;
        nom: string;
        prenom: string;
        mail: string;
        societe: string;
        gestion: GestionUtilisateur;
        estSupprime: boolean;
        estPrestataire: boolean;
        profil: Profil;
        region: Region;
        agence: Agence;
        secteur: Secteur;
    }
}
