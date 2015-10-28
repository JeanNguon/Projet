module proteca {
    'use strict';

    export interface IProtecaScope extends ng.IScope {
        proteca: ProtecaItem[];
        newUsrUtilisateur: UsrUtilisateur;
        editedUsrUtilisateur: UsrUtilisateur;
        location: ng.ILocationService;
        viewModel: ProtecaCtrl;
    }
}