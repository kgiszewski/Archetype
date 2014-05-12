angular.module("umbraco.directives").directive('archetypeProperty', function ($compile, $http, archetypePropertyEditorResource, umbPropEditorHelper, $timeout, $rootScope, $q) {
    
    function getFieldsetByAlias(fieldsets, alias)
    {
        return _.find(fieldsets, function(fieldset){
            return fieldset.alias == alias;
        });
    }

    function getPropertyIndexByAlias(properties, alias){
        for (var i in properties)
        {
            if (properties[i].alias == alias) {
                return i;
            }
        }
    }

    function getPropertyByAlias(fieldset, alias){
        return _.find(fieldset.properties, function(property){
            return property.alias == alias; 
        });
    }

    //helper that returns a JS ojbect from 'value' string or the original string
    function jsonOrString(value, developerMode, debugLabel){
        if(value && typeof value == 'string'){
            try{
                if(developerMode == '1'){
                    console.log("Trying to parse " + debugLabel + ": " + value); 
                }
                value = JSON.parse(value);
            }
            catch(exception)
            {
                if(developerMode == '1'){
                    console.log("Failed to parse " + debugLabel + "."); 
                }
            }
        }

        if(value && developerMode == '1'){
            console.log(debugLabel + " post-parsing: ");
            console.log(value); 
        }

        return value;
    }

    var linker = function (scope, element, attrs, ngModelCtrl) {
        var configFieldsetModel = getFieldsetByAlias(scope.archetypeConfig.fieldsets, scope.fieldset.alias);
        var view = "";
        var label = configFieldsetModel.properties[scope.propertyConfigIndex].label;
        var dataTypeGuid = configFieldsetModel.properties[scope.propertyConfigIndex].dataTypeGuid;
        var config = null;
        var alias = configFieldsetModel.properties[scope.propertyConfigIndex].alias;
        var defaultValue = configFieldsetModel.properties[scope.propertyConfigIndex].value;
        var umbracoPropertyAlias = scope.umbracoPropertyAlias;

        //try to convert the defaultValue to a JS object
        defaultValue = jsonOrString(defaultValue, scope.archetypeConfig.developerMode, "defaultValue");

        //grab info for the selected datatype, prepare for view
        archetypePropertyEditorResource.getDataType(dataTypeGuid).then(function (data) {
            //transform preValues array into object expected by propertyeditor views
            var configObj = {};
            _.each(data.preValues, function(p) {
                configObj[p.key] = p.value;
            });
            config = configObj;

            //determine the view to use [...] and load it
            archetypePropertyEditorResource.getPropertyEditorMapping(data.selectedEditor).then(function(propertyEditor) {
                var pathToView = umbPropEditorHelper.getViewPath(propertyEditor.view);

                //load in the DefaultPreValues for the PropertyEditor, if any
                var defaultConfigObj =  {};
                if (propertyEditor.hasOwnProperty('defaultPreValues') && propertyEditor.defaultPreValues != null) {
                    _.extend(defaultConfigObj, propertyEditor.defaultPreValues);
                }

                var mergedConfig = _.extend(defaultConfigObj, config);

                loadView(pathToView, mergedConfig, defaultValue, alias, umbracoPropertyAlias, scope, element, ngModelCtrl);
            });
        });

        ngModelCtrl.$parsers.push(validate);
        ngModelCtrl.$formatters.push(validate);

        function validate(renderModel){
            var valid = true;

            _.each(renderModel, function(fieldset){
                fieldset.isValid = true;
                _.each(fieldset.properties, function(property){
                    property.isValid = true;

                    var propertyConfig = getPropertyByAlias(configFieldsetModel, property.alias);
                    if (propertyConfig) {
                        if(propertyConfig.required && (property.value == null || property.value === "")) {
                            fieldset.isValid = false;
                            property.isValid = false;
                            valid = false;
                        }
                        // issue 116: RegEx validate property value
                        // Only validate the property value if anything has been entered - RegEx is considered a supplement to "required".
                        if (valid == true && propertyConfig.regEx && property.value) {
                            var regEx = new RegExp(propertyConfig.regEx);
                            if (regEx.test(property.value) == false) {
                                fieldset.isValid = false;
                                property.isValid = false;
                                valid = false;
                            }
                        }
                    }
                });
            });

            ngModelCtrl.$setValidity('validation', valid);
            return renderModel;
        }
    }

    function loadView(view, config, defaultValue, alias, umbracoPropertyAlias, scope, element, ngModelCtrl) {
        if (view)
        {
            $http.get(view).success(function (data) {
                if (data) {
                    if (scope.archetypeConfig.developerMode == '1')
                    {
                        console.log(scope);
                    }

                    //define the initial model and config
                    scope.model = {};
                    scope.model.config = {};

                    //ini the property value after test to make sure a prop exists in the renderModel
                    var renderModelPropertyIndex = getPropertyIndexByAlias(scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties, alias);

                    if (!renderModelPropertyIndex)
                    {
                        scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties.push(JSON.parse('{"alias": "' + alias + '", "value": "' + defaultValue + '"}'));
                        renderModelPropertyIndex = getPropertyIndexByAlias(scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties, alias);
                    }
                    scope.model.value = scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties[renderModelPropertyIndex].value;

                    //set the config from the prevalues
                    scope.model.config = config;

                    //some items need an alias
                    scope.model.alias = "archetype-property-" + umbracoPropertyAlias + "-" + scope.fieldsetIndex + "-" + scope.propertyConfigIndex;

                    //watch for changes since there is no two-way binding with the local model.value
                    scope.$watch('model.value', function (newValue, oldValue) {
                        if (newValue == oldValue) {
                            //issue #121: 
                            //don't update if there's nothing to update - if we do, the form will be considered dirty (even if the values are the same) and the user will be prompted to save/discard changes
                            return;
                        }
                        scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties[renderModelPropertyIndex].value = newValue;

                        //trigger the validation pipeline
                        ngModelCtrl.$setViewValue(ngModelCtrl.$viewValue);
                    });

                    element.html(data).show();
                    $compile(element.contents())(scope);

                    $timeout(function() {
                        var def = $q.defer();
                        def.resolve(true);
                        $rootScope.$apply();
                    }, 500)
                }
            });
        }
    }

    return {
        require: "^ngModel",
        restrict: "E",
        rep1ace: true,
        link: linker,
        scope: {
            property: '=',
            propertyConfigIndex: '=',
            archetypeConfig: '=',
            fieldset: '=',
            fieldsetIndex: '=',
            archetypeRenderModel: '=',
            umbracoPropertyAlias: '='
        }
    }
});