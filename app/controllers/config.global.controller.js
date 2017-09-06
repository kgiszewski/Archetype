angular.module('umbraco').controller('ArchetypeConfigGlobalOptionsController', function ($scope, archetypeGlobalConfigService) {
    $scope.globalSettings = {};
  
    $scope.confirmCheckNewVersionChange = function() {      
        if(confirm("By changing this value, it will cause a restart of the app domain, are you sure?"))
        {
            archetypeGlobalConfigService.setCheckForUpdates($scope.globalSettings.checkForNewVersion);
        }
        else
        {
            $scope.globalSettings.checkForNewVersion = !$scope.globalSettings.checkForNewVersion;
        }
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