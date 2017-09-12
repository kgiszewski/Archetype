angular.module('umbraco').controller('ArchetypeConfigOptionsController', function ($scope) {
    
    //handles a fieldset group add
    $scope.addFieldsetGroup = function () {
        $scope.dialogData.model.fieldsetGroups.push({ name: "" });
    }

    //handles a fieldset group removal
    $scope.removeFieldsetGroup = function ($index) {
        $scope.dialogData.model.fieldsetGroups.splice($index, 1);
    }

    $scope.apply = function(index) {
        $scope.submit($scope.dialogData);
    }
});