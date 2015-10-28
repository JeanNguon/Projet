/// <reference path='../_all.ts' />
var proteca;
(function (proteca) {
    var ProtecaCtrl = (function ($scope, $data) {
        this.$scope = $scope;
        this.$data = $data;
        $scope.usrUtilisateurs = [];
        $data.initService("http://localhost:65317/WcfDataServiceProteca.svc")
        .then(function (proteca) {
            $scope.proteca = proteca;
            $scope.usrUtilisateurs = proteca.UsrUtilisateur.toLiveArray();
        });
        return ProtecaCtrl;
    })();
    proteca.ProtecaCtrl = ProtecaCtrl;
})(proteca || (proteca = {}));
