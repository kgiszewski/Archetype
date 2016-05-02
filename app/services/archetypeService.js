angular.module('umbraco.services').factory('archetypeService', function () {

    // Variables.
    var draggedRteSettings;
    var draggedRteArchetype;
    var rteClass = ".umb-rte textarea";

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
        // This removes the rich text editors in an Archetype (e.g., during a drag operation).
        // Typically, the editors will be restored after the drag completes.
        removeEditors: function (element) {
            draggedRteSettings = {};
            draggedRteArchetype = element;

            // Process each RTE in this Archetype.
            $(rteClass, element).each(function () {

                // Store RTE settings (so they can be restored later).
                var id = $(this).attr("id");
                draggedRteSettings[id] = _.findWhere(tinyMCE.editors, { id: id }).settings;

                // Remove/hide the RTE.
                tinymce.execCommand('mceRemoveEditor', false, id);
                $(this).css("visibility", "hidden");

            });
        },
        // This restores the rich text editors in an Archetype (e.g., after a drop drop operation).
        restoreEditors: function(element) {

            // Variables.
            var bothElements = element.add(draggedRteArchetype);

            // Process each RTE in both Archetypes.
            $(rteClass, bothElements).each(function () {

                // Ensure there are stored settings for the editor (either previously, or the new ones).
                var id = $(this).attr("id");
                draggedRteSettings[id] = draggedRteSettings[id] || _.findWhere(tinyMCE.editors, { id: id }).settings;

                // Remove and reinitialize the editor.
                tinyMCE.execCommand("mceRemoveEditor", false, id);
                tinyMCE.init(draggedRteSettings[id]);

            });

        }
    }
});