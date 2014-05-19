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
        // initialize container for invalid fieldset identifiers (store on ngModelCtrl to separate Archetype validations, e.g. when there two Archetype properties on the same document)
        if(ngModelCtrl.invalidFieldsets == null) {
            ngModelCtrl.invalidFieldsets = [];
        }

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

                loadView(pathToView, mergedConfig, defaultValue, alias, umbracoPropertyAlias, scope, element, ngModelCtrl, validate);
            });
        });

        scope.$on("formSubmitting", function (ev, args) {
            // "hard" validate to highlight any erroneous entries
            validate([scope.fieldset], true);
        });

        scope.$on("formSubmitted", function (ev, args) {
            ngModelCtrl.invalidFieldsets = [];
        });

        function validate(renderModel, markAsInvalid){
            if(renderModel == undefined) {
                return renderModel;
            }
            var valid = true;
            _.each(renderModel, function(fieldset){
                fieldset.isValid = true;
                _.each(fieldset.properties, function(property){
                    property.isValid = true;
                    var propertyValid = true;
                    var propertyConfig = getPropertyByAlias(configFieldsetModel, property.alias);
                    if (propertyConfig) {
                        // use property.value !== property.value to check for NaN values on numeric inputs
                        if(propertyConfig.required && (property.value == null || property.value === "" || property.value !== property.value)) {
                            valid = false;
                            propertyValid = false;
                        }
                        // issue 116: RegEx validate property value
                        // Only validate the property value if anything has been entered - RegEx is considered a supplement to "required".
                        if (valid == true && propertyConfig.regEx && property.value) {
                            var regEx = new RegExp(propertyConfig.regEx);
                            if (regEx.test(property.value) == false) {
                                valid = false;
                                propertyValid = false;
                            }
                        }
                        // only mark the fieldset and property as invalid when doing a "hard" validation
                        if(propertyValid == false && markAsInvalid == true) {
                            fieldset.isValid = false;
                            property.isValid = false;
                        }
                    }
                });
            });

            // handle nested fieldset validation by storing the identifier of all invalid fieldsets 
            var fieldsetIdentifier = scope.umbracoPropertyAlias + "_" + scope.fieldsetIndex;
            var fieldsetIdentifierIndex = ngModelCtrl.invalidFieldsets.indexOf(fieldsetIdentifier);
            if(valid == false) {
                if(fieldsetIdentifierIndex == -1) {
                    ngModelCtrl.invalidFieldsets.push(fieldsetIdentifier);
                }
            }
            else {
                if(fieldsetIdentifierIndex != -1) {
                    ngModelCtrl.invalidFieldsets.splice(fieldsetIdentifierIndex, 1);
                }
            }
            // set invalid state if one or more fieldsets contain invalid entries
            ngModelCtrl.$setValidity('validation', ngModelCtrl.invalidFieldsets.length == 0);
            return renderModel;
        }
    }

    function loadView(view, config, defaultValue, alias, umbracoPropertyAlias, scope, element, ngModelCtrl, validate) {
        if (view)
        {
            $http.get(view).success(function (data) {
                if (data) {
                    if (scope.archetypeConfig.developerMode == '1')
                    {
                        console.log(scope);
                    }

                    //define the initial model and config
                    scope.form = scope.umbracoForm;
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
                        scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties[renderModelPropertyIndex].value = newValue;

                        // call validation method for this fieldset when a property value changes 
                        // use "soft" validation to mimic the default umbraco validation style (show error highlights on submit, not while entering data)
                        validate([scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex]], false);
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
            umbracoPropertyAlias: '=',
            umbracoForm: '='
        }
    }
});