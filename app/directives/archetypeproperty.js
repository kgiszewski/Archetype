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
        var propertyAlias = getUniquePropertyAlias(scope);
        propertyAliasParts = [];
        // initialize container for invalid fieldset property identifiers (store on ngModelCtrl to separate Archetype validations, e.g. when there two Archetype properties on the same document)
        if(ngModelCtrl.invalidProperties == null) {
            ngModelCtrl.invalidProperties = [];
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

                loadView(pathToView, mergedConfig, defaultValue, alias, propertyAlias, scope, element, ngModelCtrl, validateProperty);
            });
        });

        scope.$on("formSubmitting", function (ev, args) {
            // "hard" validate to highlight any erroneous entries
            _.each(scope.fieldset.properties, function (property) {
                validateProperty(scope.fieldset, property, true);
            });
        });

        scope.$on("formSubmitted", function (ev, args) {
            // reset the nested fieldset validation state after submit
            ngModelCtrl.invalidProperties = [];
        });

        // need to pass the property fieldset here to clear any invalid state of the fieldset when validating a single fieldset property
        // - it's the Umbraco way to hide the invalid state when altering an invalid property, even if the new value isn't valid either
        function validateProperty(fieldset, property, markAsInvalid) {
            var valid = true;
            fieldset.isValid = true;
            property.isValid = true;
            var propertyConfig = getPropertyByAlias(configFieldsetModel, property.alias);
            if (propertyConfig) {
                // use property.value !== property.value to check for NaN values on numeric inputs
                if (propertyConfig.required && (property.value == null || property.value === "" || property.value !== property.value)) {
                    valid = false;
                }
                // issue 116: RegEx validate property value
                // Only validate the property value if anything has been entered - RegEx is considered a supplement to "required".
                if (valid == true && propertyConfig.regEx && property.value) {
                    var regEx = new RegExp(propertyConfig.regEx);
                    if (regEx.test(property.value) == false) {
                        valid = false;
                    }
                }
                // only mark the property as invalid when doing a "hard" validation
                if (valid == false && markAsInvalid == true) {
                    property.isValid = false;
                }
            }

            // handle nested fieldset validation by storing the identifier of all invalid fieldset properties 
            var propertyAliasIndex = ngModelCtrl.invalidProperties.indexOf(propertyAlias);
            if (valid == false) {
                if (propertyAliasIndex == -1) {
                    ngModelCtrl.invalidProperties.push(propertyAlias);
                }
            }
            else {
                if (propertyAliasIndex != -1) {
                    ngModelCtrl.invalidProperties.splice(propertyAliasIndex, 1);
                }
            }
            
            if (markAsInvalid) {
                // mark the entire fieldset as invalid if there are any invalid properties in the fieldset, otherwise mark it as valid
                fieldset.isValid =
                    _.find(fieldset.properties, function (property) {
                        return property.isValid == false
                    }) == null;
            }

            // set invalid state if one or more fieldsets contain invalid properties
            ngModelCtrl.$setValidity('validation', ngModelCtrl.invalidProperties.length == 0);
        }
    }

    var propertyAliasParts = [];
    var getUniquePropertyAlias = function (currentScope) {
        if (currentScope.hasOwnProperty('fieldsetIndex') && currentScope.hasOwnProperty('property') && currentScope.hasOwnProperty('propertyConfigIndex'))
        {
            var currentPropertyAlias = "f" + currentScope.fieldsetIndex + "-" + currentScope.property.alias + "-p" + currentScope.propertyConfigIndex;
            propertyAliasParts.push(currentPropertyAlias);
        }
        else if (currentScope.hasOwnProperty('isPreValue')) // Crappy way to identify this is the umbraco property scope
        {
            var umbracoPropertyAlias = currentScope.$parent.$parent.property.alias; // Crappy way to get the umbraco host alias once we identify its scope
            propertyAliasParts.push(umbracoPropertyAlias);
        }

        if (currentScope.$parent)
            getUniquePropertyAlias(currentScope.$parent);

        return _.unique(propertyAliasParts).reverse().join("-");
    };


    function loadView(view, config, defaultValue, alias, propertyAlias, scope, element, ngModelCtrl, validateProperty) {
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
                    scope.model.alias = "archetype-property-" + propertyAlias;

                    //watch for changes since there is no two-way binding with the local model.value
                    scope.$watch('model.value', function (newValue, oldValue) {
                        scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties[renderModelPropertyIndex].value = newValue;

                        // call validation method for the property when the value changes 
                        // use "soft" validation to mimic the default umbraco validation style (show error highlights on submit, not while entering data)
                        validateProperty(scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex], scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties[renderModelPropertyIndex], false);
                    });

                    element.html(data).show();
                    $compile(element.contents())(scope);

                    $timeout(function() {
                        var def = $q.defer();
                        def.resolve(true);
                        $rootScope.$apply();
                    }, 500);
                }
            });
        }
    }

    return {
        require: "^ngModel",
        restrict: "E",
        replace: true,
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