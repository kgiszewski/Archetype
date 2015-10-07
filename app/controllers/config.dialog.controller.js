angular.module('umbraco').controller('ArchetypeConfigOptionsController', function ($scope) {
	$scope.model = $scope.dialogData;

    //handles a fieldset group add
    $scope.addFieldsetGroup = function () {
        $scope.model.fieldsetGroups.push({ name: "" });
    }

    //handles a fieldset group removal
    $scope.removeFieldsetGroup = function ($index) {
        $scope.model.fieldsetGroups.splice($index, 1);
    }
});