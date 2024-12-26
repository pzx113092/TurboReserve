document.addEventListener("DOMContentLoaded", function () {
    const jumbotron = document.querySelector(".jumbotron");
    jumbotron.style.opacity = 0;
    setTimeout(() => {
        jumbotron.style.transition = "opacity 1s";
        jumbotron.style.opacity = 1;
    }, 200);
});
