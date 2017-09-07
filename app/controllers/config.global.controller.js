angular.module('umbraco').controller('ArchetypeConfigGlobalOptionsController', function ($scope, archetypeGlobalConfigService) {
    $scope.globalSettings = {};
  
    $scope.confirmCheckNewVersionChange = function() {      
        archetypeGlobalConfigService.setCheckForUpdates($scope.globalSettings.checkForNewVersion);
    }
    
    function getGlobalSettings()
    {
        archetypeGlobalConfigService.globalSettings().then(function(data) {          
            $scope.globalSettings.checkForNewVersion = data.isCheckingForUpdates;
        });
    }    
    
    init();
       
    function init()
    {      
        getGlobalSettings();
    }
});