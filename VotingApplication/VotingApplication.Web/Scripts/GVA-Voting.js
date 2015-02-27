﻿(function () {
    angular.module('GVA.Voting', ['ngRoute', 'ngDialog', 'ngStorage', 'GVA.Common']).config(['$routeProvider', function ($routeProvider) {
        $routeProvider.
            when('/Vote/:pollId', {
                templateUrl: '../Routes/Vote'
            })
            .when('/Results/:pollId', {
                templateUrl: '../Routes/Results'
            })
    }]);
})();
