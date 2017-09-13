module.exports = function(grunt) {
  require('load-grunt-tasks')(grunt);
  var path = require('path');
  
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    pkgMeta: grunt.file.readJSON('config/meta.json'),
    dest: grunt.option('target') || 'dist',
    basePath: path.join('<%= dest %>', 'App_Plugins', '<%= pkgMeta.name %>'),

    'string-replace': {
      version: {
        files: {
          "app/controllers/controller_versioned.js": "app/controllers/controller.js",
          "app/less/archetype_versioned.less": "app/less/archetype.less",
          "app/views/archetype.config.developer.dialog_versioned.html": "app/views/archetype.config.developer.dialog.html",
          "app/views/archetype.config.stylescript.dialog_versioned.html": "app/views/archetype.config.stylescript.dialog.html",
          "app/views/archetype.config.fieldset.dialog_versioned.html": "app/views/archetype.config.fieldset.dialog.html",
          "app/views/archetype.config_versioned.html": "app/views/archetype.config.html",
          "app/views/archetype_versioned.html": "app/views/archetype.html",
          "app/views/archetype.default_versioned.html": "app/views/archetype.default.html"
        },
        options: {
          replacements: [{
            pattern: /{{VERSION}}/g,
            replacement: '/* Version <%= pkgMeta.version %> */'
          },
          {
            pattern: /{{VERSION_HTML}}/g,
            replacement: '<!-- Version <%= pkgMeta.version %> -->'
          }]
        }
      }
    },
    
    watch: {
      options: {
        spawn: false,
        atBegin: true
      },

      less: {
        files: ['app/**/*.less'],
        tasks: ['string-replace', 'less:dist']
      },

      js: {
        files: ['app/**/*.js'],
        tasks: ['string-replace', 'concat:dist']
      },

      html: {
        files: ['app/**/*.html'],
        tasks: ['string-replace', 'copy:html']
      },

      dll: {
        files: ['app/Umbraco/**/*.dll'],
        tasks: ['copy:dll']
      },
      
      assets: {
        files: ['assets/*.*'],
        tasks: ['copy:assets']
      }
    },

    less: {
      dist: {
        options: {
          paths: ["app/less", "lib/less", "vendor"],
        },
        files: {
          '<%= basePath %>/css/archetype.css': 'app/less/archetype_versioned.less',
        }
      }
    },

    concat: {
      options: {
        stripBanners: false
      },
      dist: {
        src: [
          'app/controllers/controller_versioned.js',
          'app/controllers/config.controller.js',
          'app/controllers/config.dialog.controller.js',
          'app/controllers/config.global.controller.js',
          'app/directives/archetypeproperty.js',
          'app/directives/archetypesubmitwatcher.js',
          'app/directives/archetypecustomview.js',
          'app/directives/archetypeLocalize.js',
          'app/directives/archetypeclickoutside.js',
          'app/services/archetypeLocalizationService.js',
          'app/helpers/sampleLabelHelpers.js',
          'app/resources/archetypePropertyEditorResource.js',
          'app/services/archetypeService.js',
          'app/services/archetypeLabelService.js',
          'app/services/archetypeCacheService.js',
          'app/services/archetypeGlobalConfigService.js'
        ],
        dest: '<%= basePath %>/js/archetype.js'
      }
    },

    copy: {
      html: {
        cwd: 'app/views/',
        src: [
            'archetype_versioned.html', 
            'archetype.default_versioned.html', 
            'archetype.config_versioned.html', 
            'archetype.config.fieldset.dialog_versioned.html', 
            'archetype.config.stylescript.dialog_versioned.html', 
            'archetype.config.developer.dialog_versioned.html'
        ],
        dest: '<%= basePath %>/views/',
        expand: true,
        rename: function(dest, src) {
            return dest + src.replace('_versioned','');
          }
      },
      assets: {
        cwd: 'assets/',
        src: ['*.*'],
        dest: '<%= basePath %>/assets',
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
            dest: 'tmp/nuget_binaries/lib/net40',
            expand: true
          },
          {
            cwd: 'app/Umbraco/Archetype.Courier/bin/Release/',
            src: ['Archetype.Courier.dll'],
            dest: 'tmp/nuget_courier/lib/net40',
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
        },
        dist_binaries: {
            src: 'tmp/nuget_binaries/package_binaries.nuspec',
            dest: 'pkg'
        },
        dist_courier: {
            src: 'tmp/nuget_courier/package_courier.nuspec',
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
                    authorUrl: '<%= pkgMeta.authorUrl %>'
                }
            },
            'files': { 
                'tmp/nuget/package.nuspec': ['config/package.nuspec']
            }
        },
        'nuspec_binaries': {
            'options': {
                'data': { 
                    name: '<%= pkgMeta.name %>.Binaries',
                    version: '<%= pkgMeta.version %>',
                    url: '<%= pkgMeta.url %>',
                    license: '<%= pkgMeta.license %>',
                    licenseUrl: '<%= pkgMeta.licenseUrl %>',
                    author: '<%= pkgMeta.author %>',
                    authorUrl: '<%= pkgMeta.authorUrl %>'
                }
            },
            'files': { 
                'tmp/nuget_binaries/package_binaries.nuspec': ['config/package_binaries.nuspec']
            }
        },
        'nuspec_courier': {
            'options': {
                'data': { 
                    name: '<%= pkgMeta.name %>.Courier',
                    version: '<%= pkgMeta.version %>',
                    url: '<%= pkgMeta.url %>',
                    license: '<%= pkgMeta.license %>',
                    licenseUrl: '<%= pkgMeta.licenseUrl %>',
                    author: '<%= pkgMeta.author %>',
                    authorUrl: '<%= pkgMeta.authorUrl %>'
                }
            },
            'files': { 
                'tmp/nuget_courier/package_courier.nuspec': ['config/package_courier.nuspec']
            }
        }
    },

    umbracoPackage: {
      dist: {
        src: 'tmp/umbraco',
        dest: 'pkg',
        options: {
          name: "<%= pkgMeta.name %>",
          version: '<%= pkgMeta.version %>',
          url: '<%= pkgMeta.url %>',
          license: '<%= pkgMeta.license %>',
          licenseUrl: '<%= pkgMeta.licenseUrl %>',
          author: '<%= pkgMeta.author %>',
          authorUrl: '<%= pkgMeta.authorUrl %>',
          readme: '<%= grunt.file.read("config/readme.txt") %>'
        }
      }
    },

    clean: {
      build: '<%= grunt.config("basePath").substring(0, 4) == "dist" ? "dist/**/*" : "null" %>',
      tmp: ['tmp'],
      html: [
        'app/views/*.html',
        '!app/views/archetype.html', 
        '!app/views/archetype.default.html', 
        '!app/views/archetype.config.html', 
        '!app/views/archetype.config.fieldset.dialog.html', 
        '!app/views/archetype.config.stylescript.dialog.html', 
        '!app/views/archetype.config.developer.dialog.html'
        ],
      js: [
        'app/controllers/*.js',
        '!app/controllers/controller.js',
        '!app/controllers/config.controller.js',
        '!app/controllers/config.dialog.controller.js',
        '!app/controllers/config.global.controller.js'
      ],
      less: [
        'app/less/*.less',
        '!app/less/archetype.less'
      ]
    },

    assemblyinfo: {
      options: {
        files: ['app/Umbraco/Umbraco.Archetype/Archetype.Umbraco.csproj', 'app/Umbraco/Archetype.Courier/Archetype.Courier.csproj'],
        filename: 'VersionInfo.cs',
        info: {
          version: '<%= (pkgMeta.version.indexOf("-") > 0 ? pkgMeta.version.substring(0, pkgMeta.version.indexOf("-")) : pkgMeta.version) %>', 
          fileVersion: '<%= pkgMeta.version %>'
        }
      }
    },

    touch: {
      webconfig: {
        src: ['<%= grunt.option("target") %>\\Web.config']
      }
    },

    msbuild: {
      options: {
        stdout: true,
        verbosity: 'quiet',
        maxCpuCount: 4,
        version: 4.0,
        buildParameters: {
          WarningLevel: 2,
          NoWarn: 1607
        }
      },
      dist: {
        src: ['app/Umbraco/Umbraco.Archetype/Archetype.Umbraco.csproj','app/Umbraco/Archetype.Courier/Archetype.Courier.csproj'],
        options: {
          projectConfiguration: 'Release',
          targets: ['Clean', 'Rebuild'],
        }
      }
    }
  });

  grunt.registerTask('default', ['clean', 'string-replace', 'less', 'concat', 'assemblyinfo', 'msbuild:dist', 'copy:dll', 'copy:assets', 'copy:html', 'copy:config', 'clean:html', 'clean:js', 'clean:less']);
  grunt.registerTask('nuget',   ['copy:nuget', 'template:nuspec', 'template:nuspec_binaries', 'template:nuspec_courier', 'nugetpack']);
  grunt.registerTask('umbraco', ['copy:umbraco', 'umbracoPackage']);
  grunt.registerTask('package', ['clean:tmp', 'default', 'nuget', 'copy:umbraco', 'umbracoPackage', 'clean:tmp']);
  grunt.registerTask('assets',  ['clean', 'string-replace', 'less', 'concat', 'assemblyinfo', 'copy:assets', 'copy:html', 'copy:config', 'clean:html', 'clean:js', 'clean:less']);
};