/// <reference path='_all.ts' />
/**
 * The main Proteca app module.
 *
 * @type {angular.Module}
 */
var proteca;
(function (proteca_1) {
    'use strict';
    var proteca = angular.module('proteca', [
        'ngRoute',
        'jaydata',
        'ui.bootstrap',
        'ngResource'
    ]);
    proteca.controller('UtilisateurCtrl', proteca_1.UtilisateurCtrl);
    proteca.config(['$routeProvider', function ($routeProvider) {
            $routeProvider
                .when('/utilisateur', {
                controller: ("UtilisateurCtrl", proteca_1.UtilisateurCtrl),
                templateUrl: 'View/usrUtilisateur.html'
            })
                .when('/', {
                controller: ("UtilisateurCtrl", proteca_1.UtilisateurCtrl),
                templateUrl: "test.html"
            })
                .otherwise({
                redirectTo: '/'
            });
        }]);
})(proteca || (proteca = {}));
