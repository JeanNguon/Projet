/// <reference path="../../app/App.ts" />
/*jslint white: true, node:true, nomen:true */
/*global beforeEach, afterEach, describe, expect, it, spyOn, xdescribe, xit, inject */
/*global openDatabase */
'use strict';
describe("service: Logger", function () {
    beforeEach(module('app'));
    var logger;
    beforeEach(inject(function (_logger_) {
        logger = _logger_;
    }));
    /*
    beforeEach(function() {

    });
    */
    it('should be registered', function () {
        expect(logger).toBeTruthy();
    });
    it('should have method info()', function () {
        expect(logger.info).toBeTruthy();
    });
    it('should have method warn()', function () {
        expect(logger.warn).toBeTruthy();
    });
    it('should have method error()', function () {
        expect(logger.error).toBeTruthy();
    });
    /*
    it('should have support for WebSQL', function() {
        //expect(App.VideosCtrl).not.to.equal(null);
          var db = openDatabase('testDB', '1.0', 'testDB', 2 * 1024);
          expect(db).toBeDefined();
          expect(db).toBeTruthy();
          //expect(db).toBeNull();
          //expect(true).toBe(true);//f
    }); */
});


/// <reference path="../../app/App.ts" />
/*jslint white: true, node:true, nomen:true */
/*global beforeEach, afterEach, describe, expect, it, spyOn, xdescribe, xit, inject */
/*global openDatabase */
'use strict';
//how to test controllers: http://angular-tips.com/blog/2014/06/introduction-to-unit-test-controllers/
describe("controller: Shell", function () {
    //var scope, controller;
    var controller;
    beforeEach(module('app'));
    beforeEach(inject(function ($controller, $rootScope) {
        //scope = $rootScope.$new();
        controller = $controller('shell', {});
    }));
    it('should be registered', function () {
        expect(controller).toBeTruthy();
    });
    it('should have a property: message', function () {
        //dump(controller.message);
        expect(controller.message).toBeDefined();
    });
    it('should have a property: message with a defined content', function () {
        expect(controller.message).toBe('a message comes here now!!');
    });
    it('should have a function: increase()', function () {
        //dump(controller.increase);
        expect(controller.increase).toBeDefined();
    });
    it('should increase count by 1 when calling: increase()', function () {
        expect(controller.count).toBe(3);
        controller.increase();
        expect(controller.count).toBe(4);
    });
});


