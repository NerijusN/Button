var button = new function() {

    var self = this;

    self.timeout = null;

    self.rumbleProp = function () {

        switch(_.random(0,6)) {
            case 0 :
                return {
                    speed: 0
                };
            case 1:
                return{
                    speed: 50
                };
            case 2:
                return {
                    opacity: true
                };
            case 3:
                return {
                    x: 10,
                    y: 10,
                    rotation: 4
                };
            case 4:
                return {
                    x: 4,
                    y: 0,
                    rotation: 0
                };
            case 5:
                return {
                    x: 0,
                    y: 0,
                    rotation: 5
                }
            case 6:
                return {
                    x: 6,
                    y: 6,
                    rotation: 6,
                    speed: 5,
                    opacity: true,
                    opacityMin: .05
                };
        }

        return {
            x: 6,
            y: 6,
            rotation: 6,
            speed: 5,
            opacity: true,
            opacityMin: .05
        };
    };

    self.init = function () {


        $.connection.hub.start();

        var signalRHub = $.connection.SignalRHub;
        signalRHub.client.hello = function (value) {
            clearTimeout(self.timeout);
            $("#did").text(value);
            $("#did").jrumble(self.rumbleProp());
            $("#did").trigger('startRumble');
            $.ionSound.play('bell_ring');
            self.timeout = setTimeout(function() { $("#did").trigger('stopRumble'); }, 1500);
        };
    };
};

$(function () {
    button.init();
});

$.ionSound({
    sounds: [
        {
            name: "bell_ring"
        },
        {
            name: "door_bell",
        }
    ],
    volume: 0.5,
    path: "/Sounds/",
    preload: false
});