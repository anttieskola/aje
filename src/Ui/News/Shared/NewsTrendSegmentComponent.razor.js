// 1. text to be displayed
// 2. x coordinate
// 3. y coordinate
// 4. max width of text (optional)
export function drawTitles(
    canvasId,
    startPosition,
    title,
    maxCount,
    countNegative,
    countNeutral,
    countPositive,
    countUnknown) {
    var canvas = document.getElementById(canvasId);
    const ctx = canvas.getContext('2d');

    // bar title (timeperiod)
    // sky-600 (#0284c7)
    ctx.fillStyle = "#0284c7";
    ctx.font = "12px Mono";
    ctx.fillText(title, startPosition + 4, canvas.height);

    // calculations
    var barAreaHeight = canvas.height - 20;
    var oneHeight = barAreaHeight / maxCount;

    // count style stone 50 (#fafaf9)
    ctx.fillStyle = "#fafaf9";

    // unknown count
    if (countUnknown != 0) {
        var unknownCenterY = barAreaHeight - (oneHeight * countUnknown / 2);
        ctx.fillText(countUnknown, startPosition + 10, unknownCenterY);
    }

    // negative count
    if (countNegative != 0) {
        var negativeCenterY = barAreaHeight - (oneHeight * (countUnknown + (countNegative / 2)));
        ctx.fillText(countNegative, startPosition + 10, negativeCenterY);
    }

    var negativeCenterY = barAreaHeight - (oneHeight * (countUnknown + (countNegative / 2)));
    ctx.fillText(countNegative, startPosition + 10, negativeCenterY);

    // neutrals count
    if (countNeutral != 0) {
        var neutralCenterY = barAreaHeight - (oneHeight * (countUnknown + countNegative + (countNeutral / 2)));
        ctx.fillText(countNeutral, startPosition + 10, neutralCenterY);
    }

    // positives count
    if (countPositive != 0) {
        var positiveCenterY = barAreaHeight - (oneHeight * (countUnknown + countNegative + countNeutral + (countPositive / 2)));
        ctx.fillText(countPositive, startPosition + 10, positiveCenterY);
    }
}

// 1. x coordinate
// 2. y coordinate
// 3. width
// 4. height
export function drawBoxes(
    canvasId,
    startPosition,
    width,
    maxCount,
    countNegative,
    countNeutral,
    countPositive,
    countUnknown) {
    var canvas = document.getElementById(canvasId);
    const ctx = canvas.getContext('2d');
    // calculations
    var barAreaHeight = canvas.height - 20;
    var oneHeight = barAreaHeight / maxCount;

    // unknowns slate-600 (#475569)
    ctx.fillStyle = "#475569";
    ctx.fillRect(startPosition, barAreaHeight - (oneHeight * countUnknown), width, (oneHeight * countUnknown));

    // negatives red-600 (#dc2626)
    ctx.fillStyle = "#dc2626";
    ctx.fillRect(startPosition, barAreaHeight - (oneHeight * (countNegative + countUnknown)), width, (oneHeight * countNegative));

    // neutrals  orange-600 (#ea580c)
    ctx.fillStyle = "#ea580c";
    ctx.fillRect(startPosition, barAreaHeight - (oneHeight * (countNeutral + countNegative + countUnknown)), width, (oneHeight * countNeutral));

    // positives green-600 (#16a34a)
    ctx.fillStyle = "#16a34a";
    ctx.fillRect(startPosition, barAreaHeight - (oneHeight * (countPositive + countNeutral + countNegative + countUnknown)), width, (oneHeight * countPositive));
}
