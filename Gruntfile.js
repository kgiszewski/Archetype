module.exports = function(grunt) {
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    dest: 'dist',

    watch: {
      less: {
        files: ['app/less/*.less', 'lib/**/*.less'],
        tasks: ['less:build'],
        options: {
          spawn: false,
        }
      },

      js: {
        files: ['app/**/*.js', 'lib/**/*.js'],
        tasks: ['concat', 'jshint'],
        options: {
          spawn: false,
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


    copy: {
      main: {
       files: [
        {expand: true, src: ['app/package.manifest'], dest: '<%= dest %>', flatten: true},
        {expand: true, src: ['app/views/archetype.html'], dest: '<%= dest %>/views', flatten: true} 
        ]
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
          'app/controllers/archetype_controller.js',
          'app/directives/content_item.js'
        ],
        dest: '<%= dest %>/js/archetype.js'
      }
    },

    nugetpack: {
    	dist: {
    		src: 'build/nuget/package.nuspec',
    		dest: 'build'
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
  grunt.loadNpmTasks('grunt-nuget');

  
  grunt.registerTask('package:nuget', ['default', ' nugetpack']);
  grunt.registerTask('css:build', ['less']);
  grunt.registerTask('js:build', ['concat']);
  grunt.registerTask('default', ['clean', 'css:build', 'js:build', 'copy']);
};

