Archetype
=========

## Install Dependencies ##
*Requires Node.js to be installed and in your system path*

    npm install -g grunt-cli && npm install -g grunt
    npm install

## Build ##
    grunt


## Deploy ##
    grunt deploy --target=C:\\path\\to\\umbraco\\site
    grunt watch:dev --target=C:\\path\to\\umbraco\\site

Add `--touch` to either command to automatically touch the web.config on a deploy

## Installation
1) Install the selected <a href='https://github.com/imulus/Archetype/releases'>release</a> through the Umbraco package installer.  Recommended if you want to just see the packaged version.

OR

2) Deploy with Grunt (see above).  Recommended if you wish to extend this repo as this will inject this project into an existing v7 Umbraco install.

## Prevalue Configs ##

Most of the fields are self evident, the following fields will be discussed further:
    
###Developer Mode###

By turning this on, you will get a textarea visible with the data model (not prevalue model).  You can change the contents of the textarea for real-time editing.  Additionally when in developer mode, the console will reflect the scope object of each property.

##Developer Notes##
If you decide to extend this project locally, you'll want to set the `<compilation>` `debug` property in your `web.config` to `true`:

    <compilation defaultLanguage="c#" debug="true" batch="false" targetFramework="4.5">

This is in order to circumvent the minification and caching of the JavaScript files.
