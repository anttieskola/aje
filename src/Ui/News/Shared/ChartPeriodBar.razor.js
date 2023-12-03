function getRandomColor() {
    var letters = '0123456789ABCDEF';
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

export function drawBox(canvasId, startPosition, width) {
    var canvas = document.getElementById(canvasId);
    const ctx = canvas.getContext('2d');
    ctx.fillStyle = getRandomColor();
    ctx.fillRect(startPosition, 0, width, canvas.height - 20);
}

export function drawTitle(canvasId, startPosition, title) {
    var canvas = document.getElementById(canvasId);
    const ctx = canvas.getContext('2d');
    ctx.fillStyle = "#0284c7";
    ctx.font = "12px Mono";
    ctx.fillText(title, startPosition + 4, canvas.height);
}

export function drawNegative(canvasId, startPosition, width, count) {
    var canvas = document.getElementById(canvasId);
    const ctx = canvas.getContext('2d');
    ctx.fillStyle = "#ff0000";
    ctx.fillRect(startPosition, 0, width, canvas.height - 20);
}