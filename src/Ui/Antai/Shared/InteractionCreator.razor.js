export function focusElement(elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        element.focus();
        console.log("focusElement: " + elementId);
    }
}

var outputStarted = false;

export function outputStart() {
    console.log("outputStart");
    outputStarted = true;
    setTimeout(() => {
        scrollToBottom();
    }, 100);
}

export function outputEnd() {
    console.log("outputEnd");
    outputStarted = false;
}

export function scrollToBottom() {
    console.log("scrollToBottom");
    if (!outputStarted) {
        window.scrollTo(0, document.body.scrollHeight);
        setTimeout(() => {
            scrollToBottom();
        }, 100);
    }
}
