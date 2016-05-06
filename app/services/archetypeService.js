angular.module('umbraco.services').factory('archetypeService', function () {

    // Variables.
    var draggedRteArchetype;
    var rteClass = ".umb-rte textarea";
    var editorSettings = {};

    //public
    return {
        //helper that returns a JS ojbect from 'value' string or the original string
        jsonOrString: function (value, developerMode, debugLabel){
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
        },
        getFieldsetByAlias: function (fieldsets, alias)
        {
            return _.find(fieldsets, function(fieldset){
                return fieldset.alias == alias;
            });
        },
        getPropertyIndexByAlias: function(properties, alias){
            for (var i in properties)
            {
                if (properties[i].alias == alias) {
                    return i;
                }
            }
        },
        getPropertyByAlias: function (fieldset, alias){
            return _.find(fieldset.properties, function(property){
                return property.alias == alias; 
            });
        },
        getUniquePropertyAlias: function (currentScope, propertyAliasParts) {
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
                this.getUniquePropertyAlias(currentScope.$parent, propertyAliasParts);

            return _.unique(propertyAliasParts).reverse().join("-");
        },
        getFieldset: function(scope) {
            return scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex];
        },
        getFieldsetProperty: function (scope) {
            return this.getFieldset(scope).properties[scope.renderModelPropertyIndex];
        },
        setFieldsetValidity: function (fieldset) {
            // mark the entire fieldset as invalid if there are any invalid properties in the fieldset, otherwise mark it as valid
            fieldset.isValid =
                _.find(fieldset.properties, function (property) {
                    return property.isValid == false
                }) == null;
        },
        validateProperty: function (fieldset, property, configFieldsetModel) {
            var propertyConfig = this.getPropertyByAlias(configFieldsetModel, property.alias);

            if (propertyConfig) {
                // use property.value !== property.value to check for NaN values on numeric inputs
                if (propertyConfig.required && (property.value == null || property.value === "" || property.value !== property.value)) {
                    property.isValid = false;
                }
                // issue 116: RegEx validate property value
                // Only validate the property value if anything has been entered - RegEx is considered a supplement to "required".
                if (property.isValid == true && propertyConfig.regEx && property.value) {
                    var regEx = new RegExp(propertyConfig.regEx);
                    if (regEx.test(property.value) == false) {
                        property.isValid = false;
                    }
                }
            }

            this.setFieldsetValidity(fieldset);
        },
        // called when the value of any property in a fieldset changes
        propertyValueChanged: function (fieldset, property) {
            // it's the Umbraco way to hide the invalid state when altering an invalid property, even if the new value isn't valid either
            property.isValid = true;
            this.setFieldsetValidity(fieldset);
        },
        // This stores the rich text editors in all Archetypes (e.g., during a drag operation).
        // Typically, the editors will be restored after the drag completes.
        storeEditors: function (element) {

            // Variables.
            var self = this;
            draggedRteArchetype = element;

            // Empty the stored settings.
            editorSettings = {};

            // For fast lookups, store each editor by the element ID.
            var editorsById = {};
            for (var i = 0; i < tinyMCE.editors.length; i++) {
                var tempEditor = tinyMCE.editors[i];
                editorsById[tempEditor.id] = tempEditor;
            }

            // Process each rich text editor.
            $(rteClass).each(function() {

                // Variables.
                var id = $(this).attr("id");
                var editor = editorsById[id];

                // Get the property's temporary ID.
                var scope = angular.element(this).scope().$parent;
                var property = self.getFieldsetProperty(scope);
                var tempId = property.editorState.temporaryId;

                // Store the editor settings by the temporary ID?
                if (editor && editor.settings) {
                    editorSettings[tempId] = editor.settings;
                }

            });

        },
        // This restores the rich text editors in the specified Archetype
        // (e.g., after a drop drop operation).
        restoreEditors: function(element) {

            // Variables.
            var bothElements = element
                ? element.add(draggedRteArchetype)
                : draggedRteArchetype;
            var self = this;

            // Process each RTE in both Archetypes.
            $(rteClass, bothElements).each(function () {

                // Variables.
                var id = $(this).attr("id");

                // Get the stored editor settings.
                var scope = angular.element(this).scope().$parent;
                var property = self.getFieldsetProperty(scope);
                var tempId = property.editorState.temporaryId;
                var settings = editorSettings[tempId];

                // Remove and reinitialize the editor.
                tinyMCE.execCommand("mceRemoveEditor", false, id);
                tinyMCE.init(settings);

            });

        },
        // Ensures the specified property has a temporary ID in its editor state,
        // optionally forcing one to be regenerated (if specified).
        ensureTemporaryId: function(property, regenerateId) {
            if (!property.editorState) {
                property.editorState = {};
            }
            var editorState = property.editorState;
            if (!editorState.temporaryId || regenerateId) {
                editorState.temporaryId = _.uniqueId("property-temp-id-");
            }
        }
    }
});