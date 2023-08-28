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

window.writeLog = (logText) => {
    console.log(logText);
}

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

window.initializeCanvas = () => {


    const canvas = document.createElement('canvas');
    canvas.width = 1000;
    canvas.height = 100;    

    const ctx = canvas.getContext('2d');

    window.toBase64Image = async (text) => {
        ctx.clearRect(0,0,1000,100);
        const fontPath = 'font/NotoSansKR-Regular-Hestia.otf'; // Update this path
        const randomColor = `rgb(${getRandomNumber(256)}, ${getRandomNumber(256)}, ${getRandomNumber(256)})`;

        ctx.fillStyle = 'rgba(5, 5, 5, 0)';
        ctx.fillRect(0, 0, canvas.width, canvas.height);

        ctx.font = "25px NotoSans";
        ctx.strokeStyle = 'rgb(0, 0, 0)';
        ctx.fillStyle = randomColor;
        ctx.textAlign = 'left';
        var x = 0;
        var y = 30;
        var lineheight = 30;
        var lines = text.split('\n');
        for (var i = 0; i < lines.length; i++)
            ctx.fillText(lines[i], x, y + (i * lineheight));

        const rect = findNonBackgroundRectangle(ctx, { r: 5, g: 5, b: 5, a: 0 }); // Update background color
        const cropImage = cropCanvas(ctx, rect.x, rect.y, rect.width, rect.height);

        const result = await convertCanvasToBase64(cropImage);
        return JSON.stringify({ base64: result, width: rect.width, height: rect.height });
    }

    function findNonBackgroundRectangle(ctx, backgroundColor) {
        const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
        const data = imageData.data;
        let minX = canvas.width;
        let minY = canvas.height;
        let maxX = -1;
        let maxY = -1;

        for (let y = 0; y < canvas.height; y++) {
            for (let x = 0; x < canvas.width; x++) {
                const index = (y * canvas.width + x) * 4;
                const pixel = {
                    r: data[index],
                    g: data[index + 1],
                    b: data[index + 2],
                    a: data[index + 3],
                };
                if (pixel.a != 0) {
                    minX = Math.min(minX, x);
                    minY = Math.min(minY, y);
                    maxX = Math.max(maxX, x);
                    maxY = Math.max(maxY, y);
                }
            }
        }

        const width = maxX - minX + 1;
        const height = maxY - minY + 1;
        return { x: minX, y: minY, width, height };
    }

    function cropCanvas(ctx, x, y, width, height) {
        const croppedCanvas = document.createElement('canvas');
        croppedCanvas.width = width;
        croppedCanvas.height = height;
        const croppedCtx = croppedCanvas.getContext('2d');
        const imageData = ctx.getImageData(x, y, width, height);
        croppedCtx.putImageData(imageData, 0, 0);
        return croppedCanvas;
    }

    async function convertCanvasToBase64(canvas) {
        return canvas.toDataURL('image/png');
    }

    function getRandomNumber(max) {
        return Math.floor(Math.random() * max);
    }

    function compareColors(color1, color2) {
        return color1.r === color2.r && color1.g === color2.g && color1.b === color2.b && color1.a === color2.a;
    }

}