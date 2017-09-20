angular.module("umbraco.directives").directive('archetypeProperty', function ($compile, $http, archetypePropertyEditorResource, umbPropEditorHelper, $timeout, $rootScope, $q, fileManager, editorState, archetypeService) {

    var linker = function (scope, element, attrs, ngModelCtrl) {
        var configFieldsetModel = archetypeService.getFieldsetByAlias(scope.archetypeConfig.fieldsets, scope.fieldset.alias);
        var view = "";
        var label = configFieldsetModel.properties[scope.propertyConfigIndex].label;
        var dataTypeGuid = configFieldsetModel.properties[scope.propertyConfigIndex].dataTypeGuid;
        var config = null;
        var alias = configFieldsetModel.properties[scope.propertyConfigIndex].alias;
        var defaultValue = configFieldsetModel.properties[scope.propertyConfigIndex].value;
        var propertyAliasParts = [];
        var propertyAlias = archetypeService.getUniquePropertyAlias(scope, propertyAliasParts);
        
        //try to convert the defaultValue to a JS object
        defaultValue = archetypeService.jsonOrString(defaultValue, scope.archetypeConfig.developerMode, "defaultValue");

        //grab info for the selected datatype, prepare for view
        archetypePropertyEditorResource.getDataType(dataTypeGuid, scope.archetypeConfig.enableDeepDatatypeRequests, editorState.current.contentTypeAlias, scope.propertyEditorAlias, alias, editorState.current.id).then(function (data) {
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

                loadView(pathToView, mergedConfig, defaultValue, alias, propertyAlias, scope, element, ngModelCtrl, configFieldsetModel);
            });
        });
    }

    function loadView(view, config, defaultValue, alias, propertyAlias, scope, element, ngModelCtrl, configFieldsetModel) {
        if (view)
        {
            $http.get(view, { cache: true }).success(function (data) {
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
                    scope.renderModelPropertyIndex = archetypeService.getPropertyIndexByAlias(archetypeService.getFieldset(scope).properties, alias);

                    if (!scope.renderModelPropertyIndex)
                    {
                        var propertyValue = { alias: alias, value: defaultValue };
                        archetypeService.getFieldset(scope).properties.push(propertyValue);
                        scope.renderModelPropertyIndex = archetypeService.getPropertyIndexByAlias(archetypeService.getFieldset(scope).properties, alias);
                    }

                    scope.renderModel = {};
                    scope.model.value = archetypeService.getFieldsetProperty(scope).value;

                    // init the property editor state (while ensuring the temporary ID is retained
                    // from any prior initializations).
                    var fieldsetProperty = archetypeService.getFieldsetProperty(scope);
                    var oldState = fieldsetProperty.editorState;
                    archetypeService.ensureTemporaryId(fieldsetProperty);
                    if (oldState) {
                        fieldsetProperty.editorState.temporaryId = oldState.temporaryId;
                    }

                    //set the config from the prevalues
                    scope.model.config = config;

                    /*
                        Property Specific Hacks

                        Preference is to not do these, but unless we figure out core issues, this is quickest fix.
                    */

                    //MNTP *single* hack
                    if(scope.model.config.maxNumber && scope.model.config.multiPicker)
                    {
                        if(scope.model.config.maxNumber == "1")
                        {
                            scope.model.config.multiPicker = "0";
                        }
                    }

                    //hacks for various built-in datatyps including upload, colorpicker and tags
                    if (!scope.propertyForm) {
                        scope.propertyForm = scope.form;
                    }
                    if (!scope.model.validation) {
                        scope.model.validation = {};
                        scope.model.validation.mandatory = 0;
                    }

                    //some items need an alias
                    scope.model.alias = "archetype-property-" + propertyAlias;
                    //some items also need an id (file upload for example)
                    scope.model.id = propertyAlias;

                    //watch for changes since there is no two-way binding with the local model.value
                    scope.$watch('model.value', function (newValue, oldValue) {
                        
                        archetypeService.getFieldsetProperty(scope).value = newValue;

                        // notify the linker that the property value changed
                        archetypeService.propertyValueChanged(archetypeService.getFieldset(scope), archetypeService.getFieldsetProperty(scope));
                    });

                    scope.$on('archetypeFormSubmitting', function (ev, args) {
                        // #385 - revert the changes made in #311 to avoid publishing invalid fieldsets (leaving the code from #311 here, in case we figure out how to re-introduce "save as draft")
                        // if(args.action !== 'save') {
                        // validate all fieldset properties
                        _.each(scope.fieldset.properties, function (property) {
                            archetypeService.validateProperty(scope.fieldset, property, configFieldsetModel);
                        });

                        var validationKey = "validation-f" + scope.fieldsetIndex;

                        ngModelCtrl.$setValidity(validationKey, scope.fieldset.isValid);
                        // }

                        // did the value change (if it did, it most likely did so during the "formSubmitting" event)
                        var property = archetypeService.getFieldsetProperty(scope);

                        var currentValue = property.value;

                        if (currentValue != scope.model.value) {
                            archetypeService.getFieldsetProperty(scope).value = scope.model.value;

                            // notify the linker that the property value changed
                            archetypeService.propertyValueChanged(archetypeService.getFieldset(scope), archetypeService.getFieldsetProperty(scope));
                        }
                    });

                    // issue 114: handle file selection on property editors
                    scope.$on("filesSelected", function (event, args) {
                        // populate the fileNames collection on the property editor state
                        var property = archetypeService.getFieldsetProperty(scope);

                        property.editorState.fileNames = [];

                        _.each(args.files, function (item) {
                            property.editorState.fileNames.push(item.name);
                        });

                        // remove the files set for this property
                        // NOTE: we can't use property.alias because the file manager registers the selected files on the assigned Archetype property alias (e.g. "archetype-property-archetype-property-archetype-property-content-0-2-0-1-0-0")
                        fileManager.setFiles(scope.model.alias, []);

                        // now tell the containing Archetype to pick up the selected files
                        scope.archetypeRenderModel.setFiles(args.files);
                    });

                    scope.$on("archetypeRemoveFieldset", function (ev, args) {
                        var validationKey = "validation-f" + args.index;
                        ngModelCtrl.$setValidity(validationKey, true);
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
            propertyEditorAlias: '=',
            archetypeConfig: '=',
            fieldset: '=',
            fieldsetIndex: '=',
            archetypeRenderModel: '=',
            umbracoPropertyAlias: '=',
            umbracoForm: '='
        }
    }
});