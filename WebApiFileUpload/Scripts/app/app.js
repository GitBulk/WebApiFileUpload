'use strict';

var module = angular.module("myApp", ['ngRoute', 'angularFileUpload']);

module.config(function ($routeProvider) {
    $routeProvider.when('/', {
        templateUrl: 'Scripts/app/templates/fileUpload.html',
        controller: 'fileUploadController'
    }).otherwise({
        redirectTo: '/'
    });
});
