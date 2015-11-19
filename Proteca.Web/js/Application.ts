/// <reference path='_all.ts' />

/**
 * The main Proteca app module.
 *
 * @type {angular.Module}
 */
module proteca {
    'use strict';
    var proteca = angular.module('proteca', [
        'ngRoute',
        'jaydata',
        'ui.bootstrap',
        'ngResource'
    ]);
    proteca.controller('UtilisateurCtrl', UtilisateurCtrl);
    proteca.config(['$routeProvider', function ($routeProvider) {
        $routeProvider
            .when('/utilisateur', {
                controller: ("UtilisateurCtrl", UtilisateurCtrl),
                templateUrl: 'View/usrUtilisateur.html'
            })
            .when('/', {
                controller: ("UtilisateurCtrl", UtilisateurCtrl),
                templateUrl:"test.html"
            })
            .otherwise({
                redirectTo: '/'
            });
    }]);
}
