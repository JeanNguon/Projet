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
    ]);
    proteca.controller('protecaCtrl', proteca_1.ProtecaCtrl);
    proteca.config(['$routeProvider', function ($routeProvider) {
            $routeProvider
                .when('http://localhost:65317/test.html', {
                controller: ("protecaCtrl", proteca_1.ProtecaCtrl),
                templateUrl: 'View/usrUtilisateur.html'
            })
                .otherwise({
                redirectTo: '/'
            });
        }]);
})(proteca || (proteca = {}));
