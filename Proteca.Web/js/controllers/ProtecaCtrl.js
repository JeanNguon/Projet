/// <reference path='../_all.ts' />
var proteca;
(function (proteca) {
    var ProtecaCtrl = (function () {
        function ProtecaCtrl($scope, $data, 
            //private $location: ng.ILocationService,
            //private protecaStorage: IProtecaStorage,
            $http, selectedUsrUtilisateur) {
            this.$scope = $scope;
            this.$data = $data;
            this.$http = $http;
            this.selectedUsrUtilisateur = selectedUsrUtilisateur;
            //get apiKey
            $data.initService('http://localhost:65317/WcfDataServiceProteca.svc')
                .then(function (remoteDB, contextFactory) {
                window['protecaRemote'] = remoteDB;
                var localDB = contextFactory({ name: 'local', databaseName: 'Proteca' });
                $.when(remoteDB.onReady(), localDB.onReady())
                    .then(function () {
                    for (var user in proteca) {
                        console.log(proteca[user]);
                    }
                });
            });
            $http.get("http://localhost:65317/WcfDataServiceProteca.svc/UsrUtilisateur")
                .success(function (response) {
                $scope.context = response;
                for (var variable in response.value) {
                    $scope.utilisateurs = response.value;
                }
                $scope.save = function () {
                    try {
                        // $scope.context.add($data.EntityState.Modified);
                        alert('success add');
                    }
                    catch (e) {
                        console.error("externe", e.message);
                    }
                    $scope.saveChanges();
                };
            });
            //add new usrUtilisateur
            $scope.save = function () {
                try {
                    $scope.selectedUsrUtilisateur.add($data.EntityState.Modified);
                    alert('success add');
                }
                catch (e) {
                    console.error("externe", e.message);
                }
                $scope.saveChanges();
            };
            $scope.saveChanges = function () {
                // $scope.utilisateurs.saveChanges();
            };
            //removeUsrUtilisateur(usrUtilisateur: UsrUtilisateur) {
            //}
        }
        //injection annotation
        ProtecaCtrl.$inject = [
            '$scope',
            '$data',
            //'$location',
            //'protecaStorage',
            '$http'
        ];
        return ProtecaCtrl;
    })();
    proteca.ProtecaCtrl = ProtecaCtrl;
})(proteca || (proteca = {}));
