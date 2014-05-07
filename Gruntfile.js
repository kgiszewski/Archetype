module.exports = function(grunt) {
  require('load-grunt-tasks')(grunt);
  var path = require('path')

  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    pkgMeta: grunt.file.readJSON('config/meta.json'),
    dest: grunt.option('target') || 'dist',
    basePath: path.join('<%= dest %>', 'App_Plugins', '<%= pkgMeta.name %>'),

    watch: {
      options: {
        spawn: false,
        atBegin: true
      },

      less: {
        files: ['app/**/*.less'],
        tasks: ['less:dist']
      },

      js: {
        files: ['app/**/*.js'],
        tasks: ['concat:dist']
      },

      html: {
        files: ['app/**/*.html'],
        tasks: ['copy:html']
      },

      dll: {
        files: ['app/Umbraco/**/*.dll'],
        tasks: ['copy:dll']
      }
    },

    less: {
      dist: {
        options: {
          paths: ["app/less", "lib/less", "vendor"],
        },
        files: {
          '<%= basePath %>/css/archetype.css': 'app/less/archetype.less',
        }
      }
    },

    concat: {
      options: {
        stripBanners: false
      },
      dist: {
        src: [
          'app/controllers/controller.js',
          'app/controllers/config.controller.js',
          'app/directives/archetypeproperty.js',
          'app/directives/localize.js',
          'app/services/localization.js',
          'app/resources/propertyeditor.js'
        ],
        dest: '<%= basePath %>/js/archetype.js'
      }
    },

    copy: {
      html: {
        cwd: 'app/views/',
        src: ['archetype.html', 'archetype.config.html'],
        dest: '<%= basePath %>/views',
        expand: true
      },
      dll: {
        cwd: 'app/Umbraco/Umbraco.Archetype/bin/Release/',
        src: 'Archetype.dll',
        dest: '<%= dest %>/bin/',
        expand: true
      },
      config: {
        files: [
          {
            cwd: 'app/langs/',
            src: ['**'],
            dest: '<%= basePath %>/langs',
            expand: true
          }
        ]
      },
      nuget: {
        files: [
          {
            cwd: '<%= dest %>',
            src: ['**/*', '!bin', '!bin/*'],
            dest: 'tmp/nuget/content',
            expand: true
          },
          {
            cwd: '<%= dest %>/bin',
            src: ['*.dll'],
            dest: 'tmp/nuget/lib/net40',
            expand: true
          }
        ]
      },
      umbraco: {
        cwd: '<%= dest %>',
        src: '**/*',
        dest: 'tmp/umbraco',
        expand: true
      }
    },

    nugetpack: {
    	dist: {
    		src: 'tmp/nuget/package.nuspec',
    		dest: 'pkg'
    	}
    },

    template: {
    	'nuspec': {
			'options': {
    			'data': { 
            name: '<%= pkgMeta.name %>',
    				version: '<%= pkgMeta.version %>',
            url: '<%= pkgMeta.url %>',
            license: '<%= pkgMeta.license %>',
            licenseUrl: '<%= pkgMeta.licenseUrl %>',
            author: '<%= pkgMeta.author %>',
            authorUrl: '<%= pkgMeta.authorUrl %>',

    				files: [{ path: 'tmp/nuget/**', target: 'content/App_Plugins/Archetype'}]
	    		}
    		},
    		'files': { 
    			'tmp/nuget/package.nuspec': ['config/package.nuspec']
    		}
    	}
    },

    umbracoPackage: {
      options: {
        name: "<%= pkgMeta.name %>",
        version: '<%= pkgMeta.version %>',
        url: '<%= pkgMeta.url %>',
        license: '<%= pkgMeta.license %>',
        licenseUrl: '<%= pkgMeta.licenseUrl %>',
        author: '<%= pkgMeta.author %>',
        authorUrl: '<%= pkgMeta.authorUrl %>',
        manifest: 'config/package.xml',
        readme: 'config/readme.txt',
        sourceDir: 'tmp/umbraco',
        outputDir: 'pkg',
      }
    },

    clean: {
      build: '<%= grunt.config("basePath").substring(0, 4) == "dist" ? "dist/**/*" : "null" %>',
      tmp: ['tmp']
    },

    assemblyinfo: {
      options: {
        files: ['app/Umbraco/Umbraco.Archetype/Archetype.Umbraco.csproj'],
        filename: 'VersionInfo.cs',
        info: {
          version: '<%= (pkgMeta.version.indexOf("-") ? pkgMeta.version.substring(0, pkgMeta.version.indexOf("-")) : pkgMeta.version) %>', 
          fileVersion: '<%= pkgMeta.version %>'
        }
      }
    },

    touch: {
      webconfig: {
        src: ['<%= grunt.option("target") %>\\Web.config']
      }
    },

    jshint: {
      options: {
        jshintrc: '.jshintrc'
      },
      src: {
        src: ['app/**/*.js', 'lib/**/*.js']
      }
    },

    msbuild: {
      options: {
        stdout: true,
        verbosity: 'quiet',
        maxCpuCount: 4,
        buildParameters: {
          WarningLevel: 2,
          NoWarn: 1607
        }
      },
      dist: {
        src: ['app/Umbraco/Umbraco.Archetype/Archetype.Umbraco.csproj'],
        options: {
          projectConfiguration: 'Release',
          targets: ['Clean', 'Rebuild'],
        }
      }
    }

  });

  grunt.registerTask('default', ['clean', 'less', 'concat', 'assemblyinfo', 'msbuild:dist', 'copy:dll', 'copy:config', 'copy:html']);

  grunt.registerTask('nuget',   ['clean:tmp', 'default', 'copy:nuget', 'template:nuspec', 'nugetpack', 'clean:tmp']);
  grunt.registerTask('umbraco', ['clean:tmp', 'default', 'copy:umbraco', 'umbracoPackage', 'clean:tmp']);
  grunt.registerTask('package', ['clean:tmp', 'default', 'copy:nuget', 'template:nuspec', 'nugetpack', 'copy:umbraco', 'umbracoPackage', 'clean:tmp']);
};