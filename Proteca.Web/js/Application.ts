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
        //'ui-bootstrap'
    ]);

    proteca.controller('protecaCtrl', ProtecaCtrl);
   
    proteca.config(['$routeProvider', function ($routeProvider) {
        $routeProvider
            .when('http://localhost:65317/test.html', {
                controller: ("protecaCtrl", ProtecaCtrl),
                templateUrl: 'View/usrUtilisateur.html'
            })
            .otherwise({
                redirectTo: '/'
            });
    }]);
 
}
