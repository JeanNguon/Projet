/// <reference path='../_all.ts' />
module proteca {
    interface IProtecaControllerScopeUsr extends ng.IScope {
        proteca: UtilisateurCtrl;
        context: any;
        utilisateurs: any;
        selectedUsrUtilisateur: any;
        deleteUrsUtilisateur: Function;
        save: Function;
        saveChanges: Function;
        newUsrUtilisateur: Function;
        remove: Function;
        selectUser: Function;
        categories: any;
    }

    export class UtilisateurCtrl {

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
            $scope.categories = [];
            $data.initService("http://localhost:65317/WcfProtecaDataService.svc")
                .then(function (context) {
                    $scope.utilisateurs = context.UsrUtilisateur.toLiveArray();
                });

            ////get the context to our remoteCtx database using JayData
      
            //var remoteCtx = new Proteca.Web.Models.ProtecaEntities({
            //    name: "oData",
            //    oDataServiceHost: "http://localhost:65317/WcfProtecaDataService.svc",
            //    //user: "rd.gop\\grtgaz-admin",
            //    //password: "3tUhADAt"
            //});

            ////when the context is ready, get all UsrUtilisateurs then give it to Angular
            //remoteCtx
            //    .onReady()
            //    .then(function (context) {
            //        $scope.context = context;
            //        //$scope.utilisateurs = context.UsrUtilisateur.toLiveArray();
            //        $scope.utilisateurs = context.UsrUtilisateur.toArray();
            //     }
            //    )
            //    .fail(error => { alert("UsrUtilisateur"); }); 
  
           
            ////////////////////
            /////Edit Product///
            ////////////////////
            $scope.save = function () {
                alert('save');
                if ($scope.selectUser) {
                    $scope.context.UsrUtilisateur.attach($scope.selectedUsrUtilisateur, true);
                    alert('Utilisateur modifié');
                    $scope.utilisateurs.entityState = $data.EntityState.Modified;
                    
                }
                else {
                    $scope.context.UsrUtilisateur.add($scope.selectedUsrUtilisateur);
                    alert('modification enregistrée');
                }
                $scope.saveChanges();
            };
            $scope.saveChanges = function () {
                $scope.context.saveChanges()
                    .then(function () {
                        $scope.selectedUsrUtilisateur = null;
                    }, function () {
                        $scope.context.stateManager.reset();
                    });
            };
        
            //remove UrsUtilisateur 
            $scope.remove = function () {
                $scope.context.remove($scope.selectedUsrUtilisateur);
                $scope.saveChanges();
            };
            //add new UsrUtilisatation
            $scope.newUsrUtilisateur = function () {
                $scope.selectedUsrUtilisateur = new $scope.context.UsrUtilisateur.add($scope.selectedUsrUtilisateur.Identifiant);
                alert('utilisateur ajouté');
            }
            $scope.selectUser = function (user: any) {
                $scope.selectedUsrUtilisateur = user;
                alert('utilisateur sélectionné ' + $scope.selectedUsrUtilisateur.Nom);
            }
        }
    }
}