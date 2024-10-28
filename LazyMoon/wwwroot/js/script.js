function saveAsFile(fileName, byteBase64) {
    const linkSource = `data:application/octet-stream;base64,${byteBase64}`;
    const downloadLink = document.createElement("a");
    downloadLink.href = linkSource;
    downloadLink.download = fileName;
    downloadLink.click();
}

window.onReady = () => {
    showMySlides(0, "slides-database");
    showMySlides(0, "slides-web");
    showMySlides(0, "slides-mobile");

    function showMySlides(slideIndex, slideClassName) {
        var i;
        var slides = document.getElementsByClassName(slideClassName);

        for (i = 0; i < slides.length; i++) {
            slides[i].style.display = "none";
        }
        slideIndex++;
        if (slideIndex > slides.length) {
            slideIndex = 1
        }
        slides[slideIndex - 1].style.display = "block";

        setTimeout(() => { showMySlides(slideIndex, slideClassName) }, 2000);
    }

    $(".image-logo").mouseenter(function () {
        $("img", this).css('animation', 'shadow 0.05s linear forwards');
    });
    $(".image-logo").mouseleave(function () {
        $("img", this).css('animation', 'none');
    });

    $(window).on('scroll', function () {
        findPosition();
    });

    function findPosition() {
        var id;
        $('section').each(function () {
            if (id == undefined)
                id = $(this).attr('id');
            if (($(this).offset().top - $(window).scrollTop()) < 20) {
                if ($(this).attr('id') != undefined)
                    id = $(this).attr('id');
            }
        });
        DotNet.invokeMethodAsync('LazyMoon', 'FindPosition', id)        
    }

    findPosition();
}

window.onMarkDownReady = () => {
    $("#markdonw-text").on("change keyup paste", function () {
        var value = $("#markdonw-text").val();
        DotNet.invokeMethodAsync('LazyMoon', 'MarkDownTextChange', value);
    });
}

function disableDragstart() {
    document.getElementById('lazy-stiker').ondragstart = function () { return false; };
}

var dictObject = {};

function setId(id, set) {
    dictObject[id] = set;
}
function gotoScroll(id) {
    var my_element = document.getElementById(id);

    my_element.scrollIntoView({
        behavior: "smooth",
        block: "start",
        inline: "nearest"
    });
}

document.onmousemove = function (e) {
    for (var key in dictObject) {
        var client = document.getElementById(key).getBoundingClientRect();
        if (dictObject[key] == 'Top')
            DotNet.invokeMethodAsync('LazyMoon', 'GetIdSize', key, client.width, client.top);
        if (dictObject[key] == 'Bottom')
            DotNet.invokeMethodAsync('LazyMoon', 'GetIdSize', key, client.width, client.bottom);
        
    }
    var mouseX = e.pageX;
    var mouseY = e.clientY;
    DotNet.invokeMethodAsync('LazyMoon', 'MousePosition', mouseX, mouseY);
}

window.loadKakaoAdScript = function () {
    const script = document.createElement("script");
    script.src = "https://t1.daumcdn.net/kas/static/ba.min.js";
    script.charset = "utf-8";
    script.async = true;
    document.body.appendChild(script);
};