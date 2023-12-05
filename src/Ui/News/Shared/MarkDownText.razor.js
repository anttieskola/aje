export function renderMarkDownText(elementId) {
    var element = document.getElementById(elementId);
    var text = element.innerHTML;
    // magic with AI making regex:es
    // remove "\["" and "\]""
    text = text.replace(/\\\[/g, '');
    text = text.replace(/\\\]/g, '');
    // bold
    text = text.replace(/\*\*(.*?)\*\*/g, '<b>$1</b>');
    // links
    text = text.replace(/\[(.*?)\]\((.*?)\)/g, '<a target="_blank" href="$2">$1</a>');
    element.innerHTML = text;
}