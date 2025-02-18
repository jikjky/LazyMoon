﻿function saveAsFile(fileName, byteBase64) {
    const linkSource = `data:application/octet-stream;base64,${byteBase64}`;
    const downloadLink = document.createElement("a");
    downloadLink.href = linkSource;
    downloadLink.download = fileName;
    downloadLink.click();
    downloadLink.remove();
}
window.loadKakaoAdScript = function () {
    const script = document.createElement("script");
    script.src = "https://t1.daumcdn.net/kas/static/ba.min.js";
    script.async = true;
    document.body.appendChild(script);
};


window.scrollToBottom = function () {
    var container = document.getElementById("chatContainer");
    if (container) {
        container.scrollTop = container.scrollHeight;
    }
};
