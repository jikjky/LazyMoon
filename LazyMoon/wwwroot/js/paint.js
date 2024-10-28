var namespace = 'LazyMoon';

var lineWidth = 10;
var strokeStyle = 'red';

var PushArray = new Array();
var Step = -1;

var canvas;
var ctx;

window.addShortcutListener = function () {
    document.addEventListener('keydown', handleShortcut);
};
function handleShortcut(event) {
    console.log(event);
    if (event.ctrlKey && event.shiftKey && event.code === 'KeyZ') {
        Redo();
    }
    else if (event.ctrlKey && event.code === 'KeyZ') {
        Undo();
    }
}

function Push() {
    Step++;
    if (Step < PushArray.length) {
        PushArray.length = Step;
    }
    if (canvas) {
        PushArray.push(canvas.toDataURL());
    }
}

async function Undo() {

    if (Step > 0) {
        Step--;

        const canvasPic = new Image();
        await new Promise(r => {
            canvasPic.onload = r;
            canvasPic.src = PushArray[Step];
        });
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.beginPath();
        ctx.drawImage(canvasPic, 0, 0);
    }
}

async function Redo() {
    if (Step < PushArray.length - 1) {
        Step++;


        const canvasPic = new Image();
        await new Promise(r => {
            canvasPic.onload = r;
            canvasPic.src = PushArray[Step];
        });
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.beginPath();
        ctx.drawImage(canvasPic, 0, 0);
    }
}

async function Clear() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.beginPath();
    Push();
}
Push();
function setDrawWidth(width) {
    lineWidth = width;
    changeValue();
}

function setDrawStroke(stroke) {
    strokeStyle = stroke;
    changeValue();
}

function changeValue() {
    DotNet.invokeMethodAsync(namespace, 'ValueChanged', lineWidth, strokeStyle);
}

function setCanvas() {
    canvas = document.querySelector('#canvas');
    ctx = canvas.getContext('2d');

    //Resizeing
    canvas.height = 500;
    canvas.width = 500;

    let painting = false;

    Push();

    function startPosition(e) {
        painting = true;
        draw(e);
    }

    function finishedPosition(e) {
        if (painting === true) { 
            painting = false;
            ctx.beginPath();
            Push();
        }
    }

    function draw(e) {
        if (!painting) return;
        ctx.lineWidth = lineWidth;
        ctx.lineCap = 'round';
        ctx.strokeStyle = strokeStyle;
        ctx.lineTo(e.offsetX, e.offsetY);
        ctx.stroke();
        ctx.beginPath();
        ctx.moveTo(e.offsetX, e.offsetY);
    }

    canvas.addEventListener('mousedown', startPosition);
    canvas.addEventListener('mouseup', finishedPosition);
    canvas.addEventListener('mouseleave', finishedPosition);
    canvas.addEventListener('mousemove', draw);

    changeValue();
}