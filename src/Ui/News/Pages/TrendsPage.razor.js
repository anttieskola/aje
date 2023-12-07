// 1. text to be displayed
// 2. x coordinate
// 3. y coordinate
// 4. max width of text (optional)
export function drawLabels(canvasId, maxCount) {
    const canvas = document.getElementById(canvasId);
    const ctx = canvas.getContext('2d');
    // max count
    // sky-600 (#0284c7)
    ctx.fillStyle = "#0284c7";
    ctx.font = "12px Mono";
    ctx.fillText(maxCount, 0, 20);
    ctx.fillText(0, 0, canvas.height - 20);

    // count label (90 degrees counter clockwise)
    ctx.save();
    ctx.translate(20, canvas.height / 2);
    ctx.rotate(-Math.PI / 2);
    ctx.fillText("count", 0, -10);
    ctx.restore();
}