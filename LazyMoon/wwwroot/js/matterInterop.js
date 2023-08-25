window.loadMatterJS = async () => {
    await new Promise((resolve) => {
        const script = document.createElement('script');
        script.onload = resolve;
        script.src = '/js/matter.js'; // Adjust the path
        document.head.appendChild(script);
    });
};

function degrees_to_radians(degrees) {
    var pi = Math.PI;
    return degrees * (pi / 180);
}

var engine;
var render;
window.initializeSimulation = (containerId) => {
    // module aliases
    var Engine = Matter.Engine,
        Render = Matter.Render,
        Runner = Matter.Runner,
        Bodies = Matter.Bodies,
        Body = Matter.Body,
        Composite = Matter.Composite;

    // create an engine
    engine = Engine.create();

    // create a renderer
    render = Render.create({
        element: document.getElementById(containerId),
        engine: engine,
        options: {
            showAngleIndicator: false,
            wireframes: false,
            background: "Transparent"
        }
    });

    
    engine.gravity.x = 0;
    engine.gravity.y = 0.5;


    // create two boxes and a ground
    var ground = Bodies.rectangle(400, 610, 810, 60, { isStatic: true });

    // add all of the bodies to the world
    Composite.add(engine.world, [ground]);

    // run the renderer
    Render.run(render);

    // create runner
    var runner = Runner.create();

    // run the engine
    Runner.run(runner, engine);
};

window.addBox = (src, w, h) => {
    // module aliases
    var Engine = Matter.Engine,
        Render = Matter.Render,
        Runner = Matter.Runner,
        Bodies = Matter.Bodies,
        Body = Matter.Body,
        Composite = Matter.Composite;

    const rand = Math.floor(Math.random() * render.bounds.max.x);

    var boxA = Bodies.rectangle(rand, 0, w, h, {
        render: {
            sprite: {
                texture: src,
            }
        }
    });

    Body.rotate(boxA, degrees_to_radians(Math.floor(Math.random() * 90) - 45));

    Composite.add(engine.world, boxA);
    setTimeout(function () {
        Composite.remove(engine.world, boxA);
    }, 20000);
    for (var i = 1; i <= 199; i++) {
        setTimeout(function () {
            Body.scale(boxA, 0.99, 0.99);
            boxA.render.sprite.xScale *= 0.99;
            boxA.render.sprite.yScale *= 0.99;

        }, i * 100);
    }
}