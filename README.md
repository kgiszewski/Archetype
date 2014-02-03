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

## Official Docs ##
http://imulus.github.io/Archetype
