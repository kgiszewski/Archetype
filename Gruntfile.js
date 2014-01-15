module.exports = function(grunt) {
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    dest: 'dist',

    watch: {
      less: {
        files: ['app/less/*.less', 'lib/**/*.less'],
        tasks: ['less:build'],
        options: {
          spawn: false
        }
      },

      js: {
        files: ['app/**/*.js', 'lib/**/*.js'],
        tasks: ['concat', 'jshint'],
        options: {
          spawn: false
        }
      },

      dev: {
        files: ['app/**'],
        tasks: ['deploy'],
        options: {
          spawn: false
        }
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

    less: {
      build: {
        options: {
          paths: ["app/less", "lib/less", "vendor"],
        },
        files: {
          '<%= dest %>/css/archetype.css': 'app/less/archetype.less',
        }
      }
    },

    concat: {
      options: {
        stripBanners: false
      },
      application: {
        src: [
          'app/controllers/controller.js',
          'app/controllers/config.controller.js',
          'app/directives/archetypeproperty.js',
          'app/services/propertyeditor.js'
        ],
        dest: '<%= dest %>/js/archetype.js'
      }
    },

    copy: {
      build: {
       files: [
        {expand: true, cwd: 'app/', src: ['package.manifest'], dest: '<%= dest %>', flatten: true},
        {expand: true, cwd: 'app/data', src: ['views.js'], dest: '<%= dest %>/js', flatten: true},
        {expand: true, cwd: 'app/views/', src: ['archetype.html', 'archetype.config.html'], dest: '<%= dest %>/views', flatten: true} 
        ]
      },
      deploy: {
        files: [
          {expand: true, cwd: '<%= dest %>/', src: ['**'], dest: '<%= grunt.option("target") %>\\App_Plugins\\Imulus.Archetype', flatten: false},
        ]
      }

    },

    touch: {
      options: {},
      webconfig: {
        src: ['<%= grunt.option("target") %>\\Web.config']
      }
    },

    clean: ['<%= dest %>']

  });

  grunt.loadNpmTasks('grunt-contrib-watch');
  grunt.loadNpmTasks('grunt-contrib-less');
  grunt.loadNpmTasks('grunt-contrib-clean');
  grunt.loadNpmTasks('grunt-contrib-concat');
  grunt.loadNpmTasks('grunt-contrib-copy');
  grunt.loadNpmTasks('grunt-contrib-uglify');
  grunt.loadNpmTasks('grunt-contrib-jshint');
  grunt.loadNpmTasks('grunt-touch');

  grunt.registerTask('touchwebconfigifenabled', function() { if (grunt.option("touch")) grunt.task.run("touch:webconfig") });
  grunt.registerTask('deploy', ['default', 'copy:deploy', 'touchwebconfigifenabled']);
  grunt.registerTask('css:build', ['less']);
  grunt.registerTask('js:build', ['concat']);
  grunt.registerTask('default', ['clean', 'css:build', 'js:build', 'copy:build']);
};

