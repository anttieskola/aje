export function focusElement(elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        element.focus();
    }
}

var outputStarted = false;

export function outputStart() {
    outputStarted = true;
    setTimeout(() => {
        scrollToBottom();
    }, 100);
}

export function outputEnd() {
    outputStarted = false;
}

export function scrollToBottom() {
    if (!outputStarted) {
        window.scrollTo(0, document.body.scrollHeight);
        setTimeout(() => {
            scrollToBottom();
        }, 100);
    }
}
