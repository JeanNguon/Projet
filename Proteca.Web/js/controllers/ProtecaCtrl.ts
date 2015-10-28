/// <reference path='../_all.ts' />

module proteca {
    interface IProtecaControllerScopeUsr extends ng.IScope {
        proteca: ProtecaCtrl;
        context: any;
        utilisateurs: Array<proteca.UsrUtilisateur>;
        selectedUsrUtilisateur: any;
        save: Function;
        saveChanges: Function;
    }

    export class ProtecaCtrl {

        //injection annotation
        static $inject = [
            '$scope',
            '$data',
        //'$location',
        //'protecaStorage',
            '$http'

        ];
        constructor(
            public $scope: IProtecaControllerScopeUsr,
            public $data: any,
            //private $location: ng.ILocationService,
            //private protecaStorage: IProtecaStorage,
            private $http,
            private selectedUsrUtilisateur: any
        ) {
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
                    })
            })
           
     
        $http.get(
                "http://localhost:65317/WcfDataServiceProteca.svc/UsrUtilisateur")
                .success(function (response) {
            $scope.context = response;
            for (var variable in response.value) {
                $scope.utilisateurs = response.value;
                //console.log(response.value);
            }
            $scope.save = function () {
                try {
                    // $scope.context.add($data.EntityState.Modified);
                    alert('success add');
                } catch (e) {
                    console.error("externe", e.message);
                }
                $scope.saveChanges();
            }
        });
        //add new usrUtilisateur
        $scope.save = function () {
            try {
                $scope.selectedUsrUtilisateur.add($data.EntityState.Modified);
                alert('success add');
            } catch (e) {
                console.error("externe", e.message);
            }
            $scope.saveChanges();
        }
        $scope.saveChanges = function () {
            // $scope.utilisateurs.saveChanges();
        }
        //removeUsrUtilisateur(usrUtilisateur: UsrUtilisateur) {
        //}

    }
}
}