'use strict';
var app = angular.module('myApp', ['angularFileUpload']);
app.controller('fileUploadController', function ($scope, $http, $timeout, $upload) {
    $scope.upload = [];
    $scope.uploadedFiles = [];
    $scope.startUploading = function ($files) {
        console.log($files);
        for (var i = 0; i < $files.length; i++) {
            var $file = $files[i];
            (function (index) {
                $scope.upload[index] = $upload.upload({
                    url: './api/fileUpload', // web api url
                    method: 'POST',
                    file: $file
                }).progress(function (e) {
                }).success(function (data, status, headers, config) {
                    $scope.uploadedFiles.push({
                        fileName: data.FileName,
                        filePath: data.LocalFilePath,
                        fileLength: data.FileLength
                    });
                }).error(function (data, status, headers, config) {
                    console.log(data);
                });
            })(i);
        }
    }
});